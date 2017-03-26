import logging

from . import map_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    map_info = service.get_event_body(event)

    map_name = map_info[service.MAP_NAME]
    creator_name = map_info[service.CREATOR_NAME]
    map_type = map_info[service.MAP_TYPE]
    map_data = map_info[service.SERIALIZED_MAP_DATA]
    map_id = service.get_map_uuid()
    creation_time = service.get_unix_time()

    logger.info("Received request to create Map: " + map_id + " - " + map_name + " by " + creator_name)

    service.MapS3Client().put_map_data(map_id, map_data)

    map_item = {service.MAP_ID: map_id,
                service.MAP_NAME: map_name,
                service.CREATOR_NAME: creator_name,
                service.CREATION_TIME: creation_time,
                service.MAP_TYPE: map_type}

    service.MapDynamoClient().put_item(map_item)

    return service.get_response({"MapID": map_id})
