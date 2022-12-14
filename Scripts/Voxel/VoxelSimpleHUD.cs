using Godot;
using System;

public class VoxelSimpleHUD : CanvasLayer
{
    [Signal]
    public delegate void OnWireFrame(bool state);
    [Signal]
    public delegate void OnOptimization(bool state);
    [Signal]
    public delegate void ColorChanged(Color color);

    public void OnWireframeToggle(bool state)
    {
        EmitSignal(nameof(OnWireFrame), state);
    }

    public void OnOptimizationToggle(bool state)
    {
        EmitSignal(nameof(OnOptimization), state);

    }

    public void OnColorChanged(Color color)
    {
        EmitSignal(nameof(ColorChanged), color);
    }

    public void OnColorToggle(bool isColor)
    {
        GetNode<Panel>("Panel").Visible = isColor;
    }
}
