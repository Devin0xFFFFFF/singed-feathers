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
    body_json = event['body'].encode("utf-8")
    join_info = json.loads(body_json)
    
    lobby_id = join_info["LobbyID"]
    join_player = join_info["JoinPlayer"]
    
    logger.info("Received request to join Lobby: " + lobby_id + " by " + str(join_player))
    
    table = dynamo.Table('SingedFeathersLobbies')
    
    query_result = table.query(
        KeyConditionExpression=Key('LobbyID').eq(lobby_id),
        ProjectionExpression=','.join(['Players', 'NumPlayers']))
        
    lobby = query_result['Items'][0]
    num_players = int(lobby['NumPlayers'])
    players = lobby['Players']
    
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"} }
    
    for player in players:
            if player['PlayerID'] == join_player['PlayerID']:
                logger.info('Player already in lobby: ' + str(players))
                response['body'] = json.dumps({ "ResultCode": 2, "ResultMessage": "Already in Lobby." })
                return response
    
    if len(players) >= num_players:
        logger.info('Lobby is full, Players = NumPlayers = ' + str(num_players) + ': ' + str(players))
        response['body'] = json.dumps({ "ResultCode": 1, "ResultMessage": "Lobby is Full." })
        return response
                
    new_players = [join_player]
    
    update_response = table.update_item(
        Key={"LobbyID": lobby_id},
        UpdateExpression="SET #p = list_append(#p, :v)",
        ExpressionAttributeNames = {'#p': 'Players'},
        ExpressionAttributeValues = {':v': new_players},
        ReturnValues='UPDATED_NEW'
        )
        
    updated_players = update_response['Attributes']['Players']
    
    logger.info('Player ' + join_player['PlayerID'] + ' joined Lobby ' + lobby_id + ' Players: ' + str(update_response))
    
    response['body'] = json.dumps({ "ResultCode": 0, "ResultMessage": "Joined Lobby." })
    return response
