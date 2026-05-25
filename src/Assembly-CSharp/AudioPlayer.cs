using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AudioPlayer : Interactable
{
	[Header("Audio Player Options")]
	[SerializeField]
	private Transform m_AudioPlayer;

	[SerializeField]
	private AudioClip m_AudioClip;

	[SerializeField]
	private bool m_EnableAnimation = true;

	public override void Init()
	{
		base.Init();
	}

	public override void OnInteract()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.AudioManager.PlayAtPosition(m_AudioClip, base.transform.position);
		if (m_EnableAnimation && (Object)(object)m_AudioPlayer != (Object)null)
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_AudioPlayer, 1.01f, 0.2f), (Ease)7), -1, (LoopType)1);
			((MonoBehaviour)this).StartCoroutine(HandleAudioClipOnComplete(m_AudioClip.length));
		}
	}

	private IEnumerator HandleAudioClipOnComplete(float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		KillAudioTween();
		m_AudioPlayer.localScale = Vector3.one;
	}

	private void KillAudioTween()
	{
		ShortcutExtensions.DOKill((Component)(object)m_AudioPlayer, false);
	}

	protected override void OnDisposed()
	{
		KillAudioTween();
		base.OnDisposed();
	}
}
