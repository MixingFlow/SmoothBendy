using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4OpeningSequenceController : BaseController
{
	private Sequence m_Sequence;

	public override void Activate()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.AccountingObjective.IsStarted)
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
		GameManager.Instance.Player.transform.position = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.PlayerPosition.GetVector();
		GameManager.Instance.Player.LookRotation(Quaternion.Euler(GameManager.Instance.GameData.CurrentSaveFile.CH4Data.PlayerRotation.GetVector()));
		GameManager.Instance.HideScreenBlocker(0.1f, 0.5f);
		UnlockPlayer();
		SendOnComplete();
	}

	private Sequence DOSequence()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		ResetSequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			S13AudioManager.Instance.PlayAudio("vo_henry_ch4_start");
		});
		num += 8.5f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num + 0.1f, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(0f);
		});
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowChapterTitle("MENU/CH4_LABEL", "MENU/CH4_TITLE");
		});
		num += 4f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(UnlockPlayer));
		num += 2.5f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_RESCUE_BORIS", string.Empty, 4f));
		});
		return m_Sequence;
	}

	private void UnlockPlayer()
	{
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.ShowCrosshair();
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
		base.OnDisposed();
	}
}
