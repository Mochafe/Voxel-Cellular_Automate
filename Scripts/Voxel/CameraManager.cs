using Godot;
using System;

[Tool]
public class CameraManager : Spatial
{
    [Export]
    private float _length = 7;
    [Export]
    private float _minLength = 10, _maxLength = 20, _step = 1, _zoomSmooth = 4f;
    [Export]
    private Vector3 _position;
    [Export]
    private float _orbitingSpeed = 1f, _orbitingSmooth = 7f;

    private bool _isOrbiting = false;
    private float _x = 0.0f, _y = 0.0f;

    Camera Camera;
    public override void _Ready()
    {
        Camera = GetChild<Camera>(0);
        _x = Rotation.x;
        _y = Rotation.y;

        Iniatialize();
    }

    public void Iniatialize()
    {
        if (Camera.Translation.z != _length)
        {
            Camera.Translation =  new Vector3(Camera.Translation.x, Camera.Translation.y, _length);
        }
        if (Translation != _position)
        {
            Translation = _position;
        }
    }

    public override void _Process(float delta)
    {
        if (Engine.EditorHint)
        {
            Iniatialize();
        }
        else
        {
            _isOrbiting = Input.IsActionPressed("mouse_orbit");


            //Zoom scroll wheel
            if (Input.IsActionJustReleased("scroll_down"))
            {
                float temp = _length;
                temp += _step;
                temp = Mathf.Clamp(temp, _minLength, _maxLength);
                _length = temp;
            }

            if (Input.IsActionJustReleased("scroll_up"))
            {
                float temp = _length;
                temp -= _step;
                temp = Mathf.Clamp(temp, _minLength, _maxLength);
                _length = temp;
            }

            //Iniatialize();
            Camera.Translation = Camera.Translation.LinearInterpolate(new Vector3(Camera.Translation.x, Camera.Translation.y, _length), _zoomSmooth * delta);
            Rotation = Rotation.LinearInterpolate(new Vector3(_x, _y, Rotation.z), _orbitingSmooth * delta);

        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_isOrbiting)
        {
            if (@event is InputEventMouseMotion motionEvent)
            {
                _y += -motionEvent.Relative.x * _orbitingSpeed / 100;

                _x +=  -motionEvent.Relative.y * _orbitingSpeed / 100;
                _x = Mathf.Clamp(_x, -Mathf.Pi / 2, Mathf.Pi / 2);
            }
        }

        if(@event.IsActionPressed("remove"))
        {
            var from = Camera.ProjectRayOrigin(GetViewport().GetMousePosition());
            var to = from + Camera.ProjectRayNormal(GetViewport().GetMousePosition()) * 200;

            PhysicsDirectSpaceState world = GetWorld().DirectSpaceState;
            var result = world.IntersectRay(from, to);

            if(result.Count > 0)
            {
                GetNode<Chunk>("../Chunk").Remove((Vector3)result["position"] + -(Vector3)result["normal"] / 2);
            }
        }

        if (@event.IsActionPressed("add"))
        {
            var from = Camera.ProjectRayOrigin(GetViewport().GetMousePosition());
            var to = from + Camera.ProjectRayNormal(GetViewport().GetMousePosition()) * 200;

            PhysicsDirectSpaceState world = GetWorld().DirectSpaceState;
            var result = world.IntersectRay(from, to);

            if (result.Count > 0)
            {
                GetNode<Chunk>("../Chunk").Add((Vector3)result["position"] + (Vector3)result["normal"] / 2);
            }
        }
    }
}
