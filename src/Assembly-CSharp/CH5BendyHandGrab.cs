using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5BendyHandGrab : TMGMonoBehaviour
{
	[SerializeField]
	private Animator m_HandAnimator;

	[SerializeField]
	private Animator m_BoatAnimator;

	[SerializeField]
	private RiverWaves m_Wave;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		((Component)m_HandAnimator).gameObject.SetActive(false);
		((Component)m_BoatAnimator).gameObject.SetActive(false);
		base.InitOnComplete();
	}

	public void Activate()
	{
		((Component)m_HandAnimator).gameObject.SetActive(true);
		((Component)m_BoatAnimator).gameObject.SetActive(true);
	}

	public void Play()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		GameManager.Instance.AudioManager.Play("Audio/SFX/sfx_BendyHand_Alice_Tom_Boat_Sink_Sequence");
		m_Wave.AddWave(base.transform.position, 15f, 0.05f, 30f);
		m_HandAnimator.SetTrigger("Play");
		m_BoatAnimator.SetTrigger("Play");
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 9f, (TweenCallback)delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			m_Wave.AddWave(base.transform.position, 7.5f, 0.06f, 3f);
		});
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 15f, (TweenCallback)delegate
		{
			this.OnComplete.Send(this);
		});
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
