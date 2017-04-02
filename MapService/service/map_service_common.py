import json
import uuid
import time
import boto3
import decimal

MAP_ID = "MapID"
MAP_NAME = "MapName"
CREATOR_NAME = "CreatorName"
CREATION_TIME = "CreationTime"
MAP_TYPE = "MapType"
SERIALIZED_MAP_DATA = "SerializedMapData"

MAP_TABLE_NAME = 'SingedFeathersMaps'
MAP_BUCKET_NAME = 'singed-feathers-maps'

dynamo = boto3.resource('dynamodb', region_name='us-west-2')
table = dynamo.Table(MAP_TABLE_NAME)
s3 = boto3.resource('s3', region_name='us-west-2')


def get_map_s3key(map_id):
    return map_id + '.json'


def put_map_info(map_info):
    return table.put_item(Item=map_info)


def get_all_maps():
    return table.scan(Select='ALL_ATTRIBUTES')['Items']


def get_map_data(map_id):
    obj = s3.Object(MAP_BUCKET_NAME, get_map_s3key(map_id)).get()
    return obj['Body'].read()


def put_map_data(map_id, map_data):
    return s3.Object(MAP_BUCKET_NAME, get_map_s3key(map_id)).put(Body=map_data)


def json_decimal_default(obj):  # Convert any Decimals to ints, we have no float values
    if isinstance(obj, decimal.Decimal):
        return int(obj)
    raise TypeError


def get_event_body(event):
    return json.loads(event['body'])


def get_event_query_param(event, param):
    return event['queryStringParameters'][param]


def get_response(body=''):
    if isinstance(body, str):
        serialized_body = body
    else:
        serialized_body = json.dumps(body, default=json_decimal_default)
    return {"statusCode": 200, "headers": {"Access-Control-Allow-Origin": "*"}, "body": serialized_body}


def get_map_uuid():
    return 'Map' + str(uuid.uuid4())


def get_unix_time():
    return int(time.time())
