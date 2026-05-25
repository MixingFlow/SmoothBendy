using DG.Tweening;
using TMG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterTitleModalController : BaseUIController
{
	[Header("RectTransforms")]
	[SerializeField]
	private RectTransform m_TitleAnchor;

	[SerializeField]
	private RectTransform m_QuoteStart;

	[SerializeField]
	private RectTransform m_QuoteEnd;

	[Header("CanvasGroup")]
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[Header("Images")]
	[SerializeField]
	private Image m_Blocker;

	[Header("TextMeshProUGUI")]
	[SerializeField]
	private TextMeshProUGUI m_ChapterLbl;

	[SerializeField]
	private TextMeshProUGUI m_TitleLbl;

	private ChapterTitleDataVO m_DataVO;

	private Sequence m_Sequence;

	public override void InitController(object _data)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_DataVO = (ChapterTitleDataVO)_data;
		m_ChapterLbl.text = m_DataVO.Chapter;
		m_TitleLbl.text = m_DataVO.Title;
		((Component)m_Blocker).gameObject.SetActive(m_DataVO.ShowBlocker);
		m_TitleLbl.ForceMeshUpdate();
		float num = m_TitleLbl.renderedWidth / 2f;
		float num2 = m_QuoteStart.sizeDelta.x / 2f;
		m_QuoteStart.anchoredPosition = new Vector2(0f - num - num2, 0f);
		m_QuoteEnd.anchoredPosition = new Vector2(num + num2, 0f);
		m_CanvasGroup.alpha = 0f;
		((Transform)m_TitleAnchor).localScale = Vector3.one * 0.8f;
	}

	public override void PlayIn()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		m_Sequence = DOTween.Sequence();
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)m_CanvasGroup.DOFade(1f, 2f));
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)ShortcutExtensions.DOScale((Transform)(object)m_TitleAnchor, 1f, 5f));
		if (m_DataVO.ShowBlocker)
		{
			TweenSettingsExtensions.Insert(m_Sequence, 2f, (Tween)(object)m_Blocker.DOFade(0f, 2.5f));
		}
		TweenSettingsExtensions.Insert(m_Sequence, 4f, (Tween)(object)m_CanvasGroup.DOFade(0f, 1f));
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(PlayInComplete));
	}

	public override void PlayInComplete()
	{
		Kill();
	}

	public override void PlayOutComplete()
	{
		base.PlayOutComplete();
	}

	protected override void OnDisposed()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
		base.OnDisposed();
	}
}
