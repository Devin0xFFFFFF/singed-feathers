using CoreGame.Controllers;
using CoreGame.Models.API.GameService;
using CoreGame.Models;

using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace GameService
{
    public class Function
    {
        private const string BUCKET_NAME = "singed-feathers-maps";
        private const string TABLE_NAME = "SingedFeathersGames";
        private const string JSON_SUFFIX = ".json";

        private const string MAP_ID = "MapID";
        private const string COMMITTED_TURNS = "CommittedTurns";
        private const string PLAYERS = "Players";

        public bool CommitTurn(CommitTurnRequest commitTurnRequest)
        {
            Document row = GetDynamoTable(commitTurnRequest.GameId);
            Dictionary<string, AttributeValue> dynamoTable = row.ToAttributeMap();

            AttributeValue playerList;
            dynamoTable.TryGetValue(PLAYERS, out playerList);
            IList<Player> players = Player.Deserialize(playerList.S);
            Player player = players.Where(p => p.PlayerID == commitTurnRequest.PlayerId).Single();
            IEnumerable<Player> otherPlayers = players.Where(p => p.PlayerID != commitTurnRequest.PlayerId);
            if (player.IsReady) {
                //don't do anything if the player can't submit a turn

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
                    IList<Delta> deltas = Delta.Deserialize(serializedDeltaList);
                    mapController.ApplyDelta(deltas);
                }

                Delta committedDelta = commitTurnRequest.Delta;
                if (!mapController.ValidateDelta(committedDelta)) {
                    return false;
                }

                int numPlayersWaitingOn = otherPlayers.Where(p => p.IsReady == false).Count();
                if (numPlayersWaitingOn > 0) {

                } else {
                    players.for
                }

                return true;
            } else {
                return false;
            }

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

        private Document GetDynamoTable(string gameId) {
            using (IAmazonDynamoDB client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USWest2)) {
                Table table = Table.LoadTable(client, TABLE_NAME);
                Task<Document> task = table.GetItemAsync(gameId);
                task.Wait();
                return task.Result;
            }
        }
    }
}
