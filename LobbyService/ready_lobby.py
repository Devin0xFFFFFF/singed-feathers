from __future__ import print_function

import json
import urllib
import boto3
import uuid
import time
import logging
from boto3.dynamodb.conditions import Key, Attr

logger = logging.getLogger()
logger.setLevel(logging.INFO)

dynamo = boto3.resource('dynamodb')
lambda_client = boto3.client('lambda')

def lambda_handler(event, context):
    body_json = event['body'].encode("utf-8")
    ready_info = json.loads(body_json)
    
    lobby_id = ready_info["LobbyID"]
    ready_player_id = ready_info["ReadyPlayerID"]
    is_ready = ready_info["IsReady"]
    
    logger.info("Received request to set ready in Lobby: " + lobby_id + " by " + ready_player_id)
    
    table = dynamo.Table('SingedFeathersLobbies')
    
    query_result = table.query(
        KeyConditionExpression=Key('LobbyID').eq(lobby_id),
        ProjectionExpression=','.join(['Players', 'NumPlayers', 'GameID']))
        
    lobby = query_result['Items'][0]
    game_id = lobby['GameID']
    num_players = int(lobby['NumPlayers'])
    players = lobby['Players']
    
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"} }
    
    if(not game_id == "0"): # Fast-fail if game is already active
        logger.info('Game already in session: ' + game_id)
        response['body'] = json.dumps({ "ResultCode": 2, "ResultMessage": "Game already in Session." })
        return response
    
    ready_player_index = -1
    total_ready_players = 0
    for index, player in enumerate(players):
            if player['PlayerID'] == ready_player_id:
                ready_player_index = index
                player['IsReady'] = is_ready
            if player['IsReady']:
                total_ready_players += 1
                
    if ready_player_index == -1:
        logger.info('Could not find player in lobby: ' + str(players))
        response['body'] = json.dumps({ "ResultCode": 1, "ResultMessage": "Not in Lobby." })
        return response
    
    update_response = table.update_item(
        Key={"LobbyID": lobby_id},
        UpdateExpression="SET #p[" + str(ready_player_index) + "] = :v",
        ExpressionAttributeNames = {'#p': 'Players'},
        ExpressionAttributeValues = {':v': players[ready_player_index]},
        ReturnValues='UPDATED_NEW'
        )
    
    ready_text = 'Set ' + ('ready' if is_ready else 'not ready') + ' in Lobby'
    logger.info('Player ' + ready_player_id + ' ' + ready_text + ': ' + lobby_id)
    
    if total_ready_players == num_players: # All players are ready, create a game
        logger.info("All players are ready, hiding lobby")
        update_response = table.update_item(
            Key={"LobbyID": lobby_id},
            UpdateExpression="SET IsPublic = :v",
            ExpressionAttributeValues = {':v': False},
            ReturnValues='UPDATED_NEW'
        )
        
        logger.info("Calling lambda function CreateGame")
        lambda_client.invoke(
            FunctionName='CreateGame',
            InvocationType='Event',
            Payload=json.dumps({ "LobbyID": lobby_id })
        )
        
    
    response['body'] = json.dumps({ "ResultCode": 0, "ResultMessage": ready_text + '.' })
    return response
