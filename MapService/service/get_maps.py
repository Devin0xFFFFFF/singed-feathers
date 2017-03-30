import logging

import map_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    logger.info("Received request to get maps.")

    maps = service.get_all_maps()
    print(maps)

    return service.get_response(body={"Maps": maps})
