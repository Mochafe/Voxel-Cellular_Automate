using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


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
    private bool _toggleWireframe = false;
    private bool _toggleWireframeLast = false;
    Array surfaceArray = new Array();
    List<Vector3> verticies = new List<Vector3>();
    List<Vector3> normales = new List<Vector3>();

    int[,,] type;

    Vector3 lastVal = Vector3.One;
    StaticBody staticBody = new StaticBody();
    CollisionShape collisionShape = new CollisionShape();

    Chunk()
    {
        type = new int[(int)_chunkSize.x, (int)_chunkSize.y, (int)_chunkSize.z];

        for (int x = 0; x < _chunkSize.x; x++)
        {
            for (int y = 0; y < _chunkSize.y; y++)
            {
                for (int z = 0; z < _chunkSize.z; z++)
                {
                    type[x, y, z] = 0;
                }
            }
        }
    }
    public override void _Ready()
    {
        staticBody.AddChild(collisionShape);
        AddChild(staticBody);
        Initialize();
    }
    public void Remove(Vector3 position)
    {
        position = position.Round();
        type[(int)position.x, (int)position.y, (int)position.z] = 0;
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

        type[(int)position.x, (int)position.y, (int)position.z] = 1;
        Draw();
    }

    public override void _Process(float delta)
    {
        if (lastVal != _chunkSize)
        {
            type = new int[(int)_chunkSize.x, (int)_chunkSize.y, (int)_chunkSize.z];


            for (int x = 1; x < _chunkSize.x; x++)
            {
                for (int y = 1; y < _chunkSize.y; y++)
                {
                    for (int z = 0; z < _chunkSize.z; z++)
                    {
                        type[x, y, z] = (x == 0 || x == _chunkSize.x - 1 || y == 0 || y == _chunkSize.y - 1 || z == 0 || z == _chunkSize.z - 1) ? 0 : 1;
                        //type[x, y, z] = (x == 0 || x == _chunkSize.x - 1 || y == 0 || y == _chunkSize.y - 1 || z == 0 || z == _chunkSize.z - 1) ? 0 : z % 2;
                        //type[x, y, z] = 0;
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

        surfaceArray.Resize((int)ArrayMesh.ArrayType.Max);

        for (int x = 1; x < _chunkSize.x - 1; x++)
        {
            for (int y = 1; y < _chunkSize.y - 1; y++)
            {
                for (int z = 1; z < _chunkSize.z - 1; z++)
                {
                    if (type[x, y, z] != 0)
                    {
                        if(_toggleOptimization)
                        {
                            CreateAt(new Vector3(x, y, z),
                            (type[x, y, z + 1] == 0) ? true : false,
                            (type[x, y, z - 1] == 0) ? true : false,
                            (type[x - 1, y, z] == 0) ? true : false,
                            (type[x + 1, y, z] == 0) ? true : false,
                            (type[x, y + 1, z] == 0) ? true : false,
                            (type[x, y - 1, z] == 0) ? true : false
                            );
                        } else
                        {
                            CreateAt(new Vector3(x, y, z));
                        }
                        
                    }
                }
            }
        }
        CreateMesh();
    }

    private void Initialize()
    {
        type = new int[(int)_chunkSize.x, (int)_chunkSize.y, (int)_chunkSize.z];


        for (int x = _offset; x < _chunkSize.x - _offset; x++)
        {
            for (int y = _offset; y < _chunkSize.y - _offset; y++)
            {
                for (int z = _offset; z < _chunkSize.z - _offset; z++)
                {
                    type[x, y, z] = 1;
                    //type[x, y, z] = (x == 0 || x == _chunkSize.x - 1 || y == 0 || y == _chunkSize.y - 1 || z == 0 || z == _chunkSize.z - 1) ? 0 : z % 2;
                    //type[x, y, z] = 0;
                }
            }
        }

        type[5, (int)_chunkSize.y - 1, 5] = 0;
        type[5, (int)_chunkSize.y - 2, 5] = 0;
        type[5, (int)_chunkSize.y - 3, 5] = 0;

        type[1, 1, 1] = 0;



        verticies = new List<Vector3>();
        normales = new List<Vector3>();

        surfaceArray.Resize((int)ArrayMesh.ArrayType.Max);

        for (int x = 1; x < _chunkSize.x - 1; x++)
        {
            for (int y = 1; y < _chunkSize.y - 1; y++)
            {
                for (int z = 1; z < _chunkSize.z - 1; z++)
                {
                    if (type[x, y, z] != 0)
                    {
                        CreateAt(new Vector3(x, y, z),
                            (type[x, y, z + 1] == 0) ? true : false,
                            (type[x, y, z - 1] == 0) ? true : false,
                            (type[x - 1, y, z] == 0) ? true : false,
                            (type[x + 1, y, z] == 0) ? true : false,
                            (type[x, y + 1, z] == 0) ? true : false,
                            (type[x, y - 1, z] == 0) ? true : false
                            );
                    }
                }
            }
        }

        CreateMesh();

        lastVal = _chunkSize;
    }

    private void CreateMesh()
    {
        surfaceArray[(int)Mesh.ArrayType.Vertex] = verticies;
        surfaceArray[(int)Mesh.ArrayType.Normal] = normales;


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

    private void CreateAt(Vector3 position, bool front = true, bool back = true, bool left = true, bool right = true, bool top = true, bool bottom = true)
    {
        int offset = verticies.Count;
        //position += Vector3.One / 2;

        //Front Face
        if (front)
        {
            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);

            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);


            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Back);
        }


        //Back face
        if (back)
        {
            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);


            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Forward);
        }




        //Top Face
        if (top)
        {
            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Up);

            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Up);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Up);


            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Up);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Up);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Up);
        }


        //Bottom Face
        if (bottom)
        {
            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Down);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Down);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Down);



            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Down);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Down);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Down);
        }


        //Left Face
        if (left)
        {
            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Left);

            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Left);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Left);


            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Left);

            verticies.Add((Vector3.Up + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Left);

            verticies.Add((Vector3.Down + Vector3.Left + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Left);
        }

        //Right Face
        if (right)
        {
            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Right);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Right);

            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Right);


            verticies.Add((Vector3.Down + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Right);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Back) / 2 + position);
            normales.Add(Vector3.Right);

            verticies.Add((Vector3.Up + Vector3.Right + Vector3.Forward) / 2 + position);
            normales.Add(Vector3.Right);
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

}
