from __future__ import print_function

import json
import urllib
import boto3
import uuid
import time
import logging

logger = logging.getLogger()
logger.setLevel(logging.INFO)

s3 = boto3.resource('s3')
dynamo = boto3.resource('dynamodb')

def lambda_handler(event, context):
    body_json = event['body'].encode("utf-8")
    map_info = json.loads(body_json)
    
    map_name = map_info["MapName"]
    creator_name = map_info["CreatorName"]
    map_type = map_info["MapType"]
    map_data = map_info["SerializedMapData"]
    map_id = 'Map' + str(uuid.uuid4())
    creation_time = int(time.time())
    
    logger.info("Received request to create Map: " + map_id + " - " + map_name + " by " + creator_name)
    
    s3_key = map_id + '.json'
    obj = s3.Object('singedfeathersmaps', s3_key).put(Body = map_data)
    
    dynamo_item = {"MapID": map_id, "MapName": map_name, "CreatorName": creator_name, "CreationTime": creation_time, "MapType": map_type}
    
    table = dynamo.Table('SingedFeathersMaps')
    
    table.put_item(Item=dynamo_item)
    
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"}, "body": json.dumps({ "MapID": map_id }) }
    return response
