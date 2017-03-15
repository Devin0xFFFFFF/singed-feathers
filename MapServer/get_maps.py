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
    logger.info("Received request to get maps.")
    
    table = dynamo.Table('SingedFeathersMaps')
    
    scan_result = table.scan(Select='ALL_ATTRIBUTES')
    maps = scan_result['Items']
    
    for map in maps: # Convert DynamoDB Decimal -> Integer
        map['CreationTime'] = int(map['CreationTime'])
	
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"}, "body": json.dumps({ "Maps": maps }) }
    return response
