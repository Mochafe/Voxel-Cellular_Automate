using Godot;
using System;

public class Background : ColorRect
{
    public override void _Ready()
    {
        MarginRight = (int)GetViewport().GetVisibleRect().Size.x;
        MarginBottom = (int)GetViewport().GetVisibleRect().Size.y;
    }
}
