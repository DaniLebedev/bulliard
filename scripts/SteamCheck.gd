extends Node

func _ready() -> void:
	var init = Steam.steamInit()
	print("Steam init:", init)

	if not Steam.isSteamRunning():
		print("Steam не запущен")
		return

	print("Steam работает")

	var name = Steam.getPersonaName()
	print("Username:", name)


func _process(delta: float) -> void:
	Steam.run_callbacks()
