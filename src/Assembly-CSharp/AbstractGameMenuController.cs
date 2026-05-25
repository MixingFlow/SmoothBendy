using DG.Tweening;
using TMG.UI;
using UnityEngine;

public abstract class AbstractGameMenuController : BaseUIController
{
	protected enum PlayInType
	{
		SCALE,
		TOP_BOTTOM,
		BOTTOM_TOP
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

	protected float m_PlayInDelay;

	protected float m_PlayoutDelay;

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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_Content = ((Component)this).GetComponent<RectTransform>();
		m_VisualsOriginScale = ((Transform)m_Visuals).localScale;
		m_VisualsStartScale = ((Transform)m_Visuals).localScale;
		m_VisualsOriginPosition = m_Visuals.anchoredPosition;
		m_VisualsStartPosition = m_Visuals.anchoredPosition;
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
		m_Visuals.anchoredPosition = m_VisualsStartPosition;
		((Transform)m_Visuals).localScale = m_VisualsStartScale;
	}

	public override void PlayIn()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		if (m_PlayInType == PlayInType.SCALE)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, m_VisualsOriginScale, 0.5f), (Ease)6), m_PlayInDelay), (UpdateType)0), new TweenCallback(PlayInComplete));
		}
		else
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_Visuals.DOAnchorPos(m_VisualsOriginPosition, 0.5f), (Ease)6), m_PlayInDelay), (UpdateType)0), new TweenCallback(PlayInComplete));
		}
	}

	public override void PlayOut()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		if (m_PlayInType == PlayInType.SCALE)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, m_VisualsStartScale, 0.5f), (Ease)5), m_PlayoutDelay), (UpdateType)0), new TweenCallback(PlayOutComplete));
		}
		else
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_Visuals.DOAnchorPos(m_VisualsStartPosition, 0.5f), (Ease)5), m_PlayoutDelay), (UpdateType)0), new TweenCallback(PlayOutComplete));
		}
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Visuals, false);
		base.OnDisposed();
	}
}
