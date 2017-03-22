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
    leave_player_id = join_info["LeavePlayerID"]
    
    logger.info("Received request to leave Lobby: " + lobby_id + " by " + leave_player_id)
    
    table = dynamo.Table('SingedFeathersLobbies')
    
    query_result = table.query(
        KeyConditionExpression=Key('LobbyID').eq(lobby_id),
        ProjectionExpression='Players')
        
    lobby = query_result['Items'][0]
    players = lobby['Players']
    
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"} }
    
    leave_player_index = -1
    for index, player in enumerate(players):
            if player['PlayerID'] == leave_player_id:
                leave_player_index = index
                break
                
    if leave_player_index == -1:
        logger.info('Could not find player in lobby: ' + str(players))
        response['body'] = json.dumps({ "ResultCode": 1, "ResultMessage": "Not in Lobby." })
        return response
    
    update_response = table.update_item(
        Key={"LobbyID": lobby_id},
        UpdateExpression="REMOVE Players[" + str(leave_player_index) + "]",
        ReturnValues='UPDATED_NEW'
    )
    
    logger.info('Player ' + leave_player_id + ' left Lobby ' + lobby_id)
    
    if len(players) - 1 == 0: # No players left, hide lobby
        update_response = table.update_item(
            Key={"LobbyID": lobby_id},
            UpdateExpression="SET IsPublic = :v",
            ExpressionAttributeValues = {':v': 'False'},
            ReturnValues='UPDATED_NEW'
        )
        logger.info("No Players left in lobby, setting to private.")
    
    response['body'] = json.dumps({ "ResultCode": 0, "ResultMessage": "Left Lobby." })
    return response
