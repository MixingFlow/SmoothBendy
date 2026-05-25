using DG.Tweening;
using TMPro;
using UnityEngine;

public class ObjectivesController : AbstractGameMenuController
{
	[Header("TextMeshProUGUIs")]
	[SerializeField]
	private TextMeshProUGUI m_Header;

	[SerializeField]
	private TextMeshProUGUI m_Objective;

	[SerializeField]
	private TextMeshProUGUI m_Tip;

	private ObjectiveDataVO m_DataVO;

	public override void InitController(object _data)
	{
		base.InitController(_data);
		m_DataVO = (ObjectiveDataVO)_data;
		m_Header.text = m_DataVO.Header;
		m_Objective.text = m_DataVO.Objective;
		m_Tip.text = m_DataVO.Tip;
		m_PlayoutDelay = m_DataVO.Delay;
		m_PlayInDelay = m_DataVO.PlayInDelay;
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		if (!m_DataVO.IsCurrentObjective)
		{
			Kill();
		}
	}

	public void Hide()
	{
		m_PlayoutDelay = 0f;
		ShortcutExtensions.DOKill((Component)(object)m_Visuals, false);
		Kill();
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
