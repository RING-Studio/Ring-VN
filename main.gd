extends Node2D

var ring_sc

var canvas = preload("res://Runtime/AVGRuntime/Canvas/Canvas.cs")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	#var sc = AssetServer.load_script("res://scriptdemo.md")
	#ring_sc = sc
	#Interpreter.load_script(sc)
	var logo := load("res://icon.svg")
	var c = canvas.new()
	c.AddTexture("红叶", load("res://assets/chara.png"), Vector2.ZERO, 0, false)
	#var file = FileAccess.open("res://log", FileAccess.READ_WRITE)
	#file.store_var(c.get_child(0), true)
	#file = FileAccess.open("res://log", FileAccess.READ_WRITE)
	var bytes = c.Serialize()
	var c2 = canvas.new()
	c2.Deserialize(bytes)
	add_child(c)
	var img = c.get_children()[0]
	print(img)
	add_child(img)
	#var c2 = bytes_to_var_with_objects(bytes)
	#add_child(c)
	#var chara := Character.new(["logo"], [logo])
	#add_child(chara)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
