using TMG.Core;
using UnityEngine;

public class CH5TomOpeningAnimations : TMGMonoBehaviour
{
	[SerializeField]
	private CH5OpeningScenes m_Controller;

	public void TossSoupBowl()
	{
		m_Controller.TossSoupBowl();
	}

	public void OpenDoor()
	{
		m_Controller.OpenDoor();
	}

	public void CloseDoor()
	{
		m_Controller.CloseDoor();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
