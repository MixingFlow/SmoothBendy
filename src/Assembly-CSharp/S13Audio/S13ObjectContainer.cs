using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public class S13ObjectContainer : MonoBehaviour
{
	public List<GameObject> objectList = new List<GameObject>();

	private Dictionary<string, GameObject> objectDictionary = new Dictionary<string, GameObject>();

	private void Awake()
	{
		foreach (GameObject @object in objectList)
		{
			if (!((object)@object).Equals((object)null))
			{
				objectDictionary.Add(((Object)@object).name, @object);
			}
		}
		S13AudioManager.Instance.audioEvents.oc = this;
	}

	public void Play(string objectName)
	{
		GameObject val = GetObject(objectName);
		if (!((Object)(object)val == (Object)null))
		{
			S13AudioSource[] componentsInChildren = val.GetComponentsInChildren<S13AudioSource>();
			foreach (S13AudioSource s13AudioSource in componentsInChildren)
			{
				s13AudioSource.Play();
			}
		}
	}

	public void Stop(string objectName)
	{
		GameObject val = GetObject(objectName);
		if (!((Object)(object)val == (Object)null))
		{
			S13AudioSource[] componentsInChildren = val.GetComponentsInChildren<S13AudioSource>();
			foreach (S13AudioSource s13AudioSource in componentsInChildren)
			{
				s13AudioSource.Stop();
			}
		}
	}

	public void Enable(string objectName)
	{
		GameObject val = GetObject(objectName);
		if (!((Object)(object)val == (Object)null) && !val.activeInHierarchy)
		{
			val.SetActive(true);
		}
	}

	public void Disable(string objectName)
	{
		GameObject val = GetObject(objectName);
		if (!((Object)(object)val == (Object)null) && val.activeInHierarchy)
		{
			val.SetActive(false);
		}
	}

	public void Destroy(string objectName, bool ignoreFades = true)
	{
		GameObject val = GetObject(objectName);
		if (!((Object)(object)val == (Object)null))
		{
			S13AudioSource[] componentsInChildren = val.GetComponentsInChildren<S13AudioSource>();
			foreach (S13AudioSource s13AudioSource in componentsInChildren)
			{
				s13AudioSource.Stop(ignoreFades);
				S13AudioManager.Instance.PruneIDFromSoundBank(((Object)s13AudioSource).name);
			}
			objectDictionary.Remove(objectName);
			objectList.Remove(val);
			Object.Destroy((Object)(object)val);
		}
	}

	private GameObject GetObject(string objectName)
	{
		if (objectDictionary.ContainsKey(objectName))
		{
			return objectDictionary[objectName];
		}
		return null;
	}
}
