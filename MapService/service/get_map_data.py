import logging

import map_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    map_id = service.get_event_query_param(event, service.MAP_ID)

    logger.info("Received request for map data with MapID: " + map_id)

    map_data = service.get_map_data(map_id)

    return service.get_response(body=map_data)
