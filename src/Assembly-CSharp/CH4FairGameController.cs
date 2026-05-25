using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4FairGameController : BaseController
{
	[Header("[Power Station]")]
	[SerializeField]
	private CH4WarehousePowerStation m_PowerStation;

	[Header("Games")]
	[SerializeField]
	private Minigame_ShootingGallery m_ShootingGallery;

	[SerializeField]
	private Minigame_BallToss m_BallToss;

	[SerializeField]
	private Transform m_InactiveBooth;

	[Header("Door")]
	[SerializeField]
	private GenericDoorController m_FairGameDoor;

	[Header("Lever")]
	[SerializeField]
	private CH3LeverLight m_FairGameLever;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private bool m_HasWonShootingGallery;

	private bool m_HasWonBallToss;

	private S13ObjectSimple m_AudioSimple;

	private AudioClip m_Henry04Clip;

	public override void InitOnComplete()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_HasWonShootingGallery = false;
		m_HasWonBallToss = false;
		m_InactiveBooth.localEulerAngles = new Vector3(90f, 0f, 0f);
		m_FairGameLever.Disable();
		m_AudioSimple = ((Component)m_InactiveBooth).GetComponentInChildren<S13ObjectSimple>();
		m_Henry04Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Henry/DIA_CH4_HENRY_04");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.FairGamesObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.FairGamesObjective.IsStarted)
		{
			ForceOpenFairGames();
			m_FairGameDoor.ForceOpen();
			m_FairGameLever.ForceComplete();
			m_PowerStation.OnPowerActivated += HandlePowerStationOnPowerActivated;
			m_PowerStation.ActivatePower();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.WarehouseObjective.IsComplete)
		{
			m_ShootingGallery.OnWin += HandleShootingGalleryOnWin;
			m_BallToss.OnWin += HandleBallTossOnWin;
			ForceOpenFairGames();
		}
		else
		{
			IniializeFairGames();
		}
	}

	private void ForceComplete()
	{
		ForceOpenFairGames();
		m_FairGameDoor.ForceOpen();
		m_FairGameLever.ForceComplete();
		m_PowerStation.ForceActivatePower();
		SendOnComplete();
	}

	private void IniializeFairGames()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		m_ShootingGallery.OnWin += HandleShootingGalleryOnWin;
		m_ShootingGallery.Activate();
		m_BallToss.OnWin += HandleBallTossOnWin;
		m_BallToss.Activate();
		m_AudioSimple.Play();
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_InactiveBooth, new Vector3(-90f, 0f, 0f), 3f, (RotateMode)3), (Ease)7);
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.WarehouseObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void ForceOpenFairGames()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		m_ShootingGallery.ForceOpen();
		m_BallToss.ForceOpen();
		m_InactiveBooth.localEulerAngles = Vector3.zero;
	}

	private void HandleShootingGalleryOnWin(object sender, EventArgs e)
	{
		m_ShootingGallery.OnWin -= HandleShootingGalleryOnWin;
		m_HasWonShootingGallery = true;
		CheckWinStatus();
	}

	private void HandleBallTossOnWin(object sender, EventArgs e)
	{
		m_BallToss.OnWin -= HandleBallTossOnWin;
		m_HasWonBallToss = true;
		CheckWinStatus();
	}

	private void CheckWinStatus()
	{
		if (m_HasWonShootingGallery && m_HasWonBallToss)
		{
			m_FairGameDoor.Open();
			m_FairGameLever.OnComplete += HandleFairGameLeverOnComplete;
			m_FairGameLever.Activate();
		}
	}

	private void HandleFairGameLeverOnComplete(object sender, EventArgs e)
	{
		m_FairGameLever.OnComplete -= HandleFairGameLeverOnComplete;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Henry04Clip, SubtitleConstants.DIA_CH4_HENRY_04));
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.FairGamesObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		m_PowerStation.OnPowerActivated += HandlePowerStationOnPowerActivated;
		m_PowerStation.ActivatePower();
	}

	private void HandlePowerStationOnPowerActivated(object sender, EventArgs e)
	{
		m_PowerStation.OnPowerActivated -= HandlePowerStationOnPowerActivated;
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.FairGamesObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_AudioSimple = null;
		m_Henry04Clip = null;
		base.OnDisposed();
	}
}
