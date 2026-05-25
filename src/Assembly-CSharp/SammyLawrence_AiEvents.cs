using TMG.Core;
using UnityEngine;

public class SammyLawrence_AiEvents : TMGMonoBehaviour
{
	[SerializeField]
	private SammyLawrence_Ai m_sammy;

	public void Attack()
	{
		m_sammy.AttackTarget();
		m_sammy.BreakPlank();
	}

	public void Attack_NoDamage()
	{
		m_sammy.BreakPlank();
	}

	public void GetAxe()
	{
		m_sammy.GetAxe();
	}
}
