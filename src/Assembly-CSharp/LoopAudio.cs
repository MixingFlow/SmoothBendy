using System.Collections;
using TMG.Core;
using UnityEngine;

public class LoopAudio : TMGMonoBehaviour
{
	[SerializeField]
	private AudioClip m_AudioClip;

	[SerializeField]
	private float delay;

	private bool m_IsActive;

	public override void OnEnable()
	{
		m_IsActive = true;
		((MonoBehaviour)this).StartCoroutine(DelayAudio());
	}

	public override void OnDisable()
	{
		((MonoBehaviour)this).StopCoroutine(DelayAudio());
		m_IsActive = false;
	}

	private IEnumerator DelayAudio()
	{
		while (m_IsActive && !base.IsDisposed)
		{
			yield return (object)new WaitForSeconds(delay);
			if (m_IsActive && !base.IsDisposed)
			{
				GameManager.Instance.AudioManager.PlayAtPosition(m_AudioClip, base.transform.position);
			}
		}
	}

	protected override void OnDisposed()
	{
		((MonoBehaviour)this).StopCoroutine(DelayAudio());
		base.OnDisposed();
	}
}
