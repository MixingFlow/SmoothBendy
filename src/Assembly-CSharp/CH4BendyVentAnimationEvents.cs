using TMG.Core;
using UnityEngine;

public class CH4BendyVentAnimationEvents : TMGMonoBehaviour
{
	[SerializeField]
	private CH4BendyVent m_Controller;

	public void OnComplete()
	{
		m_Controller.OnComplete();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
