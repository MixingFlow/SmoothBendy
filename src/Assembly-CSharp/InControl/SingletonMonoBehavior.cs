using System;
using System.Linq;
using UnityEngine;

namespace InControl;

public abstract class SingletonMonoBehavior<TComponent> : MonoBehaviour where TComponent : MonoBehaviour
{
	private static TComponent instance;

	private static bool hasInstance;

	private static int instanceId;

	private static readonly object lockObject = new object();

	public static TComponent Instance
	{
		get
		{
			lock (lockObject)
			{
				if (hasInstance)
				{
					return instance;
				}
				instance = FindFirstInstance();
				if ((Object)(object)instance == (Object)null)
				{
					throw new Exception(string.Concat("The instance of singleton component ", typeof(TComponent), " was requested, but it doesn't appear to exist in the scene."));
				}
				hasInstance = true;
				instanceId = ((Object)instance/*cast due to constrained. prefix*/).GetInstanceID();
				return instance;
			}
		}
	}

	protected bool EnforceSingleton
	{
		get
		{
			if (((Object)this).GetInstanceID() == ((Object)Instance/*cast due to constrained. prefix*/).GetInstanceID())
			{
				return false;
			}
			if (Application.isPlaying)
			{
				((Behaviour)this).enabled = false;
			}
			return true;
		}
	}

	protected bool IsTheSingleton
	{
		get
		{
			lock (lockObject)
			{
				return ((Object)this).GetInstanceID() == instanceId;
			}
		}
	}

	protected bool IsNotTheSingleton
	{
		get
		{
			lock (lockObject)
			{
				return ((Object)this).GetInstanceID() != instanceId;
			}
		}
	}

	private static TComponent[] FindInstances()
	{
		TComponent[] array = Object.FindObjectsOfType<TComponent>();
		Array.Sort(array, (TComponent a, TComponent b) => ((Component)a).transform.GetSiblingIndex().CompareTo(((Component)b).transform.GetSiblingIndex()));
		return array;
	}

	private static TComponent FindFirstInstance()
	{
		TComponent[] array = FindInstances();
		return (array.Length <= 0) ? ((TComponent)(object)null) : array[0];
	}

	protected virtual void Awake()
	{
		if (!Application.isPlaying || !Object.op_Implicit((Object)(object)Instance))
		{
			return;
		}
		if (((Object)this).GetInstanceID() != instanceId)
		{
			((Behaviour)this).enabled = false;
		}
		foreach (TComponent item in from o in FindInstances()
			where ((Object)o/*cast due to constrained. prefix*/).GetInstanceID() != instanceId
			select o)
		{
			TComponent current = item;
			((Behaviour)current).enabled = false;
		}
	}

	protected virtual void OnDestroy()
	{
		lock (lockObject)
		{
			if (((Object)this).GetInstanceID() == instanceId)
			{
				hasInstance = false;
			}
		}
	}
}
