from __future__ import print_function

import json
import urllib
import boto3
import uuid
import time
import logging
import decimal
from boto3.dynamodb.conditions import Key

logger = logging.getLogger()
logger.setLevel(logging.INFO)

dynamo = boto3.resource('dynamodb')

def json_decimal_default(obj): # Convert any Decimals to ints, we have no float values
    if isinstance(obj, decimal.Decimal):
        return int(obj)
    raise TypeError
    
def query_dynamo(table_name, key, val):
    table = dynamo.Table(table_name)
    query_result = table.query(KeyConditionExpression=Key(key).eq(val))
    if('Items' in query_result and len(query_result['Items']) > 0):
        return query_result['Items'][0]

def get_response(body={}):
    return {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"}, "body": body }
    
def get_result_response(result_code, result_message):
    if isinstance(result_message, str):
       serialized_result_message = result_message
    else:
        serialized_result_message = json.dumps(result_message, default=json_decimal_default)
    return get_response(json.dumps(
        { "ResultCode": result_code, "ResultMessage": serialized_result_message }, 
        default=json_decimal_default))
    
def find_player(players, player_id):
    player_index = -1
    for index, player in enumerate(players):
        if player['PlayerID'] == player_id:
            player_index = index
            break
    return player_index

def is_game_id_set(game_id):
    return not game_id == "0";

def lambda_handler(event, context):
    poll_info = event['queryStringParameters']
    
    lobby_id = poll_info['LobbyID']
    player_id = poll_info['PlayerID']
    
    logger.info("Received request to poll Lobby: " + lobby_id + " by " + player_id)
        
    lobby = query_dynamo('SingedFeathersLobbies', 'LobbyID', lobby_id)
    game_id = lobby['GameID']
    players = lobby['Players']
    
    if find_player(players, player_id) == -1: # Do not respond with lobby info if player is not in the lobby
        logger.info('Could not find player in lobby: ' + str(players))
        return get_result_response(1, 'Not in Lobby.')
        
    if(is_game_id_set(game_id)): # Return GameID if game already in session
        logger.info('Game already in session: ' + game_id)
        return get_result_response(2, game_id)
    
    return get_result_response(0, lobby)
