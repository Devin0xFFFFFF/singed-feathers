import pytest
from service import lobby_service_common
from mock import Mock, patch


def test_get_event_body():
    event = {"body": "{}"}
    body = lobby_service_common.get_event_body(event)
    assert body == {}


def test_get_event_query_param():
    params = {"queryStringParameters": {"Key": "Val"}}
    param = lobby_service_common.get_event_query_param(params, "Key")
    assert param == "Val"


def test_get_event_query_param_bad_key():
    with pytest.raises(KeyError):
        params = {"queryStringParameters": {"Key": "Val"}}
        lobby_service_common.get_event_query_param(params, "BadKey")


def test_get_response_empty():
    response = lobby_service_common.get_response()
    assert response == {'statusCode': 200, 'body': '', 'headers': {'Access-Control-Allow-Origin': '*'}}


def test_get_response_string():
    response = lobby_service_common.get_response('string_response')
    assert response == {'statusCode': 200, 'body': 'string_response', 'headers': {'Access-Control-Allow-Origin': '*'}}


def test_get_response_object():
    response = lobby_service_common.get_response({"Key": "Val"})
    assert response == {'statusCode': 200, 'body': '{"Key": "Val"}', 'headers': {'Access-Control-Allow-Origin': '*'}}


def test_get_lobby_uuid():
    lobbyid = lobby_service_common.get_lobby_uuid()
    assert lobbyid.startswith('Lobby') and len(lobbyid) == 41


def test_get_player_index():
    players = [{"PlayerID": "1"}]
    player_index = lobby_service_common.get_player_index(players, "1")
    assert player_index == 0


def test_count_ready_players():
    players = [{"PlayerID": "1", "PlayerState": 1}, {"PlayerID": "2", "PlayerState": 0}]
    count = lobby_service_common.count_ready_players(players)
    assert count == 1


def test_is_lobby_empty():
    players = [{"PlayerID": "1"}]
    assert lobby_service_common.is_lobby_empty(players)


def test_player_in_lobby():
    players = [{"PlayerID": "1"}]
    assert lobby_service_common.player_in_lobby(players, "1")


def test_get_result_response_string():
    response = lobby_service_common.get_result_response(0, 'string_response')
    assert '"ResultCode": 0' in response['body'] and '"ResultMessage": "string_response"' in response['body']


def test_get_result_response_object():
    response = lobby_service_common.get_result_response(0, {"Key": "Val"})
    assert '"ResultCode": 0' in response['body'] and '"ResultMessage": "{\\"Key\\": \\"Val\\"}"' in response['body']


def test_get_success_response():
    response = lobby_service_common.get_success_response('string_response')
    assert '"ResultCode": 0' in response['body'] and '"ResultMessage": "string_response"' in response['body']


def test_get_already_in_lobby_response():
    response = lobby_service_common.get_already_in_lobby_response()
    assert '"ResultCode": 2' in response['body']


def test_get_not_in_lobby_response():
    response = lobby_service_common.get_not_in_lobby_response()
    assert '"ResultCode": 1' in response['body']


def test_get_lobby_full_response():
    response = lobby_service_common.get_lobby_full_response()
    assert '"ResultCode": 1' in response['body']


def test_is_lobby_full():
    players = [{"PlayerID": "1", "PlayerState": 1}, {"PlayerID": "2", "PlayerState": 0}]
    num_players = 2
    assert lobby_service_common.is_lobby_full(players, num_players)


def test_is_game_id_set():
    game_id = "Game1"
    assert lobby_service_common.is_game_id_set(game_id)
