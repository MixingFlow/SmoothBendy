using System;
using System.Collections;
using DG.Tweening;
using TMG.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenericLoaderController : BaseUIController
{
	[Header("RectTransforms")]
	[SerializeField]
	private RectTransform m_Visuals;

	[Header("Images")]
	[SerializeField]
	private Image m_Blocker;

	[SerializeField]
	private Image m_Loader;

	[SerializeField]
	private Image m_ChapterImage;

	[SerializeField]
	private Sprite[] m_ChapterImages;

	private GenericLoaderDataVO m_DataVO;

	private AsyncOperation m_Async;

	private string m_SceneName;

	public event EventHandler OnLoaded;

	public override void InitController(object _data)
	{
		base.InitController(_data);
		m_DataVO = (GenericLoaderDataVO)_data;
		m_Loader.DOFade(0.2f, 0f);
		((Behaviour)m_Blocker).enabled = false;
		((Behaviour)m_Loader).enabled = false;
		((Behaviour)m_ChapterImage).enabled = false;
		((Component)m_Visuals).gameObject.SetActive(false);
	}

	public void LoadScene(GenericLoaderDataVO data)
	{
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		m_DataVO = data;
		m_SceneName = m_DataVO.SceneName;
		if (m_DataVO.SceneName == "CH1")
		{
			m_ChapterImage.sprite = m_ChapterImages[0];
		}
		else if (m_DataVO.SceneName == "CH2")
		{
			m_ChapterImage.sprite = m_ChapterImages[1];
		}
		else if (m_DataVO.SceneName == "CH3")
		{
			m_ChapterImage.sprite = m_ChapterImages[2];
		}
		else if (m_DataVO.SceneName == "CH4")
		{
			m_ChapterImage.sprite = m_ChapterImages[3];
		}
		else if (m_DataVO.SceneName == "CH5")
		{
			m_ChapterImage.sprite = m_ChapterImages[4];
		}
		Color color = ((Graphic)m_ChapterImage).color;
		color.a = 0f;
		((Graphic)m_ChapterImage).color = color;
		((Behaviour)m_ChapterImage).enabled = true;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(m_ChapterImage.DOFade(1f, 0.5f), 0.5f), (TweenCallback)delegate
		{
			((MonoBehaviour)this).StartCoroutine(LoadAsync());
		});
	}

	private IEnumerator LoadAsync()
	{
		m_Async = SceneManager.LoadSceneAsync(m_SceneName);
		m_Async.allowSceneActivation = false;
		((Behaviour)m_Blocker).enabled = true;
		((Behaviour)m_Loader).enabled = true;
		((Component)m_Visuals).gameObject.SetActive(true);
		TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(m_Loader.DOFade(0.4f, 1.5f), -1, (LoopType)1), (Ease)7), (UpdateType)0);
		while (m_Async.progress < 0.9f)
		{
			yield return (object)new WaitForSeconds(0.05f);
			yield return (object)new WaitForEndOfFrame();
		}
		GameManager.Instance.ShowScreenBlocker(0.4f);
		ShortcutExtensions.DOKill((Component)(object)m_Loader, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_Loader.DOFade(0f, 0.5f), new TweenCallback(LaunchLoadedScene));
	}

	public void HideLoader()
	{
		m_Async = null;
		ShortcutExtensions.DOKill((Component)(object)m_Loader, false);
		m_Loader.DOFade(0f, 0f);
		((Behaviour)m_Blocker).enabled = false;
		((Behaviour)m_Loader).enabled = false;
		((Component)m_Visuals).gameObject.SetActive(false);
		Kill();
	}

	private void LaunchLoadedScene()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(m_ChapterImage.DOFade(0f, 1f), 1f), new TweenCallback(ActualLaunchLoadedScene));
	}

	private void ActualLaunchLoadedScene()
	{
		m_Async.allowSceneActivation = true;
		this.OnLoaded.Send(this);
	}

	protected override void OnDisposed()
	{
		m_Async = null;
		m_DataVO = null;
		base.OnDisposed();
	}
}
