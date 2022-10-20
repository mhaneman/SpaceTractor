using Godot;
using System;
using System.Collections.Generic;

public class ObjectStack<T> : Spatial where T : Spatial
{
	private Stack<T> retired = new Stack<T>();
    private Stack<T> working = new Stack<T>();
    private string scenePath;
    private int initCount;
    
    public ObjectStack(string scenePath, int initCount=1)
    {
        this.scenePath = scenePath;
        this.initCount = initCount;
    }

    public override void _Ready()
    {
        defferedInstance(initCount);
    }

    private void defferedInstance(int count=1)
    {
        for (int i=0; i<count; i++)
        {
            PackedScene scene = (PackedScene)ResourceLoader.Load(scenePath);
            T t = scene.Instance<T>();
            this.CallDeferred("add_child", t);
            t.Visible = false;
            retired.Push(t);
        }
    }

    private void RunningInstance(int count=1)
	{
		for (int i=0; i<count; i++)
		{
			PackedScene scene = (PackedScene)ResourceLoader.Load(scenePath);
			T t = scene.Instance<T>();
			this.AddChild(t);
			t.Visible = false;
			retired.Push(t);
		}

	}

    private T PopRetired() 
	{
		T t;
		if (retired.Count <= working.Count)
			RunningInstance(working.Count / 2);
		t = retired.Pop();
		t.Visible = true;
		return t;
	}

    public T Summon(Transform transform) {
		T t = PopRetired();
		t.GlobalTransform = transform;
		working.Push(t);

		return t;
	}
	
	public T Summon(Transform transform, float rotation)
	{
		T t = PopRetired();
		t.GlobalTransform = transform;
		t.RotateY(rotation);
		working.Push(t);

		return t;
	}

	public T Summon(Transform transform, float rotation, Vector3 scale)
	{
		T t = PopRetired();
		t.GlobalTransform = transform;
		t.RotateY(rotation);
		t.Scale = scale;
		working.Push(t);

		return t;
	}

    // borrow a retired platform to get global transforms of all connectors
    public List<(string, Transform)> getConnectors(string childName, Transform transform, float rotation = 0f)
	{
		var children = new List<(string, Transform)>();
		T t = retired.Peek();
		t.GlobalTransform = transform;
		t.RotateY(rotation);

		Spatial connector = t.GetNode<Spatial>(childName);
		foreach(Spatial n in connector.GetChildren())
			children.Add((n.Name, n.GlobalTransform));

		
		return children;
	}

	// borrow a retired platform to get global transform of a specific connector
	public Vector3 getConnector(string childName, Transform transform, float rotation = 0f)
	{
		T t = retired.Peek();
		t.GlobalTransform = transform;
		t.RotateY(rotation);

		Vector3 connector = t.GetNode<Spatial>(childName).GlobalTransform.origin;
		return connector;
	}

    public void Dismiss() 
	{
		if (working.Count == 0)
			return;
		
		T t = working.Pop();
		t.Visible = false;
		retired.Push(t);
	}
	
	public void Clear()
	{
		
		while(working.Count > 0)
			this.Dismiss();
			
		/* GD.Print("working: ", working.Count, " retired: ", 
			retired.Count, " ", ScenePath); */
	}

}
