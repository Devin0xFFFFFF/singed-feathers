import logging

import lobby_service_common as service

logger = logging.getLogger()
logger.setLevel(logging.INFO)


def lambda_handler(event, context):
    join_info = service.get_event_body(event)
    
    lobby_id = join_info[service.LOBBY_ID]
    join_player = join_info[service.JOIN_PLAYER]
    
    logger.info("Received request to join Lobby: " + lobby_id + " by " + str(join_player))
        
    lobby = service.get_lobby_info(lobby_id, [service.PLAYERS, service.NUM_PLAYERS])
    num_players = lobby[service.NUM_PLAYERS]
    players = lobby[service.PLAYERS]

    if service.player_in_lobby(players, join_player[service.PLAYER_ID]):
        logger.info('Player already in lobby: ' + str(players))
        return service.get_already_in_lobby_response()
    
    if service.is_lobby_full(players, num_players):
        logger.info('Lobby is full, Players = NumPlayers = ' + str(num_players) + ': ' + str(players))
        return service.get_lobby_full_response()
                
    new_players = [join_player]
    
    add_response = service.add_players_to_lobby(lobby_id, new_players)
    
    logger.info('Player ' + join_player['PlayerID'] + ' joined Lobby ' + lobby_id + ' Players: ' + str(add_response))

    return service.get_success_response("Joined Lobby.")
