using System.Collections;
using DG.Tweening;
using TMG.Controls;
using TMG.UI;
using TMPro;
using UnityEngine;

public class AttentionPromptController : BaseUIController
{
	[Header("Visuals")]
	[SerializeField]
	private RectTransform m_Visuals;

	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[Header("TextMeshPro")]
	[SerializeField]
	private TextMeshProUGUI m_Header;

	[SerializeField]
	private TextMeshProUGUI m_Message;

	private AttentionDataVO m_DataVO;

	private Vector3 m_VisualsOriginScale;

	private Vector3 m_VisualsShrinkScale;

	private bool m_IsActive;

	public override void InitController(object _data)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_DataVO = (AttentionDataVO)_data;
		m_Header.text = m_DataVO.Header;
		m_Message.text = m_DataVO.Message;
		m_CanvasGroup.alpha = 0f;
		m_VisualsOriginScale = ((Transform)m_Visuals).localScale;
		m_VisualsShrinkScale = m_VisualsOriginScale * 0.75f;
		((Transform)m_Visuals).localScale = m_VisualsShrinkScale;
	}

	public override void PlayIn()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		TweenSettingsExtensions.SetEase<Tweener>(m_CanvasGroup.DOFade(1f, 0.45f), (Ease)5);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, m_VisualsOriginScale, 0.5f), (Ease)5), new TweenCallback(PlayInComplete));
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		((MonoBehaviour)this).StartCoroutine(DelayActivate());
	}

	private IEnumerator DelayActivate()
	{
		yield return (object)new WaitForSeconds(0.5f);
		yield return (object)new WaitForEndOfFrame();
		m_IsActive = true;
	}

	private void Update()
	{
		if (m_IsActive && PlayerInput.Any())
		{
			m_IsActive = false;
			Kill();
		}
	}

	public override void PlayOut()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		TweenSettingsExtensions.SetEase<Tweener>(m_CanvasGroup.DOFade(0f, 0.45f), (Ease)5);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale((Transform)(object)m_Visuals, m_VisualsShrinkScale, 0.5f), (Ease)5), new TweenCallback(PlayOutComplete));
	}

	public override void PlayOutComplete()
	{
		base.PlayOutComplete();
	}

	protected override void OnDisposed()
	{
		m_DataVO = null;
		base.OnDisposed();
	}
}
