using DG.Tweening;
using TMG.UI;
using TMPro;
using UnityEngine;

public class AudioLogModalController : BaseUIController
{
	private const float MAX_DISTANCE = 15f;

	[Header("Canvas")]
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI m_NameLbl;

	[SerializeField]
	private TextMeshProUGUI m_LogLbl;

	private AudioLogDataVO m_DataVO;

	private bool m_IsInRange;

	private bool m_IsComplete;

	public override void InitController(object _data)
	{
		base.InitController(_data);
		m_DataVO = (AudioLogDataVO)_data;
		m_CanvasGroup.alpha = 0f;
		m_IsInRange = true;
		m_IsComplete = false;
		InitText();
	}

	private void InitText()
	{
		if (m_DataVO.NameString == string.Empty)
		{
			m_NameLbl.text = m_DataVO.Name;
		}
		else
		{
			m_NameLbl.text = m_DataVO.NameString;
		}
		if (m_DataVO.LogString == string.Empty)
		{
			m_LogLbl.text = m_DataVO.Log;
		}
		else
		{
			m_LogLbl.text = m_DataVO.LogString;
		}
	}

	public override void PlayIn()
	{
		ShortcutExtensions.DOKill((Component)(object)m_CanvasGroup, false);
		m_CanvasGroup.DOFade(1f, 0.5f);
	}

	private void Update()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsComplete)
		{
			if ((Object)(object)GameManager.Instance.Player != (Object)null && Vector3.Distance(GameManager.Instance.Player.transform.position, m_DataVO.LogWorldPosition) > 15f)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}
	}

	private void Show()
	{
		if (!m_IsInRange && !m_IsComplete)
		{
			m_IsInRange = true;
			ShortcutExtensions.DOKill((Component)(object)m_CanvasGroup, false);
			m_CanvasGroup.DOFade(1f, 0.5f);
		}
	}

	private void Hide()
	{
		if (m_IsInRange && !m_IsComplete)
		{
			m_IsInRange = false;
			ShortcutExtensions.DOKill((Component)(object)m_CanvasGroup, false);
			m_CanvasGroup.DOFade(0f, 0.5f);
		}
	}

	public override void PlayOut()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		m_IsComplete = true;
		m_IsInRange = false;
		ShortcutExtensions.DOKill((Component)(object)m_CanvasGroup, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_CanvasGroup.DOFade(0f, 0.5f), new TweenCallback(PlayOutComplete));
	}

	public override void PlayOutComplete()
	{
		base.PlayOutComplete();
	}

	protected override void OnDisposed()
	{
		m_DataVO = null;
		ShortcutExtensions.DOKill((Component)(object)m_CanvasGroup, false);
		base.OnDisposed();
	}
}
