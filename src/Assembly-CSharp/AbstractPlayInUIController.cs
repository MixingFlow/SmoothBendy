using DG.Tweening;
using TMG.UI;
using UnityEngine;

public abstract class AbstractPlayInUIController : BaseUIController
{
	protected enum PlayInType
	{
		SCALE,
		TOP_BOTTOM,
		BOTTOM_TOP,
		FADE,
		NONE
	}

	protected enum AnchorSizeType
	{
		VISUALS,
		CONTENT
	}

	[Header("PlayIn Options")]
	[SerializeField]
	protected PlayInType m_PlayInType;

	[SerializeField]
	protected AnchorSizeType m_AnchorSizeType;

	[Header("Visuals")]
	[SerializeField]
	protected RectTransform m_Visuals;

	protected RectTransform m_Content;

	protected Vector2 m_VisualsOriginPosition;

	protected Vector2 m_VisualsStartPosition;

	protected Vector3 m_VisualsOriginScale;

	protected Vector3 m_VisualsStartScale;

	private CanvasGroup m_CanvasGroup;

	public override void InitController(object _data)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_Content = ((Component)this).GetComponent<RectTransform>();
		m_VisualsOriginScale = ((Transform)m_Visuals).localScale;
		m_VisualsStartScale = ((Transform)m_Visuals).localScale;
		m_VisualsOriginPosition = m_Visuals.anchoredPosition;
		m_VisualsStartPosition = m_Visuals.anchoredPosition;
		if (m_PlayInType == PlayInType.NONE)
		{
			return;
		}
		Vector2 zero = Vector2.zero;
		if (m_AnchorSizeType == AnchorSizeType.CONTENT)
		{
			((Vector2)(ref zero))._002Ector(0f, m_Content.sizeDelta.y);
		}
		else if (m_AnchorSizeType == AnchorSizeType.VISUALS)
		{
			((Vector2)(ref zero))._002Ector(0f, m_Visuals.sizeDelta.y);
		}
		if (m_PlayInType == PlayInType.SCALE)
		{
			m_VisualsStartScale = Vector2.op_Implicit(Vector2.zero);
		}
		else if (m_PlayInType == PlayInType.TOP_BOTTOM)
		{
			m_VisualsStartPosition = m_Visuals.anchoredPosition + zero;
		}
		else if (m_PlayInType == PlayInType.BOTTOM_TOP)
		{
			m_VisualsStartPosition = m_Visuals.anchoredPosition - zero;
		}
		else if (m_PlayInType == PlayInType.FADE)
		{
			m_CanvasGroup = ((Component)m_Visuals).GetComponent<CanvasGroup>();
			if ((Object)(object)m_CanvasGroup != (Object)null)
			{
				m_CanvasGroup.alpha = 0f;
			}
		}
		m_Visuals.anchoredPosition = m_VisualsStartPosition;
		((Transform)m_Visuals).localScale = m_VisualsStartScale;
	}

	public override void PlayIn()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		if (m_PlayInType == PlayInType.NONE)
		{
			PlayInComplete();
		}
		else if (m_PlayInType == PlayInType.SCALE)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, m_VisualsOriginScale, 0.5f), (Ease)27), new TweenCallback(PlayInComplete));
		}
		else if (m_PlayInType == PlayInType.FADE)
		{
			if ((Object)(object)m_CanvasGroup != (Object)null)
			{
				TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_CanvasGroup.DOFade(1f, 0.5f), (Ease)1), new TweenCallback(PlayInComplete));
			}
			else
			{
				PlayInComplete();
			}
		}
		else
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_Visuals.DOAnchorPos(m_VisualsOriginPosition, 0.5f), (Ease)27), new TweenCallback(PlayInComplete));
		}
	}

	public override void PlayOut()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		if (m_PlayInType == PlayInType.NONE)
		{
			PlayInComplete();
		}
		else if (m_PlayInType == PlayInType.SCALE)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, m_VisualsStartScale, 0.5f), (Ease)26), new TweenCallback(PlayOutComplete));
		}
		else if (m_PlayInType == PlayInType.FADE)
		{
			if ((Object)(object)m_CanvasGroup != (Object)null)
			{
				TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_CanvasGroup.DOFade(0f, 0.5f), (Ease)1), new TweenCallback(PlayOutComplete));
			}
			else
			{
				PlayOutComplete();
			}
		}
		else
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_Visuals.DOAnchorPos(m_VisualsStartPosition, 0.5f), (Ease)26), new TweenCallback(PlayOutComplete));
		}
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Visuals, false);
		m_Content = null;
		m_CanvasGroup = null;
		base.OnDisposed();
	}
}
