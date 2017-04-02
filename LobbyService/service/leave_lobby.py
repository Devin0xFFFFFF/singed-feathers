import logging

import lobby_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    leave_info = service.get_event_body(event)
    
    lobby_id = leave_info[service.LOBBY_ID]
    leave_player_id = leave_info[service.LEAVE_PLAYER_ID]
    
    logger.info("Received request to leave Lobby: " + lobby_id + " by " + leave_player_id)

    players = service.get_lobby_info(lobby_id, [service.PLAYERS])[service.PLAYERS]
    
    leave_player_index = service.get_player_index(players, leave_player_id)
                
    if leave_player_index == -1:
        logger.info('Could not find player in lobby: ' + str(players))
        return service.get_not_in_lobby_response()

    service.remove_player_from_lobby(lobby_id, leave_player_index)
    
    logger.info('Player ' + leave_player_id + ' left Lobby ' + lobby_id)
    
    if service.is_lobby_empty(players):
        service.hide_lobby(lobby_id)
        logger.info("No Players left in lobby, setting to private.")

    return service.get_success_response("Left Lobby.")
