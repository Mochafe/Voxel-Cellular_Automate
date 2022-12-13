using Godot;
using System;

namespace GameOfLife
{
    enum CellType
    {
        None,
        Cell
    }
}
public class GameOfLifeManager : Node2D
{
    TextureRect textureRect;




    [Export]
    Color _cellColor = new Color();

    [Export]
    Color _gridColor = new Color();

    [Export]
    private int _width = 160;
    [Export]
    private int _height = 90;

    GameOfLife.CellType[,] cells;


    public override void _Ready()
    {
        cells = new GameOfLife.CellType[_width, _height];

        DrawGrid();

        textureRect = GetNode<TextureRect>("Texture");
    }

    //DRAW GRID
    private void DrawGrid()
    {
        Vector2 viewport = GetViewport().GetVisibleRect().Size;
        Image gridImage = new Image();
        gridImage.Create((int)viewport.x, (int)viewport.y, false, Image.Format.Rgba8);

        gridImage.Lock();

        float spacingX = viewport.x / _width;
        float spacingY = viewport.y / _height;


        //Draw Line
        for (int x = 1; x < _width; x++)
        {
            for (int y = 1; y < _height; y++)
            {
                gridImage.SetPixel((int)(x * spacingX), (int)(y * spacingY), _gridColor);//Center

                gridImage.SetPixel((int)(x * spacingX), (int)(y * spacingY - 1), _gridColor);//Top
                gridImage.SetPixel((int)(x * spacingX), (int)(y * spacingY + 1), _gridColor);//Bottom

                gridImage.SetPixel((int)(x * spacingX - 1), (int)(y * spacingY), _gridColor);//Left
                gridImage.SetPixel((int)(x * spacingX + 1), (int)(y * spacingY), _gridColor);//Right

                gridImage.SetPixel((int)(x * spacingX - 1), (int)(y * spacingY - 1), _gridColor);//Top left
                gridImage.SetPixel((int)(x * spacingX + 1), (int)(y * spacingY - 1), _gridColor);//Top Right

                gridImage.SetPixel((int)(x * spacingX - 1), (int)(y * spacingY + 1), _gridColor);//Bottom left
                gridImage.SetPixel((int)(x * spacingX + 1), (int)(y * spacingY + 1), _gridColor);//Bottom Right

            }
        }

        gridImage.Unlock();

        ImageTexture gridTexture = new ImageTexture();

        gridTexture.CreateFromImage(gridImage);

        GetNode<TextureRect>("Grid").Texture = gridTexture;
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
            //Early return if mouse is not in viewport to prevent crash
            if (eventMouseButton.Position.x > GetViewport().GetVisibleRect().Size.x || eventMouseButton.Position.x < 0)
                return;

            if (eventMouseButton.Position.y > GetViewport().GetVisibleRect().Size.y || eventMouseButton.Position.y < 0)
                return;

            if (eventMouseButton.IsAction("add"))
            {
                ModifyCell(new Vector2((_width / GetViewport().GetVisibleRect().Size.x) * eventMouseButton.Position.x, (_height / GetViewport().GetVisibleRect().Size.y) * eventMouseButton.Position.y), GameOfLife.CellType.Cell);
            }

            if (eventMouseButton.IsAction("remove"))
            {
                ModifyCell(new Vector2((_width / GetViewport().GetVisibleRect().Size.x) * eventMouseButton.Position.x, (_height / GetViewport().GetVisibleRect().Size.y) * eventMouseButton.Position.y), GameOfLife.CellType.None);
            }
        }
    }
    //CELL
    private void ProcessCell()
    {
        GameOfLife.CellType[,] cellsBuffer = new GameOfLife.CellType[_width, _height];
        int neightbours = 0;


        for (int x = 1; x < _width - 1; x++)
        {
            for (int y = 1; y < _height - 1; y++)
            {
                neightbours = GetNeightbourCount(ref cells, new Vector2(x, y));

                if(cells[x, y] == GameOfLife.CellType.Cell)
                {
                    cellsBuffer[x, y] = GameOfLife.CellType.Cell;
                }

                if (cells[x, y] == GameOfLife.CellType.Cell &&
                (neightbours > 3 || neightbours < 2)) {
                    cellsBuffer[x, y] = GameOfLife.CellType.None;
                }


                if (cells[x, y] == GameOfLife.CellType.None && neightbours == 3)
                {
                    cellsBuffer[x, y] = GameOfLife.CellType.Cell;
                }
            }
        }

        cells = cellsBuffer;
    }

    private int GetNeightbourCount(ref GameOfLife.CellType[,] cellsArg, Vector2 position)
    {
        int neightbour = 0;

        //Up
        if (cellsArg[(int)position.x, (int)position.y - 1] == GameOfLife.CellType.Cell)
        {
            neightbour++;
        }

        //Down
        if (cellsArg[(int)position.x, (int)position.y + 1] == GameOfLife.CellType.Cell)
        {
            neightbour++;
        }

        //Left
        if (cellsArg[(int)position.x - 1, (int)position.y] == GameOfLife.CellType.Cell)
        {
            neightbour++;
        }

        //Right
        if (cellsArg[(int)position.x + 1, (int)position.y] == GameOfLife.CellType.Cell)
        {
            neightbour++;
        }

        //Top Left
        if (cellsArg[(int)position.x - 1, (int)position.y - 1] == GameOfLife.CellType.Cell)
        {
            neightbour++;
        }

        //Top Right
        if (cellsArg[(int)position.x + 1, (int)position.y - 1] == GameOfLife.CellType.Cell)
        {
            neightbour++;
        }

        //Bottom Left
        if (cellsArg[(int)position.x - 1, (int)position.y + 1] == GameOfLife.CellType.Cell)
        {
            neightbour++;
        }

        //Bottom Right
        if (cellsArg[(int)position.x + 1, (int)position.y + 1] == GameOfLife.CellType.Cell)
        {
            neightbour++;
        }

        return neightbour;
    }

    private void ModifyCell(Vector2 position, GameOfLife.CellType type)
    {
        if (((int)position.x <= 1 || (int)position.x >= _width - 1) ||
            ((int)position.y <= 1 || (int)position.y >= _height - 1)
            ) return;

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
                if (cells[x, y] == GameOfLife.CellType.Cell)
                {
                    image.SetPixel(x, y, _cellColor);
                }
            }
        }

        image.Unlock();

        image.Resize((int)GetViewport().GetVisibleRect().Size.x, (int)GetViewport().GetVisibleRect().Size.y, Image.Interpolation.Nearest);

        ImageTexture imageTexture = new ImageTexture();

        imageTexture.CreateFromImage(image);

        textureRect.Texture = imageTexture;

        image.Dispose();
        imageTexture.Dispose();
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
