using TMG.Core;
using UnityEngine;

public class CH5BendyDeathEvents : TMGMonoBehaviour
{
	private CH5TheEnd m_Controller;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Controller = Object.FindObjectOfType<CH5TheEnd>();
	}

	public void BlankScreen1()
	{
		m_Controller.BlankScreen1();
	}

	public void BlankScreen2()
	{
		m_Controller.BlankScreen2();
	}

	public void TheEnd1()
	{
		m_Controller.TheEnd1();
	}

	public void TheEnd2()
	{
		m_Controller.TheEnd2();
	}

	public void TheEndRest()
	{
		m_Controller.TheEndRest();
	}

	public void THE_END()
	{
		m_Controller.THE_END();
	}

	protected override void OnDisposed()
	{
		m_Controller = null;
		base.OnDisposed();
	}
}
