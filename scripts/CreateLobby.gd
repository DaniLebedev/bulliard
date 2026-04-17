extends Node

var lobby_id = 0

func _ready():
	Steam.lobby_created.connect(_on_lobby_created)
	Steam.lobby_joined.connect(_on_lobby_joined)
	

func create_lobby():
	Steam.createLobby(Steam.LOBBY_TYPE_FRIENDS_ONLY, 4)

func _on_lobby_created(connect, id):
	if connect == 1:
		lobby_id = id
		print("Создано:", id)

		Steam.activateGameOverlayInviteDialog(lobby_id)

func _on_lobby_join_requested(id, friend_id):
	Steam.joinLobby(id)

func _on_lobby_joined(id, perms, locked, response):
	lobby_id = id
	print("Вошёл в лобби:", id)

func _process(delta):
	Steam.run_callbacks()
	
