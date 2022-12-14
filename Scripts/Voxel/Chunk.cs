using Godot;
using Godot.Collections;
using System.Collections.Generic;

namespace Voxel
{

    enum CellType
    {
        None,
        Solid
    }
}
[Tool]
public class Chunk : Godot.MeshInstance
{
    [Export]
    private Vector3 _chunkSize = Vector3.One * 16;
    [Export]
    private int _offset = 1;
    [Export]
    private bool _toggleOptimization = true;
    private bool _toggleOptimizationLast = true;

    [Export]
    private Color _color;

    [Export]
    private bool _toggleWireframe = false;
    private bool _toggleWireframeLast = false;
    Array surfaceArray = new Array();
    List<Vector3> verticies = new List<Vector3>();
    List<Vector3> normales = new List<Vector3>();
    List<Color> colors = new List<Color>();

    (Voxel.CellType, Color)[,,] cells;

    Vector3 lastVal = Vector3.One;
    StaticBody staticBody = new StaticBody();
    CollisionShape collisionShape = new CollisionShape();

    Chunk()
    {

        cells = new (Voxel.CellType, Color)[(int)_chunkSize.x, (int)_chunkSize.y, (int)_chunkSize.z];

        for (int x = 0; x < _chunkSize.x; x++)
        {
            for (int y = 0; y < _chunkSize.y; y++)
            {
                for (int z = 0; z < _chunkSize.z; z++)
                {
                    cells[x, y, z] = (Voxel.CellType.None, _color);
                }
            }
        }
    }
    public override void _Ready()
    {
        staticBody.AddChild(collisionShape);
        AddChild(staticBody);
    }
    public void Remove(Vector3 position)
    {
        position = position.Round();
        cells[(int)position.x, (int)position.y, (int)position.z] = (Voxel.CellType.None, _color);

        Draw();
    }

    public void Add(Vector3 position)
    {
        position = position.Round();
        if (position.x == 0 || position.x == _chunkSize.x - 1 ||
            position.y == 0 || position.y == _chunkSize.y - 1 ||
            position.z == 0 || position.z == _chunkSize.z - 1
            )
            return;

        cells[(int)position.x, (int)position.y, (int)position.z] = (Voxel.CellType.Solid, _color);

        Draw();
    }

    public override void _Process(float delta)
    {
        if (lastVal != _chunkSize)
        {
            cells = new (Voxel.CellType, Color)[(int)_chunkSize.x, (int)_chunkSize.y, (int)_chunkSize.z];


            for (int x = 1; x < _chunkSize.x; x++)
            {
                for (int y = 1; y < _chunkSize.y; y++)
                {
                    for (int z = 0; z < _chunkSize.z; z++)
                    {
                        cells[x, y, z] = (x == 0 || x == _chunkSize.x - 1 || y == 0 || y == _chunkSize.y - 1 || z == 0 || z == _chunkSize.z - 1) ? (Voxel.CellType.None, _color) : (Voxel.CellType.Solid, _color);

                    }
                }
            }


            Draw();

            lastVal = _chunkSize;
        }

        if (_toggleWireframe != _toggleWireframeLast)
        {
            Draw();
            _toggleWireframeLast = _toggleWireframe;
        }

        if (_toggleOptimization != _toggleOptimizationLast)
        {
            Draw();
            _toggleOptimizationLast = _toggleOptimization;
        }
    }

    public void Draw()
    {
        verticies = new List<Vector3>();
        normales = new List<Vector3>();
        colors = new List<Color>();

        surfaceArray.Resize((int)ArrayMesh.ArrayType.Max);

        for (int x = 1; x < _chunkSize.x - 1; x++)
        {
            for (int y = 1; y < _chunkSize.y - 1; y++)
            {
                for (int z = 1; z < _chunkSize.z - 1; z++)
                {
                    if (cells[x, y, z].Item1 != Voxel.CellType.None)
                    {
                        if (_toggleOptimization)
                        {
                            CreateAt(new Vector3(x, y, z),
                                cells[x, y, z].Item2,
                            (cells[x, y, z + 1].Item1 == Voxel.CellType.None) ? true : false,
                            (cells[x, y, z - 1].Item1 == Voxel.CellType.None) ? true : false,
                            (cells[x - 1, y, z].Item1 == Voxel.CellType.None) ? true : false,
                            (cells[x + 1, y, z].Item1 == Voxel.CellType.None) ? true : false,
                            (cells[x, y + 1, z].Item1 == Voxel.CellType.None) ? true : false,
                            (cells[x, y - 1, z].Item1 == Voxel.CellType.None) ? true : false
                            );
                        }
                        else
                        {
                            CreateAt(new Vector3(x, y, z), cells[x, y, z].Item2);
                        }

                    }
                }
            }
        }
        CreateMesh();
    }


