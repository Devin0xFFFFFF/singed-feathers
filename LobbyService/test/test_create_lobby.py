import pytest
from mock import Mock, patch

from service import create_lobby

valid_event_body = {
    "body": "{ \"HostPlayer\": { \"PlayerID\": \"Player1\", \"PlayerName\": \"Bob\", \"PlayerSideSelection\": 0, "
            "\"PlayerState\": 0 }, \"LobbyName\": \"Lobby1\", \"MapID\": \"Map1\", \"NumPlayers\": 2, \"IsPublic\": "
            "false } "
    }


@patch('service.lobby_service_common.create_lobby')
def test_create_map(create_lobby_mock):
    response = create_lobby.lambda_handler(valid_event_body, None)

    create_lobby_mock.assert_called()

    assert response['statusCode'] == 200 and "LobbyID" in response['body']


@patch('service.lobby_service_common.create_lobby')
def test_get_maps_dynamo_failure(create_lobby_mock):
    with pytest.raises(IOError):
        create_lobby_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        create_lobby.lambda_handler(valid_event_body, None)
