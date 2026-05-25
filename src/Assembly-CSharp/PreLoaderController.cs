using System.Collections;
using System.Collections.Generic;
using TMG.AssetBundles;
using TMG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreLoaderController : BaseUIController
{
	[Header("RectTransforms")]
	[SerializeField]
	private RectTransform m_Visuals;

	[Header("Images")]
	[SerializeField]
	private List<Image> m_BackgroundImages;

	[SerializeField]
	private Image m_Blocker;

	[SerializeField]
	private Image m_Loader;

	[SerializeField]
	private Image m_LoadingBar;

	[SerializeField]
	private Sprite[] m_LoaderSprites;

	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI m_LoadingMessageLbl;

	private AsyncOperation m_Async;

	private int m_CurrentLoadingIndex;

	private int m_MaxLoadingIndex = 3;

	public override void InitController(object _data)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		((Transform)m_Visuals).localScale = new Vector3(1f, 1f, 1f);
		m_LoadingBar.fillAmount = 0f;
		GameManager.Instance.HideScreenBlocker();
	}

	public override void PlayInComplete()
	{
		GameManager.Instance.AssetBundleManager = new AssetBundleManager();
		((MonoBehaviour)this).StartCoroutine(RunPreLoader());
	}

	private void ProgressUpdate()
	{
		m_CurrentLoadingIndex++;
		m_Loader.sprite = m_LoaderSprites[m_CurrentLoadingIndex];
		m_LoadingBar.fillAmount = (float)m_CurrentLoadingIndex / (float)m_MaxLoadingIndex;
	}

	private IEnumerator RunPreLoader()
	{
		yield return CheckData("Loading...", null);
		yield return (object)new WaitForSeconds(0.5f);
		yield return (object)new WaitForEndOfFrame();
		yield return CheckData("Checking Save Data...", LoadSaveData());
		yield return (object)new WaitForSeconds(0.5f);
		yield return (object)new WaitForEndOfFrame();
		yield return CheckData("Refilling Ink Machine...", null);
		m_Loader.sprite = m_LoaderSprites[m_LoaderSprites.Length - 1];
		yield return (object)new WaitForSeconds(1f);
		yield return (object)new WaitForEndOfFrame();
		PlayOut();
	}

	public override void PlayOut()
	{
		GameManager.Instance.ShowScreenBlocker(0.5f, 0f, PlayOutComplete);
	}

	private IEnumerator CheckData(string label, IEnumerator coroutine)
	{
		m_LoadingMessageLbl.text = label;
		yield return (object)new WaitForEndOfFrame();
		if (coroutine != null)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(coroutine);
		}
		yield return (object)new WaitForEndOfFrame();
		ProgressUpdate();
	}

	private IEnumerator LoadSaveData()
	{
		GameManager.Instance.GameDataManager = new GameDataManager();
		GameManager.Instance.GameDataManager.Load();
		yield return null;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