    private void CreateMesh()
    {
        surfaceArray[(int)Mesh.ArrayType.Vertex] = verticies;
        surfaceArray[(int)Mesh.ArrayType.Normal] = normales;
        surfaceArray[(int)Mesh.ArrayType.Color] = colors;



        ArrayMesh arrayMesh = new ArrayMesh();

        arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

        Mesh = arrayMesh;



        //CreateTrimeshCollision();
        collisionShape.Shape = Mesh.CreateTrimeshShape();

        //TO SEE SHAPE
        if (_toggleWireframe)
        {
            Mesh = collisionShape.Shape.GetDebugMesh();
        }

    }

    private void CreateAt(Vector3 position, Color color, bool front = true, bool back = true, bool left = true, bool right = true, bool top = true, bool bottom = true)
    {
        int offset = verticies.Count;
        //position += Vector3.One / 2;

        //Front Face
        if (front)
        {
            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);
            colors.Add(color);


            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);
            colors.Add(color);
        }


        //Back face
        if (back)
        {
            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);
            colors.Add(color);


            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);
            colors.Add(color);
        }




        //Top Face
        if (top)
        {
            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Up);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Up);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Up);
            colors.Add(color);


            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Up);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Up);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Up);
            colors.Add(color);
        }


        //Bottom Face
        if (bottom)
        {
            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Down);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Down);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Down);
            colors.Add(color);



            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Down);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Down);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Down);
            colors.Add(color);
        }


        //Left Face
        if (left)
        {
            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Left);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Left);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Left);
            colors.Add(color);


            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Left);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Left);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Left);
            colors.Add(color);
        }

        //Right Face
        if (right)
        {
            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Right);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Right);
            colors.Add(color);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Right);
            colors.Add(color);


            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Right);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Right);
            colors.Add(color);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Right);
            colors.Add(color);
        }


        /* //Front vertices
         verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);//0
         normales.Add((Vector3.Up + Vector3.Left + Vector3.Back).Normalized());

         verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);//1
         normales.Add((Vector3.Up + Vector3.Right + Vector3.Back).Normalized());



         verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);//2
         normales.Add((Vector3.Down + Vector3.Right + Vector3.Back).Normalized());

         verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);//3
         normales.Add((Vector3.Down + Vector3.Left + Vector3.Back).Normalized());

         //Back vertices
         verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position); //4
         normales.Add((Vector3.Up + Vector3.Left + Vector3.Forward).Normalized());

         verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);//5
         normales.Add((Vector3.Up + Vector3.Right + Vector3.Forward).Normalized());

         verticies.Add((Vector3.Down + Vector3.Right + Vector3.Forward) / 2 + position);//6
         normales.Add((Vector3.Down + Vector3.Right + Vector3.Forward).Normalized());

         verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);//7
         normales.Add((Vector3.Down + Vector3.Left + Vector3.Forward).Normalized());*/








        /*//Front
        if (front)
        {
            indices.Add(0 + offset);
            indices.Add(2 + offset);
            indices.Add(3 + offset);

            indices.Add(0 + offset);
            indices.Add(1 + offset);
            indices.Add(2 + offset);
        }

        //Back
        if (back)
        {
            indices.Add(4 + offset);
            indices.Add(7 + offset);
            indices.Add(6 + offset);

            indices.Add(4 + offset);
            indices.Add(6 + offset);
            indices.Add(5 + offset);
        }

        //Top
        if (top)
        {
            indices.Add(0 + offset);
            indices.Add(4 + offset);
            indices.Add(1 + offset);

            indices.Add(1 + offset);
            indices.Add(4 + offset);
            indices.Add(5 + offset);
        }

        //Bottom
        if (bottom)
        {
            indices.Add(7 + offset);
            indices.Add(3 + offset);
            indices.Add(6 + offset);

            indices.Add(6 + offset);
            indices.Add(3 + offset);
            indices.Add(2 + offset);
        }

        //Left
        if (left)
        {
            indices.Add(7 + offset);
            indices.Add(4 + offset);
            indices.Add(3 + offset);

            indices.Add(3 + offset);
            indices.Add(4 + offset);
            indices.Add(0 + offset);
        }

        //Right
        if (right)
        {
            indices.Add(2 + offset);
            indices.Add(1 + offset);
            indices.Add(6 + offset);

            indices.Add(6 + offset);
            indices.Add(1 + offset);
            indices.Add(5 + offset);
        }*/

    }

    public void OnWireFrameToggle(bool state)
    {
        _toggleWireframe = state;
    }

    public void OnOptimizationToggle(bool state)
    {
        _toggleOptimization = state;
    }

    public void OnColorChanged(Color color)
    {
        _color = color;
    }
}
