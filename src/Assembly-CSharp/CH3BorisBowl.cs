using UnityEngine;

public class CH3BorisBowl : Interactable
{
	[SerializeField]
	private GameObject m_FullBowl;

	[SerializeField]
	private GameObject m_EmptyBowl;

	[SerializeField]
	private Renderer m_HighlightBowl;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		SetActive(active: false);
		m_FullBowl.SetActive(false);
		m_EmptyBowl.SetActive(false);
		((Component)m_HighlightBowl).gameObject.SetActive(false);
	}

	public void Activate()
	{
		((Component)m_HighlightBowl).gameObject.SetActive(true);
		m_HighlightBowl.material.SetFloat("_Shimmer", 1f);
		SetActive(active: true);
	}

	public override void OnInteract()
	{
		base.OnInteract();
		((Component)m_HighlightBowl).gameObject.SetActive(false);
		m_FullBowl.SetActive(true);
	}

	public void Empty()
	{
		m_FullBowl.SetActive(false);
		m_EmptyBowl.SetActive(true);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
