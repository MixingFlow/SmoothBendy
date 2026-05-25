using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4WarehouseController : BaseController
{
	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_EntranceTrigger;

	[SerializeField]
	private EventTrigger m_HauntedHouseTrigger;

	[Header("Lighting")]
	[SerializeField]
	private ParticleSystem m_BendySignSparks;

	[SerializeField]
	private LightBulbController[] m_BendyLandLights;

	[SerializeField]
	private GameObject[] m_BendyLandSupportLights;

	[SerializeField]
	private MeshRenderer[] m_HangingLights;

	[SerializeField]
	private GameObject[] m_ActualLights;

	[SerializeField]
	private LightController m_HauntedHouseLantern;

	[SerializeField]
	private GameObject[] m_FinalLight;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip[] m_HenryHauntedHouseClips;

	private AudioClip m_MusicWelcomeToBendyLand;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HenryHauntedHouseClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH4/Henry/HauntedHouse/");
		m_MusicWelcomeToBendyLand = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_WelcomeToBendyLand");
		m_HauntedHouseLantern.TurnOff();
		m_HauntedHouseTrigger.SetActive(active: false);
		for (int i = 0; i < m_HangingLights.Length; i++)
		{
			((Renderer)m_HangingLights[i]).material.SetInt("_LightOn", 0);
			m_ActualLights[i].SetActive(false);
		}
		for (int j = 0; j < m_BendyLandLights.Length; j++)
		{
			m_BendyLandLights[j].TurnOff();
		}
		for (int k = 0; k < m_BendyLandSupportLights.Length; k++)
		{
			m_BendyLandSupportLights[k].SetActive(false);
		}
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.WarehouseObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.WarehouseObjective.IsStarted)
		{
			ForceStart();
			return;
		}
		m_EntranceTrigger.OnEnter += HandleEntranceTriggerOnEnter;
		m_EntranceTrigger.SetActive(active: true);
	}

	private void ForceStart()
	{
		for (int i = 0; i < m_HangingLights.Length; i++)
		{
			((Renderer)m_HangingLights[i]).material.SetInt("_LightOn", 1);
			m_ActualLights[i].SetActive(true);
		}
		m_HauntedHouseLantern.TurnOn();
		for (int j = 0; j < m_BendyLandLights.Length; j++)
		{
			m_BendyLandLights[j].TurnOn();
		}
		m_BendyLandSupportLights[0].SetActive(true);
		m_BendyLandSupportLights[1].SetActive(true);
		m_BendyLandLights[m_BendyLandLights.Length - 1].TurnOff();
		for (int k = 0; k < m_FinalLight.Length; k++)
		{
			GameObject val = m_FinalLight[k];
			val.SetActive(false);
		}
		m_HauntedHouseTrigger.OnEnter += HandleHauntedHouseTriggerOnEnter;
		m_HauntedHouseTrigger.SetActive(active: true);
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_07");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_HAUNTED_HOUSE", "OBJECTIVES/CH4_OBJECTIVE_HAUNTED_HOUSE_TIP"));
		for (int i = 0; i < m_HangingLights.Length; i++)
		{
			MeshRenderer val = m_HangingLights[i];
			Material material = ((Renderer)val).material;
			GameObject val2 = m_ActualLights[i];
			material.SetInt("_LightOn", 1);
			val2.SetActive(true);
		}
		m_HauntedHouseLantern.TurnOn();
		for (int j = 0; j < m_BendyLandLights.Length; j++)
		{
			LightBulbController lightBulbController = m_BendyLandLights[j];
			lightBulbController.TurnOn();
		}
		m_BendyLandSupportLights[0].SetActive(true);
		m_BendyLandSupportLights[1].SetActive(true);
		LightBulbController lightBulbController2 = m_BendyLandLights[m_BendyLandLights.Length - 1];
		lightBulbController2.TurnOff();
		for (int k = 0; k < m_FinalLight.Length; k++)
		{
			GameObject val3 = m_FinalLight[k];
			val3.SetActive(false);
		}
		SendOnComplete();
	}

	private void HandleEntranceTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Expected O, but got Unknown
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Expected O, but got Unknown
		m_EntranceTrigger.OnEnter -= HandleEntranceTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_MusicWelcomeToBendyLand, AudioObjectType.MUSIC);
		S13AudioManager.Instance.PlayAudio("sfx_warehouse_turns_on");
		Sequence val = DOTween.Sequence();
		float num = 0f;
		for (int i = 0; i < m_HangingLights.Length; i++)
		{
			num = (float)i * 0.5f;
			MeshRenderer val2 = m_HangingLights[i];
			Material lightMaterial = ((Renderer)val2).material;
			GameObject actualLight = m_ActualLights[i];
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				lightMaterial.SetInt("_LightOn", 1);
				actualLight.SetActive(true);
			});
		}
		TweenSettingsExtensions.InsertCallback(val, 1.5f, new TweenCallback(m_HauntedHouseLantern.TurnOn));
		for (int num2 = 0; num2 < m_BendyLandLights.Length; num2++)
		{
			LightBulbController lightBulbController = m_BendyLandLights[num2];
			TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(lightBulbController.TurnOn));
			num = 1f + (float)num2 * 0.045f;
		}
		TweenSettingsExtensions.InsertCallback(val, 1.2f, (TweenCallback)delegate
		{
			m_BendyLandSupportLights[0].SetActive(true);
		});
		TweenSettingsExtensions.InsertCallback(val, 1.7f, (TweenCallback)delegate
		{
			m_BendyLandSupportLights[1].SetActive(true);
		});
		num += 0.25f;
		LightBulbController brokenBendyLandLight = m_BendyLandLights[m_BendyLandLights.Length - 1];
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(brokenBendyLandLight.TurnOff));
		TweenSettingsExtensions.InsertCallback(val, num + 0.15f, new TweenCallback(brokenBendyLandLight.TurnOn));
		TweenSettingsExtensions.InsertCallback(val, num + 0.3f, new TweenCallback(brokenBendyLandLight.TurnOff));
		TweenSettingsExtensions.InsertCallback(val, num + 0.5f, new TweenCallback(brokenBendyLandLight.TurnOn));
		TweenSettingsExtensions.InsertCallback(val, num + 0.6f, (TweenCallback)delegate
		{
			brokenBendyLandLight.TurnOff();
			for (int j = 0; j < m_FinalLight.Length; j++)
			{
				GameObject val3 = m_FinalLight[j];
				val3.SetActive(false);
			}
			m_BendySignSparks.Emit(10);
		});
		TweenSettingsExtensions.OnComplete<Sequence>(val, (TweenCallback)delegate
		{
			m_HauntedHouseTrigger.OnEnter += HandleHauntedHouseTriggerOnEnter;
			m_HauntedHouseTrigger.SetActive(active: true);
		});
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.WarehouseObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleHauntedHouseTriggerOnEnter(object sender, EventArgs e)
	{
		m_HauntedHouseTrigger.OnEnter -= HandleHauntedHouseTriggerOnEnter;
		int num = m_HenryHauntedHouseClips.Length;
		for (int i = 0; i < num; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryHauntedHouseClips[i], SubtitleConstants.DIALOGUE_CH4_HAUNTED_HOUSE[i], isTrimmed: true));
			if (i >= num - 1)
			{
				audioObject.OnComplete += delegate
				{
					GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_HAUNTED_HOUSE", "OBJECTIVES/CH4_OBJECTIVE_HAUNTED_HOUSE_TIP", 4f));
					SendOnComplete();
				};
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
