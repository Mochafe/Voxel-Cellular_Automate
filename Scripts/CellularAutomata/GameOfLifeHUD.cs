using Godot;
using System;

public class GameOfLifeHUD : CanvasLayer
{
    [Signal]
    public delegate void Pause(bool isPaused);
    [Signal]
    public delegate void TickChanged(float value);

    public void PauseToggle(bool pause)
    {
        EmitSignal(nameof(Pause), pause);
    }

    public void OnTickChanged(float value)
    {
        EmitSignal(nameof(TickChanged), value);
    }

    public void OnChangeScenePressed()
    {
        GetTree().ChangeScene("res://Scenes/Voxel/VoxelSimple.tscn");
    }
}
