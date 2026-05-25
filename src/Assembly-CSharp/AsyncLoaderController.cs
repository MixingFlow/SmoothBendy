using DG.Tweening;
using TMG.UI;
using UnityEngine;
using UnityEngine.UI;

public class AsyncLoaderController : BaseUIController
{
	[Header("RectTransforms")]
	[SerializeField]
	private RectTransform m_Visuals;

	[Header("Images")]
	[SerializeField]
	private Image m_Loader;

	public override void InitController(object _data)
	{
		base.InitController(_data);
		InternalHide();
	}

	public void Show()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)m_Loader, false);
		((Graphic)m_Loader).color = new Color(1f, 1f, 1f, 0.2f);
		((Behaviour)m_Loader).enabled = true;
		((Component)m_Visuals).gameObject.SetActive(true);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_Loader.DOFade(0.4f, 0.5f), (Ease)7), -1, (LoopType)1);
	}

	public void Hide()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)m_Loader, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_Loader.DOFade(0f, 0.75f), (Ease)7), new TweenCallback(InternalHide));
	}

	private void InternalHide()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)m_Loader).color = new Color(1f, 1f, 1f, 0.2f);
		((Behaviour)m_Loader).enabled = false;
		((Component)m_Visuals).gameObject.SetActive(false);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
