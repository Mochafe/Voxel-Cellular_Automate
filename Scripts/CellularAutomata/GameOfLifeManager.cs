using Godot;
using System;
using System.Collections.Generic;

enum CellType
{
    None,
    Cell
}
public class GameOfLifeManager : Node2D
{
    TextureRect textureRect;

    [Export]
    Color color = new Color();

    [Export]
    private int _width = 160;
    [Export]
    private int _height = 90;

    CellType[,] cells;


    ImageTexture imageTexture = new ImageTexture();

    public override void _Ready()
    {
        cells = new CellType[_width, _height];

        cells[10, 15] = CellType.Cell;
        cells[11, 15] = CellType.Cell;
        cells[10, 16] = CellType.Cell;


        textureRect = GetChild<TextureRect>(1);
    }

    public void UpdateCell()
    {
        ProcessCell();
        ProcessTexture();
    }

    //INPUT
    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton eventMouseButton)
        {
            if(eventMouseButton.IsAction("add"))
            {
                ModifyCell(new Vector2((_width / GetViewport().GetVisibleRect().Size.x) * eventMouseButton.Position.x, (_height / GetViewport().GetVisibleRect().Size.y) * eventMouseButton.Position.y), CellType.Cell);
            }
        }
    }
    //CELL
    private void ProcessCell()
    {
        CellType[,] cellsBuffer = new CellType[_width, _height];
        int neightbours = 0;


        for (int x = 1; x < _width - 1; x++)
        {
            for (int y = 1; y < _height - 1; y++)
            {
                neightbours = GetNeightbourCount(ref cells, new Vector2(x, y));

                if(cells[x, y] == CellType.Cell)
                {
                    cellsBuffer[x, y] = CellType.Cell;
                }

                if (cells[x, y] == CellType.Cell &&
                (neightbours > 3 || neightbours < 2)) {
                    cellsBuffer[x, y] = CellType.None;
                }


                if (cells[x, y] == CellType.None && neightbours == 3)
                {
                    cellsBuffer[x, y] = CellType.Cell;
                }
            }
        }

        cells = cellsBuffer;
    }

    private int GetNeightbourCount(ref CellType[,] cellsArg, Vector2 position)
    {
        int neightbour = 0;

        //Up
        if (cellsArg[(int)position.x, (int)position.y - 1] == CellType.Cell)
        {
            neightbour++;
        }

        //Down
        if (cellsArg[(int)position.x, (int)position.y + 1] == CellType.Cell)
        {
            neightbour++;
        }

        //Left
        if (cellsArg[(int)position.x - 1, (int)position.y] == CellType.Cell)
        {
            neightbour++;
        }

        //Right
        if (cellsArg[(int)position.x + 1, (int)position.y] == CellType.Cell)
        {
            neightbour++;
        }

        //Top Left
        if (cellsArg[(int)position.x - 1, (int)position.y - 1] == CellType.Cell)
        {
            neightbour++;
        }

        //Top Right
        if (cellsArg[(int)position.x + 1, (int)position.y - 1] == CellType.Cell)
        {
            neightbour++;
        }

        //Bottom Left
        if (cellsArg[(int)position.x - 1, (int)position.y + 1] == CellType.Cell)
        {
            neightbour++;
        }

        //Bottom Right
        if (cellsArg[(int)position.x + 1, (int)position.y + 1] == CellType.Cell)
        {
            neightbour++;
        }

        return neightbour;
    }

    private void ModifyCell(Vector2 position, CellType type)
    {
        cells[(int)position.x, (int)position.y] = type;
        ProcessTexture();
    }


    //TEXTURE
    private void ProcessTexture()
    {
        Image image = new Image();
        image.Create(_width, _height, false, Image.Format.Rgba8);

        image.Lock();

        //Draw Cell
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (cells[x, y] == CellType.Cell)
                {
                    image.SetPixel(x, y, color);
                }
            }
        }

        image.Unlock();

        image.Resize((int)GetViewport().GetVisibleRect().Size.x, (int)GetViewport().GetVisibleRect().Size.y, Image.Interpolation.Nearest);


        imageTexture.CreateFromImage(image);

        textureRect.Texture = imageTexture;
    }

    //SIGNAL

    public void OnPauseToggle(bool pause)
    {
        GetNode<Timer>("Tick").Paused = pause;
    }

    public void OnTickChange(float tick)
    {
        GetNode<Timer>("Tick").WaitTime = 1f / tick;

    }

}
