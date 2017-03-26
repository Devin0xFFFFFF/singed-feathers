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
    logger.info("Received request to get lobbies.")
    
    table = dynamo.Table('SingedFeathersLobbies')
    
    scan_result = table.scan(
        Select='ALL_ATTRIBUTES',
        FilterExpression=Key('IsPublic').eq(True))
    lobbies = scan_result['Items']
    
    for lobby in lobbies: # Convert DynamoDB Decimal -> Integer
        lobby['CreationTime'] = int(lobby['CreationTime'])
        lobby['NumPlayers'] = int(lobby['NumPlayers'])
        for player in lobby['Players']:
            player['PlayerSideSelection'] = int(player['PlayerSideSelection'])
	
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"}, "body": json.dumps({ "Lobbies": lobbies }) }
    return response
