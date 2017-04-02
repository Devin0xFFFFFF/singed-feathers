import pytest
from mock import Mock, patch

from service import create_lobby

valid_event_body = {
    "body": "{ \"HostPlayer\": { \"PlayerID\": \"Player1\", \"PlayerName\": \"Bob\", \"PlayerSideSelection\": 0, "
            "\"PlayerState\": 0 }, \"LobbyName\": \"Lobby1\", \"MapID\": \"Map1\", \"NumPlayers\": 2, \"IsPublic\": "
            "false } "
    }


@patch('service.lobby_service_common.get_map_name')
@patch('service.lobby_service_common.create_lobby')
def test_create_lobby(create_lobby_mock, get_map_name_mock):
    get_map_name_mock.return_value = ''

    response = create_lobby.lambda_handler(valid_event_body, None)

    create_lobby_mock.assert_called()
    get_map_name_mock.assert_called()

    assert response['statusCode'] == 200


@patch('service.lobby_service_common.get_map_name')
def test_get_map_name_dynamo_failure(get_map_name_mock):
    with pytest.raises(IOError):
        get_map_name_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        create_lobby.lambda_handler(valid_event_body, None)


@patch('service.lobby_service_common.get_map_name')
@patch('service.lobby_service_common.create_lobby')
def test_create_lobby_dynamo_failure(create_lobby_mock, get_map_name_mock):
    with pytest.raises(IOError):
        get_map_name_mock.return_value = ''
        create_lobby_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        create_lobby.lambda_handler(valid_event_body, None)
