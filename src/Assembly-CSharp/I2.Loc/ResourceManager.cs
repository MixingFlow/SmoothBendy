using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace I2.Loc;

public class ResourceManager : MonoBehaviour
{
	private static ResourceManager mInstance;

	public List<IResourceManager_Bundles> mBundleManagers = new List<IResourceManager_Bundles>();

	public Object[] Assets;

	private readonly Dictionary<string, Object> mResourcesCache = new Dictionary<string, Object>(StringComparer.Ordinal);

	public static ResourceManager pInstance
	{
		get
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (Object)(object)mInstance == (Object)null;
			if ((Object)(object)mInstance == (Object)null)
			{
				mInstance = (ResourceManager)(object)Object.FindObjectOfType(typeof(ResourceManager));
			}
			if ((Object)(object)mInstance == (Object)null)
			{
				GameObject val = new GameObject("I2ResourceManager", new Type[1] { typeof(ResourceManager) });
				((Object)val).hideFlags = (HideFlags)(((Object)val).hideFlags | 0x3D);
				mInstance = val.GetComponent<ResourceManager>();
				SceneManager.sceneLoaded += MyOnLevelWasLoaded;
			}
			if (flag && Application.isPlaying)
			{
				Object.DontDestroyOnLoad((Object)(object)((Component)mInstance).gameObject);
			}
			return mInstance;
		}
	}

	public static void MyOnLevelWasLoaded(Scene scene, LoadSceneMode mode)
	{
		pInstance.CleanResourceCache();
		LocalizationManager.UpdateSources();
	}

	public T GetAsset<T>(string Name) where T : Object
	{
		Object obj = FindAsset(Name);
		T val = (T)(object)((obj is T) ? obj : null);
		if ((Object)(object)val != (Object)null)
		{
			return val;
		}
		return LoadFromResources<T>(Name);
	}

	private Object FindAsset(string Name)
	{
		if (Assets != null)
		{
			int i = 0;
			for (int num = Assets.Length; i < num; i++)
			{
				if (Assets[i] != (Object)null && Assets[i].name == Name)
				{
					return Assets[i];
				}
			}
		}
		return null;
	}

	public bool HasAsset(Object Obj)
	{
		if (Assets == null)
		{
			return false;
		}
		return Array.IndexOf(Assets, Obj) >= 0;
	}

	public T LoadFromResources<T>(string Path) where T : Object
	{
		try
		{
			if (string.IsNullOrEmpty(Path))
			{
				return (T)(object)null;
			}
			if (mResourcesCache.TryGetValue(Path, out var value) && value != (Object)null)
			{
				return (T)(object)((value is T) ? value : null);
			}
			T val = (T)(object)null;
			if (Path.EndsWith("]", StringComparison.OrdinalIgnoreCase))
			{
				int num = Path.LastIndexOf("[", StringComparison.OrdinalIgnoreCase);
				int length = Path.Length - num - 2;
				string value2 = Path.Substring(num + 1, length);
				Path = Path.Substring(0, num);
				T[] array = Resources.LoadAll<T>(Path);
				int i = 0;
				for (int num2 = array.Length; i < num2; i++)
				{
					if (((Object)array[i]).name.Equals(value2))
					{
						val = array[i];
						break;
					}
				}
			}
			else
			{
				Object obj = Resources.Load(Path, typeof(T));
				val = (T)(object)((obj is T) ? obj : null);
			}
			if ((Object)(object)val == (Object)null)
			{
				val = LoadFromBundle<T>(Path);
			}
			if ((Object)(object)val != (Object)null)
			{
				mResourcesCache[Path] = (Object)(object)val;
			}
			return val;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Unable to load {0} '{1}'\nERROR: {2}", new object[3]
			{
				typeof(T),
				Path,
				ex.ToString()
			});
			return (T)(object)null;
		}
	}

	public T LoadFromBundle<T>(string path) where T : Object
	{
		int i = 0;
		for (int count = mBundleManagers.Count; i < count; i++)
		{
			if (mBundleManagers[i] != null)
			{
				Object obj = mBundleManagers[i].LoadFromBundle(path, typeof(T));
				T val = (T)(object)((obj is T) ? obj : null);
				if ((Object)(object)val != (Object)null)
				{
					return val;
				}
			}
		}
		return (T)(object)null;
	}

	public void CleanResourceCache()
	{
		mResourcesCache.Clear();
		Resources.UnloadUnusedAssets();
		((MonoBehaviour)this).CancelInvoke();
	}
}
