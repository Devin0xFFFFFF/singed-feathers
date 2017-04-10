using Amazon.Lambda.APIGatewayEvents;
using CoreGame.Models;
using CoreGame.Models.API.GameService;
using CoreGame.Models.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace GameService {
    public class GameServiceTest {

        private void TestFullGame() {
            GameService gameService = new GameService();
            string lobbyId = "TestLobby";

            string gameId = gameService.CreateGame(lobbyId);
            Assert.StartsWith("Game", gameId);

            string player1Id = "Player1Id";
            string player2Id = "Player2Id";

            APIGatewayProxyRequest commitRequest = new APIGatewayProxyRequest();
            CommitTurnRequest commitRequestObject;
            Delta delta;
            APIGatewayProxyResponse commitResponse;

            APIGatewayProxyRequest pollRequest = new APIGatewayProxyRequest();
            PollRequest pollRequestObject;
            APIGatewayProxyResponse pollResponse;
            PollResponse pollResponseObject;
            string nullResponse = JsonConvert.SerializeObject(new PollResponse(false, new List<Delta>()));

            // Turn One

            delta = new Delta(new Position(0, 0), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            Assert.Equal(nullResponse, pollResponse.Body);

            delta = new Delta(new Position(2, 3), new Command(MoveType.Water, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 0);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 0);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 2);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 3);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 0);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 0);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 2);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 3);

            // Turn Two

            delta = new Delta(new Position(0, 0), new Command(MoveType.Water, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            Assert.Equal(nullResponse, pollResponse.Body);

            delta = new Delta(new Position(3, 6), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 3);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 6);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 0);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 0);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 3);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 6);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 0);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 0);

            // Turn Three

            delta = new Delta(new Position(0, 3), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = new Delta(new Position(3, 6), new Command(MoveType.Water, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 0);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 3);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 3);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 6);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 0);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 3);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 3);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 6);

            // Turn Four

            delta = new Delta(new Position(0, 3), new Command(MoveType.Water, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = new Delta(new Position(3, 6), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 3);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 6);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 0);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 3);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 3);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 6);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 0);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 3);

            // Turn Five

            delta = new Delta(new Position(7, 3), new Command(MoveType.Water, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 1);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 7);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 3);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 1);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Water);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 7);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 3);

            // Turn Six

            delta = new Delta(new Position(7, 3), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = new Delta(new Position(7, 3), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 7);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 3);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 7);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 3);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 7);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 3);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 7);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 3);

            // Turn Seven

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            // Turn Eight

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            // Turn Nine

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            // Turn Ten

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            // Turn extra

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("false", commitResponse.Body);

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("false", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            Assert.Equal(nullResponse, pollResponse.Body);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            Assert.Equal(nullResponse, pollResponse.Body);
        }

        private void TestQuickGame() {
            GameService gameService = new GameService();
            string lobbyId = "TestLobby";

            string gameId = gameService.CreateGame(lobbyId);
            Assert.StartsWith("Game", gameId);

            string player1Id = "Player1Id";
            string player2Id = "Player2Id";

            APIGatewayProxyRequest commitRequest = new APIGatewayProxyRequest();
            CommitTurnRequest commitRequestObject;
            Delta delta;
            APIGatewayProxyResponse commitResponse;

            APIGatewayProxyRequest pollRequest = new APIGatewayProxyRequest();
            PollRequest pollRequestObject;
            APIGatewayProxyResponse pollResponse;
            PollResponse pollResponseObject;
            string nullResponse = JsonConvert.SerializeObject(new PollResponse(false, new List<Delta>()));

            // Turn One

            delta = new Delta(new Position(4, 1), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            Assert.Equal(nullResponse, pollResponse.Body);

            delta = new Delta(new Position(6, 7), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 4);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 1);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 6);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 7);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 4);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 1);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 6);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 7);

            // Turn Two

            delta = new Delta(new Position(7, 6), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            Assert.Equal(nullResponse, pollResponse.Body);

            delta = new Delta(new Position(5, 4), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 5);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 4);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 7);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 6);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 5);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 4);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 7);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 6);

            // Turn Three

            delta = new Delta(new Position(6, 5), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = new Delta(new Position(6, 3), new Command(MoveType.Fire, 100));
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 6);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 5);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 6);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 3);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 2);
            Assert.Equal(pollResponseObject.Turn[0].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[1].Command.MoveType, MoveType.Fire);
            Assert.Equal(pollResponseObject.Turn[0].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[1].Command.Heat, 100);
            Assert.Equal(pollResponseObject.Turn[0].Position.X, 6);
            Assert.Equal(pollResponseObject.Turn[0].Position.Y, 5);
            Assert.Equal(pollResponseObject.Turn[1].Position.X, 6);
            Assert.Equal(pollResponseObject.Turn[1].Position.Y, 3);

            // Turn Four

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("true", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            pollResponseObject = JsonConvert.DeserializeObject<PollResponse>(pollResponse.Body);
            Assert.True(pollResponseObject.IsValid);
            Assert.Equal(pollResponseObject.Turn.Count, 0);

            // Turn extra

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player1Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("false", commitResponse.Body);

            delta = null;
            commitRequestObject = new CommitTurnRequest(gameId, player2Id, delta);
            commitRequest.Body = JsonConvert.SerializeObject(commitRequestObject);
            commitResponse = gameService.CommitTurn(commitRequest);
            Assert.Equal("false", commitResponse.Body);

            pollRequestObject = new PollRequest(gameId, player2Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            Assert.Equal(nullResponse, pollResponse.Body);

            pollRequestObject = new PollRequest(gameId, player1Id);
            pollRequest.Body = JsonConvert.SerializeObject(pollRequestObject);
            pollResponse = gameService.Poll(pollRequest);
            Assert.Equal(nullResponse, pollResponse.Body);
        }

        public APIGatewayProxyResponse TestGameService() {
            APIGatewayProxyResponse response = new APIGatewayProxyResponse();
            response.StatusCode = 200;
            response.Headers = new Dictionary<string, string>();
            response.Headers.Add(GameService.ACCESS_CONTROL_ALLOW_ORIGIN, GameService.ACCESS_CONTROL_ALLOW_ORIGIN_VALUE);
            
            try {
                TestFullGame();
                TestQuickGame();
                response.Body = "0";
            } catch (Xunit.Sdk.XunitException e) {
                response.Headers.Add("Exception", e.ToString());
                response.Body = "2";
            } catch (Exception e) {
                response.Headers.Add("Exception", e.ToString());
                response.Body = "3";
            }
            return response;
        }
    }
}
