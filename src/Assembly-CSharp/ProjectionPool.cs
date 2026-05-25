using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionPool
{
	private static ProjectionPool defaultPool;

	private PoolInstance instance;

	private Transform parent;

	private List<PoolItem> activePool;

	private List<PoolItem> inactivePool;

	public static ProjectionPool Default
	{
		get
		{
			if (defaultPool == null)
			{
				defaultPool = DynamicDecals.System.PoolFromInstance(DynamicDecals.System.Settings.pools[0]);
			}
			return defaultPool;
		}
	}

	public string Title => instance.title;

	public int ID => instance.id;

	private int Limit => instance.limits[QualitySettings.GetQualityLevel()];

	internal Transform Parent
	{
		get
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)parent == (Object)null)
			{
				parent = new GameObject(instance.title + " Pool").transform;
			}
			return parent;
		}
	}

	public ProjectionPool(PoolInstance Instance)
	{
		instance = Instance;
	}

	public static ProjectionPool GetPool(string Title)
	{
		return DynamicDecals.System.GetPool(Title);
	}

	public static ProjectionPool GetPool(int ID)
	{
		return DynamicDecals.System.GetPool(ID);
	}

	internal void Update(float DeltaTime)
	{
		if (activePool == null || activePool.Count <= 0)
		{
			return;
		}
		for (int num = activePool.Count - 1; num >= 0; num--)
		{
			if ((Object)(object)activePool[num].GameObject == (Object)null)
			{
				activePool.RemoveAt(num);
			}
			else
			{
				activePool[num].Update(DeltaTime);
			}
		}
	}

	public bool CheckIntersecting(Vector3 Point, float intersectionStrength)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (activePool != null && activePool.Count > 0)
		{
			for (int i = 0; i < activePool.Count; i++)
			{
				if (activePool[i].Projection.CheckIntersecting(Point) > intersectionStrength)
				{
					return true;
				}
			}
		}
		return false;
	}

	public Projection Request(ProjectionType Type)
	{
		if (activePool == null)
		{
			activePool = new List<PoolItem>();
		}
		if (inactivePool != null && inactivePool.Count > 0)
		{
			PoolItem poolItem = inactivePool[0];
			inactivePool.RemoveAt(0);
			activePool.Add(poolItem);
			poolItem.Reset(Type);
			return poolItem.Projection;
		}
		if (activePool.Count < Limit)
		{
			PoolItem poolItem2 = new PoolItem(this);
			poolItem2.Reset(Type);
			activePool.Add(poolItem2);
			return poolItem2.Projection;
		}
		PoolItem poolItem3 = activePool[0];
		activePool.RemoveAt(0);
		activePool.Add(poolItem3);
		poolItem3.Reset(Type);
		return poolItem3.Projection;
	}

	public Projection RequestCopy(Projection Projection)
	{
		if ((Object)(object)Projection == (Object)null)
		{
			return null;
		}
		if (((object)Projection).GetType() == typeof(Decal))
		{
			Decal decal = (Decal)Request(ProjectionType.Decal);
			decal.CopyAllProperties((Decal)Projection);
			return decal;
		}
		if (((object)Projection).GetType() == typeof(Eraser))
		{
			Eraser eraser = (Eraser)Request(ProjectionType.Eraser);
			eraser.CopyAllProperties((Eraser)Projection);
			return eraser;
		}
		if (((object)Projection).GetType() == typeof(OmniDecal))
		{
			OmniDecal omniDecal = (OmniDecal)Request(ProjectionType.OmniDecal);
			omniDecal.CopyAllProperties((OmniDecal)Projection);
			return omniDecal;
		}
		throw new NotImplementedException("Projection Type not recognized, If your implementing your own projection types, you need to implement a copy method like the projection types above");
	}

	public void Return(Projection Projection)
	{
		if (Projection.PoolItem != null)
		{
			Return(Projection.PoolItem);
		}
	}

	internal void Return(PoolItem Item)
	{
		if (inactivePool == null)
		{
			inactivePool = new List<PoolItem>();
		}
		activePool.Remove(Item);
		if ((Object)(object)Item.GameObject != (Object)null)
		{
			Item.GameObject.SetActive(false);
		}
		inactivePool.Add(Item);
	}
}
