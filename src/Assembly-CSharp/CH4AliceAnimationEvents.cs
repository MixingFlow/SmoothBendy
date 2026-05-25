using UnityEngine;

public class CH4AliceAnimationEvents : MonoBehaviour
{
	[SerializeField]
	private CH4ClosingSequenceController m_Controller;

	public void AliceHit()
	{
		m_Controller.AliceHit();
	}

	public void AliceFall()
	{
		m_Controller.AliceFall();
	}

	public void LookAtAlice()
	{
		m_Controller.LookAtAlice();
	}
}
