using DG.Tweening;
using UnityEngine;

public class CH1OpeningSequenceController : BaseController
{
	private Sequence m_Sequence;

	private AudioClip m_DoorClip;

	private AudioClip m_HenryClip01;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_DoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/Door/SFX_Door_Unlock_Open_Close_01");
		m_HenryClip01 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_01");
	}

	public override void Activate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.InkMachineRevealObjective.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			TweenSettingsExtensions.OnComplete<Sequence>(DOSequence(), new TweenCallback(base.SendOnComplete));
		}
	}

	private void ForceComplete()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.Player.transform.position = GameManager.Instance.GameData.CurrentSaveFile.CH1Data.PlayerPosition.GetVector();
		GameManager.Instance.Player.LookRotation(Quaternion.Euler(GameManager.Instance.GameData.CurrentSaveFile.CH1Data.PlayerRotation.GetVector()));
		GameManager.Instance.HideScreenBlocker(0.1f, 0.5f);
		UnlockPlayer();
		SendOnComplete();
	}

	private Sequence DOSequence()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		ResetSequence();
		float num = 1f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(0.1f);
			GameManager.Instance.AudioManager.Play(m_DoorClip);
		});
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowChapterTitle("MENU/CH1_LABEL", "MENU/CH1_TITLE");
		});
		num += 4f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(UnlockPlayer));
		TweenSettingsExtensions.InsertCallback(m_Sequence, 1f + m_HenryClip01.length, new TweenCallback(PlayInitialDialogue));
		return m_Sequence;
	}

	private void UnlockPlayer()
	{
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.ShowCrosshair();
	}

	private void PlayInitialDialogue()
	{
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip01, "DIACH1/DIA_CH1_HENRY_01"));
		audioObject.OnComplete += delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_01", "OBJECTIVES/CH1_OBJ_01_TIP", 4f));
		};
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		m_HenryClip01 = null;
		base.OnDisposed();
	}
}
