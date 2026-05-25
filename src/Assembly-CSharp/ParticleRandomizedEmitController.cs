using System.Collections;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class ParticleRandomizedEmitController : TMGMonoBehaviour
{
	[SerializeField]
	private bool m_StartActive;

	[SerializeField]
	private bool m_fireOnAwake;

	[SerializeField]
	private Vector2 m_RandomRange = new Vector2(1f, 3f);

	private bool m_ActiveSystem;

	private ParticleSystem m_System;

	public void SetActive(bool _IsActive)
	{
		m_ActiveSystem = _IsActive;
		if (_IsActive)
		{
			((MonoBehaviour)this).StartCoroutine(RandomizeParticleEmission());
			if (m_fireOnAwake)
			{
				DoEmit();
			}
		}
		else
		{
			((MonoBehaviour)this).StopAllCoroutines();
		}
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_System = ((Component)this).GetComponent<ParticleSystem>();
		SetActive(m_StartActive);
	}

	private IEnumerator RandomizeParticleEmission()
	{
		yield return (object)new WaitForSeconds(Random.Range(m_RandomRange.x, m_RandomRange.y));
		DoEmit();
		if (m_ActiveSystem)
		{
			((MonoBehaviour)this).StartCoroutine(RandomizeParticleEmission());
		}
	}

	private void DoEmit()
	{
		S13AudioManager.Instance.PlayAudio("sfx_bert_steam_bursts");
		m_System.Play();
	}
}
