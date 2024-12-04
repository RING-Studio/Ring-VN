extends Control

var Callback: Callable
var Texts: Array[String]

func _ready() -> void:
	for child in $MarginContainer/HBoxContainer.get_children():
		$MarginContainer/HBoxContainer.remove_child(child)
	for text in Texts:
		var tab = ChooseTab.new()
		tab.text = text
		tab.add_theme_font_size_override("", 40)
		$MarginContainer/HBoxContainer.add_child(tab)

func choose(index: int, text := ""):
	Callback.call(index, text)
	#TODO: 选项卡消失动画
	queue_free()
