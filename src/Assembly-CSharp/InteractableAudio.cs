using System.Collections;
using UnityEngine;

public class InteractableAudio : Interactable
{
	[Header("Audio Player Options")]
	[SerializeField]
	private Transform m_AudioLocation;

	[SerializeField]
	private AudioClip m_AudioClip;

	private bool m_CanInteract;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_CanInteract = true;
	}

	public override void OnInteract()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (m_CanInteract)
		{
			m_CanInteract = false;
			GameManager.Instance.AudioManager.PlayAtPosition(m_AudioClip, m_AudioLocation.position);
			((MonoBehaviour)this).StartCoroutine(HandleAudioClipOnComplete(m_AudioClip.length));
		}
	}

	private IEnumerator HandleAudioClipOnComplete(float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		m_CanInteract = true;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
