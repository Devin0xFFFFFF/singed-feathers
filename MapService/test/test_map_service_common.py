import pytest
from service import map_service_common
from mock import Mock, patch


def test_get_event_body():
    event = {"body": "{}"}
    body = map_service_common.get_event_body(event)
    assert body == {}


def test_get_event_query_param():
    params = {"queryStringParameters": {"Key": "Val"}}
    param = map_service_common.get_event_query_param(params, "Key")
    assert param == "Val"


def test_get_event_query_param_bad_key():
    with pytest.raises(KeyError):
        params = {"queryStringParameters": {"Key": "Val"}}
        map_service_common.get_event_query_param(params, "BadKey")


def test_get_response_empty():
    response = map_service_common.get_response()
    assert response == {'statusCode': 200, 'body': '', 'headers': {'Access-Control-Allow-Origin': '*'}}


def test_get_response_string():
    response = map_service_common.get_response('string_response')
    assert response == {'statusCode': 200, 'body': 'string_response', 'headers': {'Access-Control-Allow-Origin': '*'}}


def test_get_response_object():
    response = map_service_common.get_response({"Key": "Val"})
    assert response == {'statusCode': 200, 'body': '{"Key": "Val"}', 'headers': {'Access-Control-Allow-Origin': '*'}}


def test_get_map_uuid():
    mapid = map_service_common.get_map_uuid()
    assert mapid.startswith('Map') and len(mapid) == 39
