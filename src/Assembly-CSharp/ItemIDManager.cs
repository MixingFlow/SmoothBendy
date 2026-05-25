using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemIDManager : MonoBehaviour
{
	[Serializable]
	public class ItemIdData
	{
		[Tooltip("Just a helper to remember what this set of items is all about.")]
		public string name;

		[Tooltip("Leave empty if you want to manually add items to the Scene Objects list. Note that that is some black magic you are working with...")]
		public List<GameObject> prefabsToStore = new List<GameObject>();

		[Header("Do Not Touch Under Pain Of Death. I'm Not Joking.")]
		public List<GameObject> sceneObjects = new List<GameObject>();

		public int GetIndex(GameObject go)
		{
			return sceneObjects.IndexOf(go);
		}
	}

	[SerializeField]
	private List<ItemIdData> itemIds = new List<ItemIdData>();

	public int GetItemId(GameObject go)
	{
		int num = -1;
		foreach (ItemIdData itemId in itemIds)
		{
			num = Mathf.Max(num, itemId.GetIndex(go));
		}
		return num;
	}
}
