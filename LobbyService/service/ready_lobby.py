import logging

import lobby_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    ready_info = service.get_event_body(event)
    
    lobby_id = ready_info[service.LOBBY_ID]
    ready_player_id = ready_info[service.READY_PLAYER_ID]
    is_ready = ready_info[service.IS_READY]
    
    logger.info("Received request to set ready in Lobby: " + lobby_id + " by " + ready_player_id)
    
    lobby = service.get_lobby_info(lobby_id, [service.PLAYERS, service.NUM_PLAYERS, service.GAME_ID])

    game_id = lobby[service.GAME_ID]
    num_players = lobby[service.NUM_PLAYERS]
    players = lobby[service.PLAYERS]
    
    if service.is_game_id_set(game_id):  # Fast-fail if game is already active
        logger.info('Game already in session: ' + game_id)
        return service.get_result_response(service.GAME_STARTED_CODE, "Game already in Session.")

    ready_player_index = service.get_player_index(players, ready_player_id)

    if ready_player_index == -1:
        logger.info('Could not find player in lobby: ' + str(players))
        return service.get_not_in_lobby_response()

    if is_ready:
        players[ready_player_index][service.PLAYER_STATE] = service.PLAYER_STATE_LOBBY_READY
    else:
        players[ready_player_index][service.PLAYER_STATE] = service.PLAYER_STATE_LOBBY_UNREADY
    service.set_player_ready_in_lobby(lobby_id, players, ready_player_index)
    
    ready_text = 'Set ' + ('ready' if is_ready else 'not ready') + ' in Lobby'
    logger.info('Player ' + ready_player_id + ' ' + ready_text + ': ' + lobby_id)
    
    if service.count_ready_players(players) == num_players:  # All players are ready, create a game
        logger.info("All players are ready, hiding lobby")
        service.hide_lobby(lobby_id)
        
        logger.info("Calling lambda function CreateGame")
        service.invoke_create_game(lobby_id)

    return service.get_success_response(ready_text + '.')
