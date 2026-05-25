using TMG.Core;
using UnityEngine;

public class CH4TomAnimationEvents : TMGMonoBehaviour
{
	[SerializeField]
	private CH4ClosingSequenceController m_Controller;

	public void TomTap()
	{
		m_Controller.TomTap();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
