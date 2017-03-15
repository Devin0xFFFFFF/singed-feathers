from __future__ import print_function

import json
import urllib
import boto3
import logging

logger = logging.getLogger()
logger.setLevel(logging.INFO)

s3 = boto3.resource('s3')

def lambda_handler(event, context):
    map_id = event['queryStringParameters']['MapID']
    
    logger.info("Received request for map data with MapID: " + map_id)
    
    key = map_id + '.json'
    obj = s3.Object('singedfeathersmaps', key).get()
    map_data = obj['Body'].read()
    
    response = {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"}, "body": map_data }
    return response
