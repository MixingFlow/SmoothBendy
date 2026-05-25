using System;
using System.Collections;
using UnityEngine;

public class CannedSoupEdibleStack : CannedSoupEdible
{
	[Header("Stacked Options")]
	[SerializeField]
	private CannedSoupEdible m_StackedCan;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if ((Object)(object)m_StackedCan != (Object)null)
		{
			m_Active = false;
			m_StackedCan.OnInteracted += HandleStackedCanOnInteracted;
		}
		((MonoBehaviour)this).StartCoroutine(DelayStackCheck());
	}

	private IEnumerator DelayStackCheck()
	{
		yield return (object)new WaitForSeconds(1f);
		yield return (object)new WaitForEndOfFrame();
		if ((Object)(object)m_StackedCan == (Object)null)
		{
			m_Active = true;
		}
	}

	private void HandleStackedCanOnInteracted(object sender, EventArgs e)
	{
		m_StackedCan.OnInteracted += HandleStackedCanOnInteracted;
		m_Active = true;
	}

	public override void OnInteract()
	{
		if ((Object)(object)m_StackedCan != (Object)null)
		{
			m_StackedCan.OnInteracted += HandleStackedCanOnInteracted;
		}
		PlayEatSound();
		Dispose();
	}
}
