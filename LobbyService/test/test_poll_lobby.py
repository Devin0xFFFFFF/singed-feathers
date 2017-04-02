import pytest
from mock import Mock, patch

from service import poll_lobby

valid_query_string_params = {
    "queryStringParameters": {
        "PlayerID": "Player2",
        "LobbyID": "Lobby1"}
}

valid_get_lobby = {
    "GameID": "0",
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player2", "PlayerName": "Joe", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}

not_in_get_lobby = {
    "GameID": "0",
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player3", "PlayerName": "Tom", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}

game_started_get_lobby = {
    "GameID": "Game1",
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player2", "PlayerName": "Joe", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}


@patch('service.lobby_service_common.get_lobby')
def test_poll_lobby(get_lobby_mock):
    get_lobby_mock.return_value = valid_get_lobby

    response = poll_lobby.lambda_handler(valid_query_string_params, None)

    get_lobby_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 0" in response['body']


@patch('service.lobby_service_common.get_lobby')
def test_poll_lobby_not_in(get_lobby_mock):
    get_lobby_mock.return_value = not_in_get_lobby

    response = poll_lobby.lambda_handler(valid_query_string_params, None)

    get_lobby_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 1" in response['body']


@patch('service.lobby_service_common.get_lobby')
def test_poll_lobby_game_started(get_lobby_mock):
    get_lobby_mock.return_value = game_started_get_lobby

    response = poll_lobby.lambda_handler(valid_query_string_params, None)

    get_lobby_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 2" in response['body']


@patch('service.lobby_service_common.get_lobby')
def test_poll_lobby_dynamo_lobby_info_failure(get_lobby_mock):
    with pytest.raises(IOError):
        get_lobby_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        poll_lobby.lambda_handler(valid_query_string_params, None)
