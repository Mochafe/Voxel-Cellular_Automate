using Godot;
using System;

public class VoxelSimpleHUD : CanvasLayer
{
    [Signal]
    public delegate void OnWireFrame(bool state);
    [Signal]
    public delegate void OnOptimization(bool state);

    public void OnWireframeToggle(bool state)
    {
        EmitSignal(nameof(OnWireFrame), state);
    }

    public void OnOptimizationToggle(bool state)
    {
        EmitSignal(nameof(OnOptimization), state);

    }
}
