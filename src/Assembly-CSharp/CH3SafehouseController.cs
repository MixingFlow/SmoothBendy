using System;
using System.Collections.Generic;
using S13Audio;
using UnityEngine;

public class CH3SafehouseController : BaseController
{
	private const int MAX_SOUP_NEEDED = 3;

	[Header("[SAFEHOUSE]")]
	[SerializeField]
	private GameObject m_SafehouseBlocker;

	[SerializeField]
	private EventTrigger m_SafehouseExitWarning;

	[Header("Objective: Make Soup!")]
	[SerializeField]
	private EventTrigger m_BarricadeTrigger;

	[SerializeField]
	private Interactable m_EmptyPot;

	[SerializeField]
	private Interactable m_FullPot;

	[SerializeField]
	private GameObject m_CarryBowl;

	[SerializeField]
	private List<CannedSoupEdible> TEMP_Soup;

	[SerializeField]
	private CH3BorisBowl m_BorisBowl;

	[SerializeField]
	private ParticleSystem TEMP_BowlParticles;

	[SerializeField]
	private InteractableTrunk m_Trunk;

	[Header("Leaving The Safehouse")]
	[SerializeField]
	private InteractableTrunk m_Toolbox;

	[SerializeField]
	private CH3SafehouseLever m_SafehouseLever;

	[SerializeField]
	private CustomDoorController m_ExitDoor;

	[SerializeField]
	private WaypointList m_BorisExitPath;

	[SerializeField]
	private EventTrigger m_ExitTrigger;

	[SerializeField]
	private EventTrigger m_EnterTrigger;

	[Header("FUN: Toilets!")]
	[SerializeField]
	private Interactable m_Toilet_01;

	[SerializeField]
	private Interactable m_Toilet_02;

	[SerializeField]
	private CustomDoorController m_LockedStall;

	[SerializeField]
	private GameObject m_LockedStallBlocker;

	[Header("FUN: Boris Bone")]
	[SerializeField]
	private CH3BoneController m_Bone;

	[SerializeField]
	private InteractableGeneric m_RecordPlayer;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private int m_CollectedSoupCount;

	private bool m_CanCookSoup;

	private bool m_IsBorisOutside;

	private bool m_IsPlayerOutside;

	private AudioClip m_SoupClip;

	private AudioClip m_FlushClip;

	private AudioClip m_SafehouseDoorClip;

	private AudioClip m_SoupInteractClip;

	private AudioClip m_BorisSoupClip;

	private AudioClip m_HenryLine05Clip;

	private AudioClip m_HenryLine06Clip;

	private AudioClip m_HenryLine07Clip;

	private AudioClip[] m_HenryHeyBuddyClips;

	private AudioClip[] m_HenryNotGettingOutClips;

	private Sprite m_ObjectiveSprite;

