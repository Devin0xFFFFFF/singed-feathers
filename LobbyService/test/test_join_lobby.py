import pytest
from mock import Mock, patch

from service import join_lobby

valid_event_body = {"body": "{ \"JoinPlayer\": { \"PlayerID\": \"Player2\", \"PlayerName\": \"Joe\", "
                            "\"PlayerSideSelection\": 1, \"PlayerState\": 0 }, \"LobbyID\": "
                            "\"Lobby2464c3ee-3022-484e-8e3d-80dc3776a89a\" }"}

valid_get_lobby_info = {
        "NumPlayers": 2,
        "Players": [
            {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0}
        ]
    }

already_in_get_lobby_info = {
    "NumPlayers": 2,
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player2", "PlayerName": "Joe", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}

full_get_lobby_info = {
    "NumPlayers": 2,
    "Players": [
        {"PlayerID": "Player1", "PlayerName": "Bob", "PlayerSideSelection": 0, "PlayerState": 0},
        {"PlayerID": "Player3", "PlayerName": "Tom", "PlayerSideSelection": 1, "PlayerState": 0}
    ]
}


@patch('service.lobby_service_common.get_lobby_info')
@patch('service.lobby_service_common.add_players_to_lobby')
def test_join_lobby(add_players_to_lobby_mock, get_lobby_info_mock):
    get_lobby_info_mock.return_value = valid_get_lobby_info

    response = join_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()
    add_players_to_lobby_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 0" in response['body']


@patch('service.lobby_service_common.get_lobby_info')
def test_join_lobby_already_in(get_lobby_info_mock):
    get_lobby_info_mock.return_value = already_in_get_lobby_info

    response = join_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 2" in response['body']


@patch('service.lobby_service_common.get_lobby_info')
def test_join_lobby_full(get_lobby_info_mock):
    get_lobby_info_mock.return_value = full_get_lobby_info

    response = join_lobby.lambda_handler(valid_event_body, None)

    get_lobby_info_mock.assert_called()

    assert response['statusCode'] == 200 and "\"ResultCode\": 1" in response['body']


@patch('service.lobby_service_common.get_lobby_info')
def test_join_lobby_dynamo_lobby_info_failure(get_lobby_info_mock):
    with pytest.raises(IOError):
        get_lobby_info_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        join_lobby.lambda_handler(valid_event_body, None)


@patch('service.lobby_service_common.get_lobby_info')
@patch('service.lobby_service_common.add_players_to_lobby')
def test_join_lobby_dynamo_add_players_failure(add_players_to_lobby_mock, get_lobby_info_mock):
    with pytest.raises(IOError):
        get_lobby_info_mock.return_value = valid_get_lobby_info
        add_players_to_lobby_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        join_lobby.lambda_handler(valid_event_body, None)
