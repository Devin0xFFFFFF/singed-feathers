import pytest
from mock import Mock, patch

from service import create_map

valid_event_body = {
    "body": "{ \"MapName\": \"testmap\", \"CreatorName\": \"testcreator\", \"MapType\": \"testtype\", "
            "\"SerializedMapData\": \"{}\" } "
}


@patch('service.map_service_common.put_map_data')
@patch('service.map_service_common.put_map_info')
def test_create_map(put_map_info_mock, put_map_data_mock):
    response = create_map.lambda_handler(valid_event_body, None)

    put_map_data_mock.assert_called()
    put_map_info_mock.assert_called()

    assert response['statusCode'] == 200


@patch('service.map_service_common.put_map_data')
def test_get_maps_s3_failure(put_map_data_mock):
    with pytest.raises(IOError):
        put_map_data_mock.side_effect = Mock(side_effect=IOError('S3 Exception'))

        create_map.lambda_handler(valid_event_body, None)
    put_map_data_mock.assert_called()


@patch('service.map_service_common.put_map_data')
@patch('service.map_service_common.put_map_info')
def test_get_maps_dynamo_failure(put_map_info_mock, put_map_data_mock):
    with pytest.raises(IOError):
        put_map_info_mock.side_effect = Mock(side_effect=IOError('Dynamo Exception'))

        create_map.lambda_handler(valid_event_body, None)
