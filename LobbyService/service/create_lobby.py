import logging

import lobby_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    lobby_info = service.get_event_body(event)

    lobby_name = lobby_info[service.LOBBY_NAME]
    host_player = lobby_info[service.HOST_PLAYER]
    map_id = lobby_info[service.MAP_ID]
    num_players = lobby_info[service.NUM_PLAYERS]
    is_public = lobby_info[service.IS_PUBLIC]
    lobby_id = service.get_lobby_uuid()
    creation_time = service.get_unix_time()
    players = [host_player]
    map_name = service.get_map_name(map_id)

    logger.info("Received request to create Lobby: " + lobby_id + " - " + lobby_name + " by " + str(host_player))

    lobby_item = {service.LOBBY_ID: lobby_id,
                  service.LOBBY_NAME: lobby_name,
                  service.MAP_ID: map_id,
                  service.MAP_NAME: map_name,
                  service.CREATION_TIME: creation_time,
                  service.PLAYERS: players,
                  service.NUM_PLAYERS: num_players,
                  service.IS_PUBLIC: is_public,
                  service.GAME_ID: service.GAME_ID_DEFAULT}

    service.create_lobby(lobby_item)

    return service.get_response(lobby_id)
