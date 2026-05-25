using DG.Tweening;
using TMG.UI;
using UnityEngine;

public abstract class AbstractPromptController : BaseUIController
{
	protected enum PlayInType
	{
		SCALE,
		TOP_BOTTOM,
		BOTTOM_TOP
	}

	[Header("PlayIn Options")]
	[SerializeField]
	protected PlayInType m_PlayInType;

	[Header("Visuals")]
	[SerializeField]
	protected RectTransform m_Visuals;

	protected RectTransform m_Content;

	protected Vector2 m_VisualsOriginPosition;

	protected Vector2 m_VisualsStartPosition;

	protected Vector3 m_VisualsOriginScale;

	protected Vector3 m_VisualsStartScale;

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
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_Content = ((Component)this).GetComponent<RectTransform>();
		m_VisualsOriginScale = ((Transform)m_Visuals).localScale;
		m_VisualsStartScale = ((Transform)m_Visuals).localScale;
		m_VisualsOriginPosition = m_Visuals.anchoredPosition;
		m_VisualsStartPosition = m_Visuals.anchoredPosition;
		if (m_PlayInType == PlayInType.SCALE)
		{
			m_VisualsStartScale = Vector2.op_Implicit(Vector2.zero);
		}
		else if (m_PlayInType == PlayInType.TOP_BOTTOM)
		{
			m_VisualsStartPosition = m_Visuals.anchoredPosition + new Vector2(0f, m_Content.sizeDelta.y);
		}
		else if (m_PlayInType == PlayInType.BOTTOM_TOP)
		{
			m_VisualsStartPosition = m_Visuals.anchoredPosition - new Vector2(0f, m_Content.sizeDelta.y);
		}
		m_Visuals.anchoredPosition = m_VisualsStartPosition;
		((Transform)m_Visuals).localScale = m_VisualsStartScale;
	}

	public override void PlayIn()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		if (m_PlayInType == PlayInType.SCALE)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, m_VisualsOriginScale, 0.5f), (Ease)27), new TweenCallback(PlayInComplete));
		}
		else
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_Visuals.DOAnchorPos(m_VisualsOriginPosition, 0.5f), (Ease)27), new TweenCallback(PlayInComplete));
		}
	}

	public override void PlayOut()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		if (m_PlayInType == PlayInType.SCALE)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, m_VisualsStartScale, 0.5f), (Ease)26), new TweenCallback(PlayOutComplete));
		}
		else
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_Visuals.DOAnchorPos(m_VisualsStartPosition, 0.5f), (Ease)26), new TweenCallback(PlayOutComplete));
		}
	}
}
