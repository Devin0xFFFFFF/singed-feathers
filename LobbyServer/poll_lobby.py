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

def lambda_handler(event, context):
    poll_info = event['queryStringParameters']
    
    lobby_id = poll_info["LobbyID"]
    player_id = poll_info["PlayerID"]
    
    logger.info("Received request to poll Lobby: " + lobby_id + " by " + player_id)
    
    table = dynamo.Table('SingedFeathersLobbies')
    
    query_result = table.query(KeyConditionExpression=Key('LobbyID').eq(lobby_id))
        
    lobby = query_result['Items'][0]
    game_id = lobby['GameID']
    players = lobby['Players']
    
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"} }
    
    player_index = -1
    for index, player in enumerate(players):
            if player['PlayerID'] == player_id:
                player_index = index
                break
                
    if player_index == -1: # Do not respond with lobby info if player is not in the lobby
        logger.info('Could not find player in lobby: ' + str(players))
        response['body'] = json.dumps({ "ResultCode": 1, "ResultMessage": "Not in Lobby." })
        return response
        
    if(not game_id == "0"): # Return GameID if game already in session
        logger.info('Game already in session: ' + game_id)
        response['body'] = json.dumps({ "ResultCode": 2, "ResultMessage": game_id })
        return response
        
    
    lobby['CreationTime'] = int(lobby['CreationTime'])
    lobby['NumPlayers'] = int(lobby['NumPlayers'])
    for player in lobby['Players']:
        player['PlayerSideSelection'] = int(player['PlayerSideSelection'])
    
    response['body'] = json.dumps({ "ResultCode": 0, "ResultMessage": json.dumps(lobby) })
    return response