	private BorisAi m_Boris => GameManager.Instance.CharacterManager.Boris;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ObjectiveSprite = GameManager.Instance.AssetManager.GetAsset<Sprite>("UI/ObjectiveIcons/soup_icon");
		m_SoupClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_soupcooking");
		m_FlushClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_Flush");
		m_SafehouseDoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_safehousedoor");
		m_SoupInteractClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_souppotinteract");
		m_BorisSoupClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_borisbowlofsoup");
		m_HenryLine05Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_05_thatshouldbeenough");
		m_HenryLine06Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_06_hereyougo");
		m_HenryLine07Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_07_letsseewhatsoutthere");
		m_HenryHeyBuddyClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Henry/HeyBuddy/");
		m_HenryNotGettingOutClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Henry/NotGettingOut/");
		m_FullPot.SetActive(active: false);
		m_FullPot.gameObject.SetActive(false);
		m_SafehouseBlocker.SetActive(false);
		m_EmptyPot.SetActive(active: false);
		m_CarryBowl.SetActive(false);
		m_EnterTrigger.SetActive(active: false);
		m_ExitTrigger.SetActive(active: false);
		m_SafehouseExitWarning.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.SafehouseObjective.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		m_Boris.Interact.SetActive(active: false);
		m_Toilet_01.OnInteracted += HandleToiletOnInteracted;
		m_ExitDoor.Lock();
		for (int i = 0; i < TEMP_Soup.Count; i++)
		{
			TEMP_Soup[i].OnInteracted += HandleBaconSoupOnInteracted;
		}
		m_RecordPlayer.OnInteracted += HandleRecordPlayerOnInteracted;
		m_BarricadeTrigger.SetActive(active: true);
		m_BarricadeTrigger.OnEnter += HandleBarricadeTriggerOnEnter;
	}

	private void ForceComplete()
	{
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_01");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		for (int i = 0; i < TEMP_Soup.Count; i++)
		{
			TEMP_Soup[i].gameObject.SetActive(false);
		}
		m_LockedStall.Unlock();
		m_LockedStall.Open();
		m_LockedStallBlocker.SetActive(false);
		m_Trunk.ForceOpen();
		m_Boris.Interact.SetActive(active: false);
		m_Boris.PlaceToolbox(isSilent: true);
		m_Toolbox.ForceOpen();
		m_SafehouseLever.ForceComplete();
		m_SafehouseBlocker.SetActive(true);
		m_FullPot.SetActive(active: false);
		m_FullPot.gameObject.SetActive(true);
		m_EmptyPot.SetActive(active: false);
		m_EmptyPot.gameObject.SetActive(false);
		m_BorisBowl.Empty();
		m_ExitDoor.ForceClose();
		m_ExitDoor.Lock();
		m_Boris.ForceStand();
		m_Boris.ResetAll();
		m_Boris.LookAtPlayer();
		m_Bone.Activate();
		m_Boris.transform.position = m_BorisExitPath.Waypoints[m_BorisExitPath.Waypoints.Count - 1].transform.position;
		m_Boris.transform.eulerAngles = m_BorisExitPath.Waypoints[m_BorisExitPath.Waypoints.Count - 1].transform.eulerAngles;
		m_BorisExitPath.Dispose();
		m_Boris.StopWaypointPathing();
		SendOnComplete();
	}

	private void HandleRecordPlayerOnInteracted(object sender, EventArgs e)
	{
		m_Boris.SetDancing(m_RecordPlayer.isOn);
	}

	private void HandleBarricadeTriggerOnEnter(object sender, EventArgs e)
	{
		m_BarricadeTrigger.OnEnter -= HandleBarricadeTriggerOnEnter;
		for (int i = 0; i < m_HenryNotGettingOutClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryNotGettingOutClips[i], SubtitleConstants.DIA_CH3_HENRY_NOT_GETTING_OUT[i], isTrimmed: true));
			if (i == 1)
			{
				audioObject.OnComplete += HandleNotGettingOutOnComplete;
			}
		}
	}

	private void HandleNotGettingOutOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_02", "OBJECTIVES/CH3_OBJECTIVE_02_TIP", 4f));
		m_Boris.Interact.SetActive(active: true);
		m_Boris.Interact.OnInteracted += HandleBorisSoupOnInteracted;
	}

	private void HandleBorisSoupOnInteracted(object sender, EventArgs e)
	{
		m_Boris.Interact.OnInteracted -= HandleBorisSoupOnInteracted;
		m_Boris.Interact.SetActive(active: false);
		for (int i = 0; i < m_HenryHeyBuddyClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryHeyBuddyClips[i], SubtitleConstants.DIA_CH3_HENRY_HEY_BUDDY[i], isTrimmed: true));
			if (i == 1)
			{
				audioObject.OnComplete += HandleHeyBuddyOnComplete;
			}
		}
	}

	private void HandleHeyBuddyOnComplete(object sender, EventArgs e)
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_03", "OBJECTIVES/CH3_OBJECTIVE_03_TIP", 4f);
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, m_CollectedSoupCount);
		GameManager.Instance.ShowObjective(objectiveDataVO);
		m_CanCookSoup = true;
		m_Trunk.SetActive(active: true);
	}

	private void HandleBaconSoupOnInteracted(object sender, EventArgs e)
	{
		CannedSoupEdible cannedSoupEdible = (CannedSoupEdible)sender;
		cannedSoupEdible.OnInteracted -= HandleBaconSoupOnInteracted;
		TEMP_Soup.Remove(cannedSoupEdible);
		m_CollectedSoupCount++;
		GameManager.Instance.CurrentObjective.ItemCounter = m_CollectedSoupCount;
		if (m_CollectedSoupCount == 3)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryLine05Clip, "DIACH3/DIA_CH3_HENRY_05")).OnComplete += delegate
			{
				GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_04", string.Empty, 4f));
			};
			m_EmptyPot.SetActive(active: true);
			m_EmptyPot.OnInteracted += HandlePotOnInteracted;
		}
	}

	private void HandlePotOnInteracted(object sender, EventArgs e)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (m_CanCookSoup)
		{
			m_EmptyPot.OnInteracted -= HandlePotOnInteracted;
			m_EmptyPot.Dispose();
			m_FullPot.gameObject.SetActive(true);
			TEMP_BowlParticles.Play();
			AudioObject audioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_SoupClip, m_FullPot.transform.position);
			audioObject.OnComplete += HandleCookOnComplete;
		}
	}

	private void HandleCookOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleCookOnComplete;
		TEMP_BowlParticles.Stop();
		m_FullPot.SetActive(active: true);
		m_FullPot.OnInteracted += HandleCookedSoupOnInteracted;
	}

	private void HandleCookedSoupOnInteracted(object sender, EventArgs e)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		m_FullPot.OnInteracted -= HandleCookedSoupOnInteracted;
		m_FullPot.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_SoupInteractClip);
		m_CarryBowl.transform.SetParent(GameManager.Instance.Player.WeaponParent);
		m_CarryBowl.transform.localPosition = Vector3.zero;
		m_CarryBowl.transform.localEulerAngles = Vector3.zero;
		m_CarryBowl.SetActive(true);
		m_BorisBowl.OnInteracted += HandleBorisBowlOnInteracted;
		m_BorisBowl.Activate();
	}

	private void HandleBorisBowlOnInteracted(object sender, EventArgs e)
	{
		m_BorisBowl.OnInteracted -= HandleBorisBowlOnInteracted;
		GameManager.Instance.AudioManager.Play(m_BorisSoupClip);
		Object.Destroy((Object)(object)m_CarryBowl);
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryLine06Clip, "DIACH3/DIA_CH3_HENRY_06"));
		m_Boris.OnToolboxPlaced += HandleOnToolboxPlaced;
		m_Boris.GetToolbox();
	}

	private void HandleOnToolboxPlaced(object sender, EventArgs e)
	{
		m_Boris.OnToolboxPlaced -= HandleOnToolboxPlaced;
		m_Toolbox.SetActive(active: true);
		m_SafehouseLever.Activate();
		m_SafehouseLever.OnComplete += HandleSafehouseLeverOnComplete;
	}

	private void HandleSafehouseLeverOnComplete(object sender, EventArgs e)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		m_SafehouseLever.OnComplete -= HandleSafehouseLeverOnComplete;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty, 4f));
		GameManager.Instance.AudioManager.PlayAtPosition(m_SafehouseDoorClip, m_ExitDoor.transform.position);
		m_BorisBowl.Empty();
		m_ExitDoor.OnOpened += HandleDoorOnOpened;
		m_ExitDoor.Unlock();
		m_ExitDoor.SetSilent(active: true);
		m_ExitDoor.Open(2f);
		m_Bone.Activate();
		m_LockedStall.Unlock();
		m_LockedStall.Open();
		m_LockedStallBlocker.SetActive(false);
		m_Toilet_02.SetActive(active: true);
		m_Toilet_02.OnInteracted += HandleSpecialToiletOnInteracted;
		m_SafehouseExitWarning.SetActive(active: true);
		m_SafehouseExitWarning.OnEnter += HandleSafehouseExitWarningOnEnter;
		m_ExitTrigger.SetActive(active: true);
		m_ExitTrigger.OnEnter += HandleExitTriggerOnEnter;
	}

	private void HandleSafehouseExitWarningOnEnter(object sender, EventArgs e)
	{
		m_SafehouseExitWarning.OnEnter -= HandleSafehouseExitWarningOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryLine07Clip, "DIACH3/DIA_CH3_HENRY_07"));
	}

	private void HandleExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_ExitTrigger.OnEnter -= HandleExitTriggerOnEnter;
		m_ExitTrigger.SetActive(active: false);
		m_IsPlayerOutside = true;
		if (CheckOutsideStatus())
		{
			CloseSafehouse();
			return;
		}
		m_EnterTrigger.ResetTrigger();
		m_EnterTrigger.SetActive(active: true);
		m_EnterTrigger.OnEnter += HandleEnterTriggerOnEnter;
	}

	private void HandleEnterTriggerOnEnter(object sender, EventArgs e)
	{
		m_EnterTrigger.OnEnter -= HandleEnterTriggerOnEnter;
		m_EnterTrigger.SetActive(active: false);
		m_IsPlayerOutside = false;
		m_ExitTrigger.ResetTrigger();
		m_ExitTrigger.SetActive(active: true);
		m_ExitTrigger.OnEnter += HandleExitTriggerOnEnter;
	}

	private bool CheckOutsideStatus()
	{
		return m_IsPlayerOutside && m_IsBorisOutside;
	}

	private void HandleDoorOnOpened(object sender, EventArgs e)
	{
		m_ExitDoor.OnOpened -= HandleDoorOnOpened;
		m_Boris.OnGetUp += HandleBorisOnGetUp;
		m_Boris.GetUp();
	}

	private void HandleBorisOnGetUp(object sender, EventArgs e)
	{
		m_Boris.OnGetUp -= HandleBorisOnGetUp;
		m_Boris.OnWaypointComplete += HandleBorisOnWaypointComplete;
		m_Boris.UpdateWaypointList(m_BorisExitPath.Waypoints);
	}

	private void HandleBorisOnWaypointComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleBorisOnWaypointComplete;
		m_BorisExitPath.Dispose();
		m_Boris.StopWaypointPathing();
		m_IsBorisOutside = true;
		if (CheckOutsideStatus())
		{
			CloseSafehouse();
		}
	}

	private void CloseSafehouse()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		m_SafehouseBlocker.SetActive(true);
		m_ExitDoor.Close(1.5f);
		GameManager.Instance.AudioManager.PlayAtPosition(m_SafehouseDoorClip, m_ExitDoor.transform.position);
		m_ExitDoor.OnClosed += delegate
		{
			m_ExitDoor.Lock();
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.SafehouseObjective.IsComplete = true;
			GameManager.Instance.GameDataManager.Save();
			SendOnComplete();
		};
	}

	private void HandleSpecialToiletOnInteracted(object sender, EventArgs e)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_Toilet_02.OnInteracted -= HandleSpecialToiletOnInteracted;
		Interactable interactable = (Interactable)sender;
		GameManager.Instance.AudioManager.PlayAtPosition(m_FlushClip, interactable.transform.position);
	}

	private void HandleToiletOnInteracted(object sender, EventArgs e)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_Toilet_01.OnInteracted -= HandleToiletOnInteracted;
		Interactable interactable = (Interactable)sender;
		AudioObject audioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_FlushClip, interactable.transform.position);
		audioObject.OnComplete += delegate
		{
			m_Toilet_01.OnInteracted += HandleToiletOnInteracted;
		};
	}

	protected override void OnDisposed()
	{
		m_ObjectiveSprite = null;
		m_SoupClip = null;
		m_FlushClip = null;
		m_SafehouseDoorClip = null;
		m_SoupInteractClip = null;
		m_BorisSoupClip = null;
		m_HenryLine05Clip = null;
		m_HenryLine06Clip = null;
		m_HenryLine07Clip = null;
		m_HenryHeyBuddyClips = null;
		m_HenryNotGettingOutClips = null;
		base.OnDisposed();
	}
}
