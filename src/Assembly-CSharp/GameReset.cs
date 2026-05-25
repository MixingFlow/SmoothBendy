using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core;
using S13Audio;
using TMG.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReset : TMGMonoBehaviour
{
	private List<GameObject> m_GameObjects = new List<GameObject>();

	public override void Init()
	{
		base.Init();
		m_GameObjects = Object.FindObjectsOfType<GameObject>().ToList();
		m_GameObjects.Remove(base.gameObject);
		foreach (GameObject gameObject in m_GameObjects)
		{
			if (!Object.op_Implicit((Object)(object)gameObject.GetComponent<DOTweenComponent>()) && !Object.op_Implicit((Object)(object)gameObject.GetComponent<SteamManager>()) && !Object.op_Implicit((Object)(object)gameObject.GetComponent<SeasonalController>()) && !Object.op_Implicit((Object)(object)gameObject.GetComponent<EOSController>()))
			{
				Object.Destroy((Object)(object)gameObject);
			}
		}
		S13AudioManager.Instance.UnloadAllSoundBanks();
		GameManager.Instance.Dispose();
		GameManager.Instance.isGameLoaded = true;
	}

	public override void InitOnComplete()
	{
		((MonoBehaviour)this).StartCoroutine(GoToMainMenu());
	}

	private IEnumerator GoToMainMenu()
	{
		AsyncOperation async = SceneManager.LoadSceneAsync("InitializeGame");
		async.allowSceneActivation = false;
		while (async.progress < 0.9f)
		{
			yield return (object)new WaitForEndOfFrame();
		}
		async.allowSceneActivation = true;
	}

	protected override void OnDisposed()
	{
		if (m_GameObjects != null)
		{
			m_GameObjects.Clear();
			m_GameObjects = null;
		}
		base.OnDisposed();
	}
}
