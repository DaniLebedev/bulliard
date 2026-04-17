extends Node


func _ready() -> void:
	pass


func _process(delta: float) -> void:
	pass




func _on_play_button_down() -> void:
	get_tree().change_scene_to_file("res://levels/Bar/Bar.tscn");
	pass
