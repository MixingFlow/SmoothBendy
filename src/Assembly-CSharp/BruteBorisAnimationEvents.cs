using TMG.Core;
using UnityEngine;

public class BruteBorisAnimationEvents : TMGMonoBehaviour
{
	[SerializeField]
	private BruteBorisAi m_Controller;

	private AudioClip m_MusicBadDog;

	private AudioClip m_HenryDialogue;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_MusicBadDog = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_BadDog");
		m_HenryDialogue = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Henry/DIA_CH4_HENRY_08");
	}

	public void Attack1()
	{
		m_Controller.ApplyAttack(0);
	}

	public void Attack2()
	{
		m_Controller.ApplyAttack(1);
	}

	public void DoAttack()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_Controller.AttackTarget(Vector3.zero);
	}

	public void DoJump()
	{
		m_Controller.DoJump();
	}

	public void Stomp()
	{
		m_Controller.ApplyStomp();
	}

	public void DoSmash()
	{
		m_Controller.DoSmash();
	}

	public void ShakeCamera()
	{
		m_Controller.ApplyShake();
	}

	public void RevealGrabCart()
	{
		GameManager.Instance.AudioManager.Play(m_MusicBadDog, AudioObjectType.MUSIC);
		m_Controller.ApplyShake(1.5f);
	}

	public void PickupCart()
	{
		m_Controller.PickupCart();
	}

	public void ThrowCart()
	{
		m_Controller.ThrowCart();
	}

	public void PlaySmashAudio()
	{
		m_Controller.PlaySmashAudio();
	}

	public void Reveal()
	{
		m_Controller.Reveal();
	}

	public void HenryDialogue()
	{
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryDialogue, SubtitleConstants.DIA_CH4_HENRY_08));
	}

	public void ParentCart()
	{
		m_Controller.SetCartParent();
	}

	public void UnlockPlayer()
	{
		m_Controller.UnlockPlayer();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
