extends Control

var Callback: Callable
# C#到GDScript的string类型转换有bug，不能用Array[String]
# 4.4修复了这个bug，升级以后可以去掉这个workaround
var Texts: Array

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
	if self.get_parent():
		self.get_parent().remove_child(self)
	queue_free()
