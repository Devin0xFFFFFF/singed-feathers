import logging

import lobby_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    logger.info("Received request to get lobbies.")

    lobbies = service.get_public_lobbies()

    return service.get_response({"Lobbies": lobbies})
