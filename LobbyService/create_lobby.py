from __future__ import print_function

import json
import urllib
import boto3
import uuid
import time
import logging

logger = logging.getLogger()
logger.setLevel(logging.INFO)

dynamo = boto3.resource('dynamodb')

def lambda_handler(event, context):
    body_json = event['body'].encode("utf-8")
    lobby_info = json.loads(body_json)
    
    lobby_name = lobby_info["LobbyName"]
    host_player = lobby_info["HostPlayer"]
    map_id = lobby_info["MapID"]
    num_players = lobby_info["NumPlayers"]
    is_public = lobby_info["IsPublic"]
    lobby_id = "Lobby" + str(uuid.uuid4())
    creation_time = int(time.time())
    players = [host_player]
    
    logger.info("Received request to create Lobby: " + lobby_id + " - " + lobby_name + " by " + str(host_player))
    
    dynamo_item = {"LobbyID": lobby_id, "LobbyName": lobby_name, "MapID": map_id, "CreationTime": creation_time, "Players": players, "NumPlayers": num_players, "IsPublic": is_public, "GameID": "0"}
    
    table = dynamo.Table('SingedFeathersLobbies')
    
    table.put_item(Item=dynamo_item)
    
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"}, "body": json.dumps({ "LobbyID": lobby_id }) }
    return response
