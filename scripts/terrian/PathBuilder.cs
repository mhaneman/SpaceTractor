using Godot;
using System.Collections.Generic;
using System;

public class PathBuilder : Spatial
{
	Random rand = new Random();
	private PathSpawner<StaticBody> pathSpawner;
	private LinkedList<Vector3> pathFollow;

	private float[] directions = {-Mathf.Pi/2, 0f, Mathf.Pi/2};
	private int numOfPlatforms;
	
	private float withinReachArea = 20f;
	private float platformSpacing = 15f;
	private float minHeight = -50f;
	private float maxXZDistance = 300f;

	private Vector3 end = new Vector3(0, 200, 0);
	private float maxNumProcess = 10;
	private Vector3 scalePlatformWidth = new Vector3(2f, 1f, 1f);
	private float saltWeight = 0.8f;

	private int numProcess = 0;
	private bool runProcess = true;
	private bool endProcess = false;
	
	
	public override void _Ready()
	{
		pathSpawner = new PathSpawner<StaticBody>(this);
		numOfPlatforms = pathSpawner.PlatformTypes.Count;
	}
	
	private void on_EnteredPortal()
	{
		pathSpawner.Reset();
		endProcess = false;
		numProcess = 0;
		runProcess = true;
	}
	
	public override void _Process(float delta)
	{
		if (!runProcess)
			return;

		initalPlatforms(pathSpawner);
		addDirectionalPlatform();
		checkProcesses();
	}

	private void checkProcesses()
	{
		if (endProcess || numProcess >= maxNumProcess)
		{
			pathSpawner.SummonPortal();
			runProcess = false;
			numProcess += 10;
		} 
		numProcess++;
	}

	private void initalPlatforms(PathSpawner<StaticBody> pathSpawner)
	{
		if (numProcess == 0)
		{
			pathSpawner.SummonPortal();
			pathSpawner.Summon(0, 0f, 8);
		}
	}

	private void addDirectionalPlatform()
	{
		var platforms = new PriorityQueue<(int, float)>(true);
		foreach(var potentialPlatform in pathSpawner.getAllPlatformConnectingGlobalPoints())
		{
			var plat = potentialPlatform.Value;
			
			if (!IsOverlappingOrInBounds(plat))
			{
				setIsWithinReach(plat);
				platforms.Enqueue(ratePlatform(plat), potentialPlatform.Key);
			}
		}
		
		// if all platforms collide with enviroment -> summon portal
		if (platforms.Count == 0)
		{
			endProcess = true;
			return;
		}
		
		(int platformType, float rotation) newPlatform = platforms.Dequeue();
		pathSpawner.Summon(newPlatform.platformType, newPlatform.rotation);
	}

	private int ratePlatform(Vector3 currentPos)
	{
		float distance = currentPos.DistanceTo(end);
		float salt = saltWeight * distance * (float) rand.NextDouble();
		return (int) (distance + salt);
	}

	private void setIsWithinReach(Vector3 position)
	{
		if (position.DistanceTo(end) <= withinReachArea)
			endProcess = true;
	}
	
	private bool IsOverlappingOrInBounds(Vector3 position)
	{
		foreach (StaticBody s in pathSpawner.ActivePlatforms)
		{
			if (position.DistanceTo(s.GlobalTransform.origin) <= platformSpacing &&
				position.y >= minHeight && 
				position.x >= -maxXZDistance && position.x <= maxXZDistance &&
				position.z >= -maxXZDistance && position.z <= maxXZDistance)
				return true;
		}
		return false;
	}
}