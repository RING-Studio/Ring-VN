@tool
extends Control

@export_enum("Other", "Me") var MessageSender: String

@export var ScreenWidth: float = 768
@export_multiline var text: String = "测试消息😀"

func _ready() -> void:
	self.custom_minimum_size.x = ScreenWidth
	$Text.text = text
	$Text/Back.size = $Text.size + Vector2(80, 40)
	var head_size: Vector2 = $Head.texture.get_size() * $Head.scale
	if MessageSender == "Me":
		var left_margin_width: float = ScreenWidth - $Text/Back.size.x - head_size.x - 20
		$Text.position.x = left_margin_width + 40
		$Head.position.x = $Text.position.x + $Text.size.x + 40 + 10
	if MessageSender == "Other":
		$Head.position = Vector2(10, 10)
		$Text.position.x = $Head.position.x + head_size.x + 50
	self.custom_minimum_size.y = $Text/Back.size.y

func _process(delta: float) -> void:
	$Text/Back.size = $Text.size + Vector2(80, 40)
