import pytest
from mock import Mock, patch

from service import get_lobbies


@patch('lobby_service_common.get_public_lobbies')
def test_get_lobbies(get_public_lobbies_mock):
    get_public_lobbies_mock.return_value = {}

    response = get_lobbies.lambda_handler({}, None)

    valid_response = {'statusCode': 200, 'body': '{"Lobbies": {}}', 'headers': {'Access-Control-Allow-Origin': '*'}}

    get_public_lobbies_mock.assert_called()

    assert response == valid_response


@patch('lobby_service_common.get_public_lobbies')
def test_get_lobbies_dynamo_failure(get_public_lobbies_mock):
    with pytest.raises(IOError):
        get_public_lobbies_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        get_lobbies.lambda_handler({}, None)
