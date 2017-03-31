import pytest
from mock import Mock, patch

from service import ready_lobby

valid_event_body = {"body": "{ \"ReadyPlayerID\": \"Player2\", \"IsReady\": true, \"LobbyID\": "
                            "\"Lobby2464c3ee-3022-484e-8e3d-80dc3776a89a\" }"}

valid_get_lobby_info = {
    "NumPlayers": 2,
    "GameID": "0",
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player2", "PlayerName": "Joe", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}

not_in_get_lobby_info = {
    "NumPlayers": 2,
    "GameID": "0",
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player3", "PlayerName": "Tom", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}

game_started_get_lobby_info = {
    "NumPlayers": 2,
    "GameID": "Game1",
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player2", "PlayerName": "Joe", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}

all_ready_get_lobby_info = {
    "NumPlayers": 2,
    "GameID": "0",
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 1},
        {"PlayerID": "Player2", "PlayerName": "Joe", "PlayerSideSelection": 1, "PlayerState": 1}
    ]
}


@patch('lobby_service_common.get_lobby_info')
@patch('lobby_service_common.set_player_ready_in_lobby')
def test_ready_lobby(set_player_ready_in_lobby_mock, get_lobby_info_mock):
    get_lobby_info_mock.return_value = valid_get_lobby_info

    response = ready_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()
    set_player_ready_in_lobby_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 0" in response['body']


@patch('lobby_service_common.get_lobby_info')
def test_leave_lobby_not_in(get_lobby_info_mock):
    get_lobby_info_mock.return_value = not_in_get_lobby_info

    response = ready_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 1" in response['body']


@patch('lobby_service_common.get_lobby_info')
def test_ready_lobby_game_started(get_lobby_info_mock):
    get_lobby_info_mock.return_value = game_started_get_lobby_info

    response = ready_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 2" in response['body']


@patch('lobby_service_common.get_lobby_info')
@patch('lobby_service_common.set_player_ready_in_lobby')
@patch('lobby_service_common.hide_lobby')
@patch('lobby_service_common.invoke_create_game')
def test_ready_lobby_all_ready(invoke_create_game_mock,
                               hide_lobby_mock,
                               set_player_ready_in_lobby_mock,
                               get_lobby_info_mock):
    get_lobby_info_mock.return_value = all_ready_get_lobby_info

    response = ready_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()
    set_player_ready_in_lobby_mock.assert_called()
    hide_lobby_mock.assert_called()
    invoke_create_game_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 0" in response['body']


@patch('lobby_service_common.get_lobby_info')
def test_ready_lobby_dynamo_lobby_info_failure(get_lobby_info_mock):
    with pytest.raises(IOError):
        get_lobby_info_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        ready_lobby.lambda_handler(valid_event_body, None)


@patch('lobby_service_common.get_lobby_info')
@patch('lobby_service_common.set_player_ready_in_lobby')
def test_ready_lobby_dynamo_remove_player_failure(set_player_ready_in_lobby_mock, get_lobby_info_mock):
    with pytest.raises(IOError):
        get_lobby_info_mock.return_value = valid_get_lobby_info
        set_player_ready_in_lobby_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        ready_lobby.lambda_handler(valid_event_body, None)
