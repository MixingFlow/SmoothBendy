using System;
using Ai;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH2CSmasherController : TMGMonoBehaviour
{
	[SerializeField]
	private CH2SewerController m_SewerController;

	[SerializeField]
	private Transform m_Smasher;

	[SerializeField]
	private CH3LeverLight m_Lever;

	[SerializeField]
	private CH3LeverLight m_LeverUp;

	[SerializeField]
	private Transform m_TopLocation;

	[SerializeField]
	private Transform m_BottomLocation;

	[SerializeField]
	private ParticleSystem m_Particles;

	[SerializeField]
	private Transform m_SearcherB;

	[SerializeField]
	private GameObject m_MinerHat;

	private AudioClip m_RumbleClip;

	private AudioObject m_RumbleAudio;

	public bool IsDown { get; private set; }

	public override void InitOnComplete()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_Lever.Disable();
		m_LeverUp.Disable();
		IsDown = true;
		m_Lever.SetSingleInteraction(active: false);
		m_Lever.Activate(isAlreadyActive: true);
		m_Lever.OnComplete += HandleLeverOnComplete;
		m_LeverUp.SetSingleInteraction(active: false);
		m_LeverUp.OnComplete += HandleLeverUpOnComplete;
		m_LeverUp.Activate();
		m_RumbleClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Rumble_Loop_01");
		((Component)m_Smasher).transform.position = m_BottomLocation.position;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InternecionValue == 414)
		{
			m_MinerHat.SetActive(true);
		}
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		IsDown = true;
		Sequence val = DOTween.Sequence();
		float num = 0.5f;
		if ((Object)(object)m_SewerController != (Object)null && m_SewerController.m_CanKillJack)
		{
			TweenSettingsExtensions.InsertCallback(val, 0.75f, new TweenCallback(m_SewerController.KillJack));
			m_SewerController = null;
		}
		TweenSettingsExtensions.InsertCallback(val, 0.75f, (TweenCallback)delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Expected O, but got Unknown
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Expected O, but got Unknown
			m_Particles.Emit(30);
			Collider[] array = Physics.OverlapSphere(m_BottomLocation.position, 3.5f, LayerMask.GetMask(new string[1] { "Ai" }));
			for (int i = 0; i < array.Length; i++)
			{
				BaseAiController component = ((Component)array[i]).GetComponent<BaseAiController>();
				if (Object.op_Implicit((Object)(object)component))
				{
					if (Object.op_Implicit((Object)(object)m_SearcherB) && (Object)(object)component.transform == (Object)(object)m_SearcherB)
					{
						GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InternecionValue = 414;
						GameManager.Instance.GameData.CurrentSaveFile.Internecions[1] = 4;
						GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
						m_MinerHat.SetActive(true);
						GameManager.Instance.GameCamera.VisionEffect.BeginEffect();
						TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
						{
							GameManager.Instance.GameCamera.VisionEffect.EndEffect();
						});
						m_RumbleAudio = GameManager.Instance.AudioManager.Play(m_RumbleClip, AudioObjectType.SOUND_EFFECT, -1);
						TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 5f, 0.1f, 15, 90f, false, false), new TweenCallback(ScreenRumbleOnComplete));
					}
					component.Dispose();
				}
			}
		});
		S13AudioManager.Instance.InvokeEvent("evt_sewer_puzzle_winch_down");
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Smasher, m_BottomLocation.position, 1f, false), (Ease)30));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(SmasherDownOnComplete));
	}

	private void ScreenRumbleOnComplete()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		ShortcutExtensions.DOLocalMove(GameManager.Instance.GameCamera.transform, Vector3.zero, 0.5f, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_RumbleAudio.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
		{
			if ((Object)(object)m_RumbleAudio != (Object)null)
			{
				m_RumbleAudio.Clear();
				m_RumbleAudio = null;
			}
		});
	}

	private void SmasherDownOnComplete()
	{
		m_LeverUp.DOReset();
	}

	private void HandleLeverUpOnComplete(object sender, EventArgs e)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0.5f;
		S13AudioManager.Instance.InvokeEvent("evt_sewer_puzzle_winch_up_start");
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Smasher, m_TopLocation.position, 8f, false), (Ease)5));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(SmasherUpOnComplete));
	}

	private void SmasherUpOnComplete()
	{
		m_Lever.DOReset();
		IsDown = false;
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_Lever))
		{
			m_Lever.OnComplete -= HandleLeverOnComplete;
		}
		if (Object.op_Implicit((Object)(object)m_LeverUp))
		{
			m_LeverUp.OnComplete -= HandleLeverOnComplete;
		}
		base.OnDisposed();
	}
}
