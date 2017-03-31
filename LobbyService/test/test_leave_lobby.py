import pytest
from mock import Mock, patch

from service import leave_lobby

valid_event_body = {"body": "{ \"LeavePlayerID\": \"Player2\", \"LobbyID\": "
                            "\"Lobby2464c3ee-3022-484e-8e3d-80dc3776a89a\" }"}

valid_get_lobby_info = {
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player2", "PlayerName": "Joe", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}

not_in_get_lobby_info = {
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player3", "PlayerName": "Tom", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}

empty_get_lobby_info = {
    "Players": [
        {"PlayerID": "Player2", "PlayerName": "Joe", "PlayerSideSelection": 0, "PlayerState": 0},
    ]
}


@patch('lobby_service_common.get_lobby_info')
@patch('lobby_service_common.remove_player_from_lobby')
def test_leave_lobby(remove_player_from_lobby_mock, get_lobby_info_mock):
    get_lobby_info_mock.return_value = valid_get_lobby_info

    response = leave_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()
    remove_player_from_lobby_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 0" in response['body']


@patch('lobby_service_common.get_lobby_info')
def test_leave_lobby_not_in(get_lobby_info_mock):
    get_lobby_info_mock.return_value = not_in_get_lobby_info

    response = leave_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 1" in response['body']


@patch('lobby_service_common.get_lobby_info')
@patch('lobby_service_common.remove_player_from_lobby')
@patch('lobby_service_common.hide_lobby')
def test_leave_lobby_empty(hide_lobby_mock, remove_player_from_lobby_mock, get_lobby_info_mock):
    get_lobby_info_mock.return_value = empty_get_lobby_info

    response = leave_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()
    remove_player_from_lobby_mock.assert_called()
    hide_lobby_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 0" in response['body']


@patch('lobby_service_common.get_lobby_info')
def test_leave_lobby_dynamo_lobby_info_failure(get_lobby_info_mock):
    with pytest.raises(IOError):
        get_lobby_info_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        leave_lobby.lambda_handler(valid_event_body, None)


@patch('lobby_service_common.get_lobby_info')
@patch('lobby_service_common.remove_player_from_lobby')
def test_leave_lobby_dynamo_remove_player_failure(remove_player_from_lobby_mock, get_lobby_info_mock):
    with pytest.raises(IOError):
        get_lobby_info_mock.return_value = valid_get_lobby_info
        remove_player_from_lobby_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        leave_lobby.lambda_handler(valid_event_body, None)
