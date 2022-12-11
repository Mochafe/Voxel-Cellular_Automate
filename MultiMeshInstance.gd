tool
extends MultiMeshInstance

const HOW_MANY : int = 10
# Declare member variables here. Examples:
# var a = 2
# var b = "text"
func init():
	multimesh.instance_count = HOW_MANY
	for i in range(multimesh.instance_count):
		var position = Transform()
		position = position.translated(Vector3(i, i, i))
		multimesh.set_instance_transform(i, position)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _Process():
	if Engine.editor_hint:
		init()
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
