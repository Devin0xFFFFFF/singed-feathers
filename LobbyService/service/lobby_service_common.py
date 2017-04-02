import json
import uuid
import time
import boto3
import decimal
from boto3.dynamodb.conditions import Key

LOBBY_NAME = 'LobbyName'
HOST_PLAYER = 'HostPlayer'
JOIN_PLAYER = 'JoinPlayer'
MAP_ID = 'MapID'
NUM_PLAYERS = 'NumPlayers'
IS_PUBLIC = 'IsPublic'
LOBBY_ID = 'LobbyID'
CREATION_TIME = 'CreationTime'
PLAYERS = 'Players'
GAME_ID = 'GameID'
PLAYER_ID = 'PlayerID'
LEAVE_PLAYER_ID = 'LeavePlayerID'
READY_PLAYER_ID = 'ReadyPlayerID'
PLAYER_STATE = 'PlayerState'
IS_READY = 'IsReady'

PLAYER_STATE_LOBBY_UNREADY = 0
PLAYER_STATE_LOBBY_READY = 1

SUCCESS_CODE = 0
NOT_IN_LOBBY_CODE = 1
LOBBY_FULL_CODE = 1
ALREADY_IN_LOBBY_CODE = 2
GAME_STARTED_CODE = 2

GAME_ID_DEFAULT = '0'

LOBBY_TABLE_NAME = 'SingedFeathersLobbies'

dynamo = boto3.resource('dynamodb', region_name='us-west-2')
table = dynamo.Table(LOBBY_TABLE_NAME)

lambda_client = boto3.client('lambda', region_name='us-west-2')


def create_lobby(lobby):
    return table.put_item(Item=lobby)


def get_public_lobbies():
    scan_result = table.scan(
        Select='ALL_ATTRIBUTES',
        FilterExpression=Key(IS_PUBLIC).eq(True))
    return scan_result['Items']


def get_lobby(lobby_id):
    query_result = table.query(KeyConditionExpression=Key(LOBBY_ID).eq(lobby_id))
    return query_result['Items'][0]


def get_lobby_info(lobby_id, fields):
    query_result = table.query(
        KeyConditionExpression=Key(LOBBY_ID).eq(lobby_id),
        ProjectionExpression=','.join(fields))
    return query_result['Items'][0]


def add_players_to_lobby(lobby_id, new_players):
    update_response = table.update_item(
        Key={LOBBY_ID: lobby_id},
        UpdateExpression="SET #p = list_append(#p, :v)",
        ExpressionAttributeNames={'#p': PLAYERS},
        ExpressionAttributeValues={':v': new_players},
        ReturnValues='UPDATED_NEW'
    )
    return update_response['Attributes'][PLAYERS]


def remove_player_from_lobby(lobby_id, player_index):
    return table.update_item(
        Key={LOBBY_ID: lobby_id},
        UpdateExpression="REMOVE " + PLAYERS + "[" + str(player_index) + "]",
        ReturnValues='UPDATED_NEW')


def set_player_ready_in_lobby(lobby_id, players, player_index):
    return table.update_item(
        Key={LOBBY_ID: lobby_id},
        UpdateExpression="SET #p[" + str(player_index) + "] = :v",
        ExpressionAttributeNames={'#p': PLAYERS},
        ExpressionAttributeValues={':v': players[player_index]},
        ReturnValues='UPDATED_NEW'
    )


def hide_lobby(lobby_id):
    return table.update_item(
        Key={LOBBY_ID: lobby_id},
        UpdateExpression="SET " + IS_PUBLIC + " = :v",
        ExpressionAttributeValues={':v': False},
        ReturnValues='UPDATED_NEW')


def invoke_create_game(lobby_id):
    lambda_client.invoke(
        FunctionName='CreateGame',
        InvocationType='Event',
        Payload=json.dumps({LOBBY_ID: lobby_id})
    )


def get_player_index(players, player_id):
    for index, player in enumerate(players):
        if player[PLAYER_ID] == player_id:
            return index
    return -1


def count_ready_players(players):
    total_ready_players = 0
    for player in players:
        if player[PLAYER_STATE] == PLAYER_STATE_LOBBY_READY:
            total_ready_players += 1
    return total_ready_players


def is_lobby_empty(players, removed_count=1):  # Assume we just removed a player
    return len(players) - removed_count == 0


def player_in_lobby(players, player_id):
    return not get_player_index(players, player_id) == -1


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


def get_result_response(result_code, result_message):
    if isinstance(result_message, str):
        serialized_result_message = result_message
    else:
        serialized_result_message = json.dumps(result_message, default=json_decimal_default)
    return get_response({"ResultCode": result_code, "ResultMessage": serialized_result_message})


def get_success_response(result_message):
    return get_result_response(SUCCESS_CODE, result_message)


def get_already_in_lobby_response():
    return get_result_response(ALREADY_IN_LOBBY_CODE, "Already in Lobby.")


def get_not_in_lobby_response():
    return get_result_response(NOT_IN_LOBBY_CODE, "Not in Lobby.")


def get_lobby_full_response():
    return get_result_response(LOBBY_FULL_CODE, "Lobby is Full.")


def is_lobby_full(players, num_players):
    return len(players) >= num_players


def is_game_id_set(game_id):
    return not game_id == GAME_ID_DEFAULT


def get_lobby_uuid():
    return 'Lobby' + str(uuid.uuid4())


def get_unix_time():
    return int(time.time())
