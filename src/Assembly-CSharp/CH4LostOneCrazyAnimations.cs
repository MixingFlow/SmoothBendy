using TMG.Core;
using UnityEngine;

public class CH4LostOneCrazyAnimations : TMGMonoBehaviour
{
	[SerializeField]
	private CH4LostOneCrazy m_Controller;

	public void Footstep()
	{
		m_Controller.PlayFootsteps();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
