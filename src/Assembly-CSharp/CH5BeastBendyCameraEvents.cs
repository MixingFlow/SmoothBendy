using TMG.Core;
using UnityEngine;

public class CH5BeastBendyCameraEvents : TMGMonoBehaviour
{
	private CH5ThroneRoom m_Controller;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Controller = Object.FindObjectOfType<CH5ThroneRoom>();
	}

	public void OnComplete()
	{
		m_Controller.RevealOnComplete();
	}
}
