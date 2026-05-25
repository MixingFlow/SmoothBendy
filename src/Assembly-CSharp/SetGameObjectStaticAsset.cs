using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class SetGameObjectStaticAsset : ScriptableObject
{
	[Serializable]
	public class NameToTag
	{
		[Tooltip("String that must exist in the prefab's name")]
		public string name;

		[Tooltip("Tag that will be used if the above exists in the prefab's name")]
		public string tagName;
	}

	[HideInInspector]
	public bool isStatic;

	public List<NameToTag> tagsToSet = new List<NameToTag>();

	[FormerlySerializedAs("itemsToMakeStatic")]
	public List<GameObject> prefabsToMakeStatic;

	[Header("Debugging")]
	[Tooltip("This list is auto-generated, please do not manually edit.")]
	public List<GameObject> individualItemsToMakeStatic = new List<GameObject>();

	public List<GameObject> oldInvididualItemsToMakeStatic = new List<GameObject>();

	private void OnValidate()
	{
	}

	public void SetStatic()
	{
		SetStaticValue(val: true);
	}

	public void SetNonStatic()
	{
		SetStaticValue(val: false);
	}

	protected void SetStaticValue(bool val)
	{
		isStatic = val;
		for (int num = prefabsToMakeStatic.Count - 1; num >= 0; num--)
		{
			if ((Object)(object)prefabsToMakeStatic[num] == (Object)null)
			{
				prefabsToMakeStatic.RemoveAt(num);
			}
		}
		individualItemsToMakeStatic.Clear();
		foreach (GameObject item in prefabsToMakeStatic)
		{
			CheckForStaticValiditiy(item);
		}
		for (int i = 0; i < individualItemsToMakeStatic.Count; i++)
		{
			GameObject val2 = individualItemsToMakeStatic[i];
			if (!Object.op_Implicit((Object)(object)val2))
			{
				continue;
			}
			if (val)
			{
				if (val2.isStatic)
				{
					continue;
				}
				MeshCollider component = val2.GetComponent<MeshCollider>();
				if (Object.op_Implicit((Object)(object)component) && !Object.op_Implicit((Object)(object)component.sharedMesh))
				{
					MeshFilter component2 = val2.GetComponent<MeshFilter>();
					if (!Object.op_Implicit((Object)(object)component2.sharedMesh))
					{
						Debug.LogWarning((object)("Missing mesh from Mesh Filter on prefab " + ((Object)val2).name));
					}
					else
					{
						component.sharedMesh = component2.sharedMesh;
					}
				}
			}
			val2.isStatic = val;
		}
	}

	public void ClearAllTags()
	{
		foreach (GameObject item in prefabsToMakeStatic)
		{
			ClearTags(item, recurse: true);
		}
	}

	public void AssignAllTags()
	{
		foreach (GameObject item in prefabsToMakeStatic)
		{
			AssignTags(item, recurse: true);
		}
	}

	private void ClearTags(GameObject go, bool recurse = false)
	{
		if (recurse)
		{
			Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(true);
			Transform[] array = componentsInChildren;
			foreach (Transform val in array)
			{
				((Component)val).gameObject.tag = string.Empty;
			}
		}
		else
		{
			go.tag = string.Empty;
		}
	}

	private void AssignTags(GameObject go, bool recurse = false)
	{
		foreach (NameToTag item in tagsToSet)
		{
			if (!((Object)go).name.ToLower().Contains(item.name.ToLower()))
			{
				continue;
			}
			Collider componentInChildren = go.GetComponentInChildren<Collider>();
			if (!Object.op_Implicit((Object)(object)componentInChildren))
			{
				continue;
			}
			if (recurse)
			{
				Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(true);
				Transform[] array = componentsInChildren;
				foreach (Transform val in array)
				{
					((Component)val).gameObject.tag = item.tagName;
				}
			}
			else
			{
				go.tag = item.tagName;
			}
		}
	}

	private void CheckForStaticValiditiy(GameObject go)
	{
		Animator component = go.GetComponent<Animator>();
		if (Object.op_Implicit((Object)(object)component))
		{
			return;
		}
		SwayProp component2 = go.GetComponent<SwayProp>();
		if (Object.op_Implicit((Object)(object)component2))
		{
			return;
		}
		CustomRotator component3 = go.GetComponent<CustomRotator>();
		if (Object.op_Implicit((Object)(object)component3))
		{
			return;
		}
		Light component4 = go.GetComponent<Light>();
		if (Object.op_Implicit((Object)(object)component4))
		{
			return;
		}
		LightController component5 = go.GetComponent<LightController>();
		if (Object.op_Implicit((Object)(object)component5))
		{
			return;
		}
		InteractableTrunk component6 = go.GetComponent<InteractableTrunk>();
		if (Object.op_Implicit((Object)(object)component6))
		{
			return;
		}
		NotStaticMarker component7 = go.GetComponent<NotStaticMarker>();
		if (Object.op_Implicit((Object)(object)component7))
		{
			return;
		}
		CH4SafeDoors component8 = go.GetComponent<CH4SafeDoors>();
		if (Object.op_Implicit((Object)(object)component8))
		{
			return;
		}
		Interactable component9 = go.GetComponent<Interactable>();
		if (!Object.op_Implicit((Object)(object)component9))
		{
			Renderer component10 = go.GetComponent<Renderer>();
			if (Object.op_Implicit((Object)(object)component10) && !individualItemsToMakeStatic.Contains(go))
			{
				individualItemsToMakeStatic.Add(go);
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				CheckForStaticValiditiy(((Component)go.transform.GetChild(i)).gameObject);
			}
		}
	}
}
