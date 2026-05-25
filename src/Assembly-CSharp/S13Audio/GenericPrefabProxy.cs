using UnityEngine;

namespace S13Audio;

public class GenericPrefabProxy : MonoBehaviour
{
	public GameObject prefab;

	private void Awake()
	{
		if (!((Object)(object)prefab == (Object)null))
		{
			GameObject val = Object.Instantiate<GameObject>(prefab, ((Component)this).transform);
			((Object)val.gameObject).name = ((Object)prefab).name;
			Object.Destroy((Object)(object)this);
		}
	}

	private void OnDestroy()
	{
		prefab = null;
	}
}
