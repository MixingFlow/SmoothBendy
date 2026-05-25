using System.Collections;
using UnityEngine;

namespace I2.Loc;

public class CoroutineManager : MonoBehaviour
{
	private static CoroutineManager mInstance;

	private static CoroutineManager pInstance
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			if ((Object)(object)mInstance == (Object)null)
			{
				GameObject val = new GameObject("_Coroutiner");
				((Object)val).hideFlags = (HideFlags)61;
				mInstance = val.AddComponent<CoroutineManager>();
				if (Application.isPlaying)
				{
					Object.DontDestroyOnLoad((Object)(object)val);
				}
			}
			return mInstance;
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
	}

	public static Coroutine Start(IEnumerator coroutine)
	{
		return ((MonoBehaviour)pInstance).StartCoroutine(coroutine);
	}
}
