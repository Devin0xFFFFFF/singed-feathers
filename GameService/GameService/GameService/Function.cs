using CoreGame.Controllers;
using CoreGame.Models.API.GameService;
using CoreGame.Models;

using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using CoreGame.Controllers.Interfaces;
using CoreGame.Utility;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace GameService {
    public class Function {
        private const string BUCKET_NAME = "singed-feathers-maps";
        private const string GAME_TABLE_NAME = "SingedFeathersGames";
        private const string LOBBY_TABLE_NAME = "SingedFeathersLobbies";
        private const string JSON_SUFFIX = ".json";
        private const string GAME_PREFIX = "Game";

        private const string MAP_ID = "MapID";
        private const string GAME_ID = "GameID";
        private const string LOBBY_ID = "LobbyID";
        private const string COMMITTED_TURNS = "CommittedTurns";
        private const string STAGED_TURNS = "StagedTurns";
        private const string PLAYERS = "Players";
        private const string PLAYER_NAME = "PlayerName";
        private const string PLAYER_ID = "PlayerID";
        private const string PLAYER_SIDE_SELECTION = "PlayerSideSelection";

        private Player _player;
        private IList<Player> _players;
        private IEnumerable<Player> _otherPlayers;

        private JsonSerializerSettings _settings;

        public APIGatewayProxyResponse CommitTurn(APIGatewayProxyRequest apigProxyEvent)
        {
            APIGatewayProxyResponse response = new APIGatewayProxyResponse();
            response.StatusCode = 200;
            try {
                string input = apigProxyEvent.Body;
                _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                CommitTurnRequest commitTurnRequest = JsonConvert.DeserializeObject<CommitTurnRequest>(input, _settings);
                Dictionary<string, AttributeValue> dynamoTable = GetDynamoTable(commitTurnRequest.GameId, GAME_TABLE_NAME);
                if (dynamoTable == null) {
                    response.Body = ("false");
                    return response;
                }

                SetPlayers(dynamoTable, commitTurnRequest.PlayerId);

                if (_player.PlayerState == PlayerState.NotSubmitted) {
                    // Don't do anything if the player can't submit a turn

                    IMapController mapController = ReplayGameFromTable(dynamoTable);

                    Delta committedDelta = commitTurnRequest.Delta;
                    if (!mapController.ValidateDelta(committedDelta)) {
                        response.Body = ("false");
                        return response;
                    }
                    _player.Delta = committedDelta;
                    _player.PlayerState = PlayerState.Submitted;
                    int playerIndex = _players.IndexOf(_player);

                    dynamoTable = UpdateDynamoPlayers(_player, playerIndex, commitTurnRequest.GameId);
                    if (dynamoTable != null) {
                        SetPlayers(dynamoTable, commitTurnRequest.PlayerId);
                        int numPlayersWaitingOn = _otherPlayers.Where(p => p.PlayerState == PlayerState.NotSubmitted).Count();
                        if (numPlayersWaitingOn == 0) {
                            List<Delta> currentTurn = new List<Delta>();
                            foreach (Player p in _players) {
                                if (p.Delta != null) {
                                    currentTurn.Add(p.Delta);
                                }
                            }
                            string commitedTurn = JsonConvert.SerializeObject(currentTurn, _settings);
                            foreach (Player p in _players) {
                                p.Delta = null;
                                if (p.PlayerState != PlayerState.Quit) {
                                    p.PlayerState = PlayerState.Polling;
                                }
                            }
                            currentTurn = TurnMergeUtility.SortDeltas(currentTurn);
                            UpdateStagedTurn(currentTurn, commitTurnRequest.GameId);
                        }
                    } else {
                        response.Body = ("false");
                        return response;
                    }
                    response.Body = ("true");
                    return response;
                } else {
                    response.Body = ("false");
                    return response;
                }
            } catch (Exception e) {
                Console.Write(e);
                response.Body = ("false");
                return response;
            }
        }

        public APIGatewayProxyResponse Poll(APIGatewayProxyRequest apigProxyEvent) {
            APIGatewayProxyResponse response = new APIGatewayProxyResponse();
            response.StatusCode = 200;
            try {
                string input = apigProxyEvent.Body;
                _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                PollRequest request = JsonConvert.DeserializeObject<PollRequest>(input, _settings);
                Dictionary<string, AttributeValue> dynamoTable = GetDynamoTable(request.GameId, GAME_TABLE_NAME);

                SetPlayers(dynamoTable, request.PlayerId);
                bool canPoll = _player.PlayerState == PlayerState.Polling;

                List<Delta> deltaList = new List<Delta>();
                if (canPoll) {
                    int playerIndex = _players.IndexOf(_player);
                    deltaList = GetStagedTurn(dynamoTable);
                    IMapController mapController = ReplayGameFromTable(dynamoTable);
                    if (mapController.GetTurnsLeft() <= dynamoTable[COMMITTED_TURNS].L.Count || mapController.IsGameOver()) {
                        _player.PlayerState = PlayerState.Quit;
                        UpdateDynamoPlayers(_player, playerIndex, request.GameId);
                        ConditionalCleanUpTable(request.GameId);
                    } else {
                        _player.PlayerState = PlayerState.NotSubmitted;
                        UpdateDynamoPlayers(_player, playerIndex, request.GameId);
                    }
                }
                response.Body = GetPollResponse(canPoll, deltaList);
                return response;
            } catch (Exception e) {
                Console.Write(e);
                response.Body = GetPollResponse(false, null);
                return response;
            }
        }

        public APIGatewayProxyResponse Surrender(APIGatewayProxyRequest apigProxyEvent) {
            APIGatewayProxyResponse response = new APIGatewayProxyResponse();
            response.StatusCode = 200;
            string input = apigProxyEvent.Body;
            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            PollRequest request = JsonConvert.DeserializeObject<PollRequest>(input, _settings);
            Dictionary<string, AttributeValue> dynamoTable = GetDynamoTable(request.GameId, GAME_TABLE_NAME);

            SetPlayers(dynamoTable, request.PlayerId);
            _player.PlayerState = PlayerState.Quit;
            int playerIndex = _players.IndexOf(_player);
            UpdateDynamoPlayers(_player, playerIndex, request.GameId);
            ConditionalCleanUpTable(request.GameId);
            return response;
        }

        public void CreateGame(string lobbyId) {
            AttributeValue playerList;
            Dictionary<string, AttributeValue> playerMap;
            AttributeValue mapId;
            string playerName, playerId;
            PlayerSideSelection side;
            Player p;

            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            Dictionary<string, AttributeValue> dynamoLobbyTable = GetDynamoTable(lobbyId, LOBBY_TABLE_NAME);

            dynamoLobbyTable.TryGetValue(PLAYERS, out playerList);
            dynamoLobbyTable.TryGetValue(MAP_ID, out mapId);

            DynamoDBList players = new DynamoDBList();
            foreach (AttributeValue map in playerList.L) {
                playerMap = map.M;
                playerName = playerMap[PLAYER_NAME].S;
                playerId = playerMap[PLAYER_ID].S;
                if (playerMap[PLAYER_SIDE_SELECTION].N.CompareTo("0") == 0) {
                    side = PlayerSideSelection.SavePigeons;
                } else {
                    side = PlayerSideSelection.BurnPigeons;
                }
                p = new Player(playerId, playerName, side, PlayerState.NotSubmitted);
                players.Add(JsonConvert.SerializeObject(p, _settings));
            }

            string gameId = GAME_PREFIX + Guid.NewGuid().ToString();
            using (IAmazonDynamoDB client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USWest2)) {
                Table table = Table.LoadTable(client, GAME_TABLE_NAME);

                Task<Document> task = table.GetItemAsync(gameId);
                task.Wait();
                while (task.Result != null) {
                    gameId = GAME_PREFIX + Guid.NewGuid().ToString();
                    task = table.GetItemAsync(gameId);
                    task.Wait();
                }

                Document row = new Document();
                row[GAME_ID] = gameId;
                row[COMMITTED_TURNS] = new DynamoDBList();
                row[MAP_ID] = mapId.S;
                row[PLAYERS] = players;
                row[STAGED_TURNS] = "[ ]";

                table.PutItemAsync(row).Wait();
                UpdateGameId(lobbyId, gameId);
            }
        }

        private string GetPollResponse(bool canPoll, List<Delta> deltaList) {
            PollResponse response = new PollResponse(canPoll, deltaList);
            return JsonConvert.SerializeObject(response);
        }

        private string GetMap(string mapId) {
            string responseBody = "";
            using (IAmazonS3 client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2)) {
                Task<GetObjectResponse> task = client.GetObjectAsync(BUCKET_NAME, mapId);
                task.Wait();
                GetObjectResponse response = task.Result;
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream)) {
                    responseBody = reader.ReadToEnd();
                }
            }
            return responseBody;
        }

        private IMapController ReplayGameFromTable(Dictionary<string, AttributeValue> dynamoTable) {
            AttributeValue mapId;
            dynamoTable.TryGetValue(MAP_ID, out mapId);
            string serializedMap = GetMap(mapId.S + JSON_SUFFIX);
            MapController mapController = new MapController();
            bool success = mapController.GenerateMap(serializedMap);

            AttributeValue turnList;
            dynamoTable.TryGetValue(COMMITTED_TURNS, out turnList);
            List<AttributeValue> deltaLists = turnList.L;
            foreach (AttributeValue deltaList in deltaLists) {
                string serializedDeltaList = deltaList.S;
                IList<Delta> deltas = JsonConvert.DeserializeObject<List<Delta>>(serializedDeltaList, _settings);
                mapController.ApplyDelta(deltas);
            }
            return mapController;
        }

        private void SetPlayers(Dictionary<string, AttributeValue> dynamoTable, string playerId) {
            AttributeValue playerList;
            dynamoTable.TryGetValue(PLAYERS, out playerList);
            _players = new List<Player>();
            foreach (AttributeValue p in playerList.L) {
                _players.Add(JsonConvert.DeserializeObject<Player>(p.S, _settings));
            }
            _player = _players.Where(p => p.PlayerID.CompareTo(playerId) == 0).Single();
            _otherPlayers = _players.Where(p => p.PlayerID != playerId);
        }

        private Dictionary<string, AttributeValue> GetDynamoTable(string primaryKey, string tableName) {
            using (IAmazonDynamoDB client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USWest2)) {
                Table table = Table.LoadTable(client, tableName);
                Task<Document> task = table.GetItemAsync(primaryKey);
                task.Wait();
                if (task.Result != null) {
                    return task.Result.ToAttributeMap();
                }
                return null;
            }
        }

        private Dictionary<string, AttributeValue> UpdateDynamoPlayers(Player player, int index, string gameId) {
            using (IAmazonDynamoDB client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USWest2)) {
                UpdateItemRequest update = new UpdateItemRequest {
                    TableName = GAME_TABLE_NAME,
                    Key = new Dictionary<string, AttributeValue>() { { GAME_ID, new AttributeValue { S = gameId } } },
                    ExpressionAttributeNames = new Dictionary<string, string>() {
                            {"#p", PLAYERS}
                        },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>() {
                            {":v", new AttributeValue {S = JsonConvert.SerializeObject(player, _settings) } }
                        },
                    UpdateExpression = "SET #p[" + index + "] = :v",
                    ReturnValues = "ALL_NEW"
                };
                Task<UpdateItemResponse> task = client.UpdateItemAsync(update);
                task.Wait();
                UpdateItemResponse response = task.Result;
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return response.Attributes;
                } else {
                    return null;
                }
            }
        }

        private bool UpdateGameId(string lobbyId, string gameId) {
            using (IAmazonDynamoDB client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USWest2)) {
                UpdateItemRequest update = new UpdateItemRequest {
                    TableName = LOBBY_TABLE_NAME,
                    Key = new Dictionary<string, AttributeValue>() { { LOBBY_ID, new AttributeValue { S = lobbyId } } },
                    ExpressionAttributeNames = new Dictionary<string, string>() {
                            {"#g", GAME_ID}
                        },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>() {
                            {":v", new AttributeValue {S = gameId } }
                        },
                    UpdateExpression = "SET #g = :v",
                };
                Task<UpdateItemResponse> task = client.UpdateItemAsync(update);
                task.Wait();
                UpdateItemResponse response = task.Result;
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
        }

        private bool UpdateStagedTurn(List<Delta> deltas, string gameId) {
            using (IAmazonDynamoDB client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USWest2)) {
                AttributeValue deltaList = new AttributeValue { S = JsonConvert.SerializeObject(deltas) };
                Dictionary<string, AttributeValue> values = new Dictionary<string, AttributeValue>() {
                    {":d", deltaList },
                    {":c", new AttributeValue {L = new List<AttributeValue> { deltaList } } }
                };
                string updateExpression = "SET";
                for (int index = 0; index < _players.Count; index++) {
                    values.Add(":v" + index, new AttributeValue { S = JsonConvert.SerializeObject(_players[index], _settings) });
                    updateExpression += " #p[" + index + "] = :v" + index + ",";
                }
                updateExpression += " #s = :d, #c = list_append(#c, :c)";

                UpdateItemRequest update = new UpdateItemRequest {
                    TableName = GAME_TABLE_NAME,
                    Key = new Dictionary<string, AttributeValue>() { { GAME_ID, new AttributeValue { S = gameId } } },
                    ExpressionAttributeNames = new Dictionary<string, string>() {
                            {"#p", PLAYERS},
                            {"#s", STAGED_TURNS},
                            {"#c", COMMITTED_TURNS}
                        },
                    ExpressionAttributeValues = values,
                    UpdateExpression = updateExpression
                };
                Task<UpdateItemResponse> task = client.UpdateItemAsync(update);
                task.Wait();
                UpdateItemResponse response = task.Result;
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
        }

        private List<Delta> GetStagedTurn(Dictionary<string, AttributeValue> dynamoTable) {
            AttributeValue stagedTurns;
            dynamoTable.TryGetValue(STAGED_TURNS, out stagedTurns);

            return JsonConvert.DeserializeObject<List<Delta>>(stagedTurns.S, _settings);
        }

        private void ConditionalCleanUpTable(string gameId) {
            using (IAmazonDynamoDB client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USWest2)) {
                Table table = Table.LoadTable(client, GAME_TABLE_NAME);
                if (_players.All(p => p.PlayerState == PlayerState.Quit)) {
                    table.DeleteItemAsync(gameId).Wait();
                }
            }
        }
    }
}
