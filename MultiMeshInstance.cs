using Godot;
using System;

[Tool]
public class MultiMeshInstance : Godot.MultiMeshInstance
{
    [Export]
    private int _size = 1;

    public override void _Ready()
    {
        Initialize();
    }

    private void Initialize()
    {

        for(int i = 0; i < Multimesh.InstanceCount; i++)
        {
            Transform position = new Transform();
            position = position.Translated(new Vector3( i, 0, 0));
            Multimesh.SetInstanceTransform(i, position);
        }

        //int count = 0;

       /* for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                for (int z = 0; z < _size; z++)
                {
                    Transform t = new Transform();
                    t = t.Translated(new Vector3(x, y, z));
                    Multimesh.SetInstanceTransform(count, t);

                    count++;
                }
            }
        }*/
    }

   public override void _Process(float delta)
    {
       /* if(Engine.EditorHint)
        {
            Initialize();
        }*/
    }

}
