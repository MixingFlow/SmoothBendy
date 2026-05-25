using System.Collections;
using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class PoolingManager : TMGMonoBehaviour
{
	private Dictionary<int, List<GameObject>> m_PooledObjects = new Dictionary<int, List<GameObject>>();

	public GameObject GetFromPool(string prefab)
	{
		int hashCode = prefab.GetHashCode();
		if (!m_PooledObjects.ContainsKey(hashCode))
		{
			m_PooledObjects.Add(hashCode, new List<GameObject>());
		}
		if (m_PooledObjects[hashCode].Count <= 0)
		{
			m_PooledObjects[hashCode].Add(Object.Instantiate<GameObject>(GameManager.Instance.AssetManager.GetAsset<GameObject>(prefab)));
		}
		GameObject val = m_PooledObjects[hashCode][0];
		m_PooledObjects[hashCode].Remove(val);
		val.SetActive(true);
		((MonoBehaviour)this).StartCoroutine(DelayPooling(hashCode, val));
		return val;
	}

	private IEnumerator DelayPooling(int hashID, GameObject go)
	{
		yield return (object)new WaitForSeconds(15f);
		while (GameManager.Instance.isPaused)
		{
			yield return null;
		}
		yield return (object)new WaitForEndOfFrame();
		SendToPool(hashID, go);
	}

	private void SendToPool(int hashID, GameObject go)
	{
		if (Object.op_Implicit((Object)(object)go))
		{
			go.SetActive(false);
			if (!m_PooledObjects.ContainsKey(hashID))
			{
				m_PooledObjects.Add(hashID, new List<GameObject>());
			}
			m_PooledObjects[hashID].Add(go);
			go.transform.SetParent(base.transform);
		}
	}

	protected override void OnDisposed()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		if (m_PooledObjects != null)
		{
			m_PooledObjects.Clear();
			m_PooledObjects = null;
		}
		base.OnDisposed();
	}
}
