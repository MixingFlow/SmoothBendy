using System;
using S13Audio;
using UnityEngine;

public class CH2LostKeysController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private CH2AudioLogsController m_AudioLogController;

	[Header("Objective: Find The Keys")]
	[SerializeField]
	private Interactable m_Keys;

	[SerializeField]
	private BaseDoorController m_ClosetDoor;

	[SerializeField]
	private Sprite m_KeySprite;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_KeyClip;

	public Sprite KeySprite => m_KeySprite;

	public override void InitOnComplete()
	{
		m_KeyClip = GameManager.Instance.GetAudioClip("Audio/SFX/Collectables/SFX_Keys_Pickup_01");
		m_Keys.SetActive(active: false);
		m_ClosetDoor.Lock();
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.LostKeysObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.LostKeysObjective.IsStarted)
		{
			ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_UNLOCK_CLOSET", "OBJECTIVES/CH2_OBJECTIVE_UNLOCK_CLOSET_TIP");
			objectiveDataVO.AddItemCounter(m_KeySprite, 0);
			GameManager.Instance.UpdateObjective(objectiveDataVO);
			m_AudioLogController.Activate();
			m_Keys.OnInteracted += HandleKeysOnCollected;
			m_Keys.SetActive(active: true);
		}
		else
		{
			m_AudioLogController.OnLostKeyObjective += HandleLostKeysObjectiveOnActive;
			m_AudioLogController.Activate();
		}
	}

	private void HandleLostKeysObjectiveOnActive(object sender, EventArgs e)
	{
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.LostKeysObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		m_Keys.OnInteracted += HandleKeysOnCollected;
		m_Keys.SetActive(active: true);
	}

	private void HandleKeysOnCollected(object sender, EventArgs e)
	{
		m_Keys.OnInteracted -= HandleKeysOnCollected;
		m_Keys.Dispose();
		GameManager.Instance.ShowCollectable(CollectableDataVO.Create(m_KeyClip, "UI/ChapterOneCollectables/ChapterOneCollectables", "collectable_keys"));
		m_ClosetDoor.Unlock();
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.LostKeysObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH2_save_point_05");
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_UNLOCK_CLOSET", "OBJECTIVES/CH2_OBJECTIVE_UNLOCK_CLOSET_TIP");
		objectiveDataVO.AddItemCounter(m_KeySprite, 1);
		GameManager.Instance.UpdateObjective(objectiveDataVO);
		m_AudioLogController.Activate();
		m_Keys.Dispose();
		m_ClosetDoor.Unlock();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_Keys))
		{
			m_Keys.OnInteracted -= HandleKeysOnCollected;
		}
		m_KeyClip = null;
		base.OnDisposed();
	}
}
