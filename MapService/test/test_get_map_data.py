import pytest
from mock import Mock, patch

from service import get_map_data

valid_query_string_params = {"queryStringParameters": {"MapID": "Map1"}}


@patch('service.map_service_common.get_map_data')
def test_get_map_data(get_map_data_mock):
    get_map_data_mock.return_value = {}

    response = get_map_data.lambda_handler(valid_query_string_params, None)

    valid_response = {'statusCode': 200, 'body': '{}', 'headers': {'Access-Control-Allow-Origin': '*'}}

    get_map_data_mock.assert_called()

    assert response == valid_response


@patch('service.map_service_common.get_map_data')
def test_get_map_data_s3_failure(get_map_data_mock):
    with pytest.raises(IOError):
        get_map_data_mock.side_effect = Mock(side_effect=IOError('S3 Exception'))

        get_map_data.lambda_handler(valid_query_string_params, None)
