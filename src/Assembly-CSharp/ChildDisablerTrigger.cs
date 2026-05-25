using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChildDisablerTrigger : MonoBehaviour
{
	private List<GameObject> m_ChildObjects = new List<GameObject>();

	private void Start()
	{
		GetChild(((Component)this).gameObject);
	}

	private void GetChild(GameObject go)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		foreach (Transform item in go.transform)
		{
			Transform val = item;
			m_ChildObjects.Add(((Component)val).gameObject);
			GetChild(((Component)val).gameObject);
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		foreach (GameObject childObject in m_ChildObjects)
		{
			childObject.SetActive(false);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		foreach (GameObject childObject in m_ChildObjects)
		{
			childObject.SetActive(true);
		}
	}
}
