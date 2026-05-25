using TMG.Core;
using UnityEngine;

public class CH4BendyProjAnimationEvents : TMGMonoBehaviour
{
	[SerializeField]
	private CH4ProjectionistBendyFight m_Controller;

	public void EventOnComplete()
	{
		m_Controller.EventOnComplete();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
