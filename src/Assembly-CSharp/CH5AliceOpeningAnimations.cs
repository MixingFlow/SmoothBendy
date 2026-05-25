using TMG.Core;
using UnityEngine;

public class CH5AliceOpeningAnimations : TMGMonoBehaviour
{
	[SerializeField]
	private CH5OpeningScenes m_Controller;

	public void DisableBrush()
	{
		m_Controller.DisableBrush();
	}

	public void EnableBrush()
	{
		m_Controller.EnableBrush();
	}

	public void EnableSeeingTool()
	{
		m_Controller.EnableSeeingTool();
	}

	public void DropSeeingTool()
	{
		m_Controller.DropSeeingTool();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
