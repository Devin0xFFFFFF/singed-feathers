import pytest
from mock import Mock, patch


from service import get_maps


@patch('map_service_common.get_all_maps')
def test_get_maps(get_all_maps_mock):
    get_all_maps_mock.return_value = {}

    response = get_maps.lambda_handler({}, None)

    valid_response = {'statusCode': 200, 'body': '{"Maps": {}}', 'headers': {'Access-Control-Allow-Origin': '*'}}

    get_all_maps_mock.assert_called()

    assert response == valid_response


@patch('map_service_common.get_all_maps')
def test_get_maps_dynamo_failure(get_all_maps_mock):
    with pytest.raises(IOError):
        get_all_maps_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        get_maps.lambda_handler({}, None)
