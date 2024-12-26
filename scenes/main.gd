class_name GameManager
extends Node

static var cnt := 0

@onready var minigame_scene: PackedScene
var RingIO: Node = preload("res://EAL/RingIO.cs").new()

func game_start():
	get_tree().root.add_child(RingIO)
	var title = $Title
	remove_child(title)
	title.queue_free()
	return
	var tween := create_tween()
	tween.tween_property($Black, "modulate:a", 1.0, 1.0)
	tween.tween_interval(0.5)
	tween.tween_callback(func():
		#RingIO.StartVNRuntime()
		
		)
	tween.tween_property($Black, "modulate:a", 0.0, 1.0)
	tween.tween_callback(func():
		#$Runtime.process_mode = Node.PROCESS_MODE_INHERIT
		)

func load_snapshot():
	var runtime: Node2D = load("res://Runtime/Runtime.cs").new()
	runtime.LoadSnapshot("res://snapshot")
	runtime.name = "Runtime"
	add_child(runtime)
	var title = $Title
	remove_child(title)
	title.queue_free()

func load_tetromino():
	var tetromino = minigame_scene.instantiate()
	tetromino.name = "Tetromino"
	tetromino.cleanup_hook = Callable(end_tetromino)
	$Runtime.process_mode = Node.PROCESS_MODE_DISABLED
	$Runtime.visible = false
	add_child(tetromino)
	
func end_tetromino():
	$Tetromino.queue_free()
	$Runtime.process_mode = Node.PROCESS_MODE_INHERIT
	$Runtime.visible = true

func init_minigame(name: String) -> void:
	match name:
		"Tetromino": load_tetromino()
		_: print("Cannot find minigame " + name)

func get_persist_data(name: String) -> Dictionary:
	return {}

func set_persist_data(name: String, data: Dictionary) -> void:
	pass
