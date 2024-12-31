class_name ChooseTab
extends Button

var root: Node

func _ready() -> void:
	root = get_parent().get_parent().get_parent()
	self.pressed.connect(_on_pressed)
	
func _on_pressed():
	root.choose(get_index(), self.text)
