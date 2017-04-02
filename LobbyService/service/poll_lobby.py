import logging

import lobby_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    lobby_id = service.get_event_query_param(event, service.LOBBY_ID)
    player_id = service.get_event_query_param(event, service.PLAYER_ID)
    
    logger.info("Received request to poll Lobby: " + lobby_id + " by " + player_id)
        
    lobby = service.get_lobby(lobby_id)
    game_id = lobby[service.GAME_ID]
    players = lobby[service.PLAYERS]
    
    if not service.player_in_lobby(players, player_id):
        logger.info('Could not find player in lobby: ' + str(players))
        return service.get_not_in_lobby_response()

    if service.is_game_id_set(game_id):  # Return GameID if game already in session
        logger.info('Game already in session: ' + game_id)
        return service.get_result_response(service.GAME_STARTED_CODE, game_id)
    
    return service.get_success_response(lobby)
