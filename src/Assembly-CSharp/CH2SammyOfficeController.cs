using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using S13Audio;
using UnityEngine;

public class CH2SammyOfficeController : BaseController
{
	[Header("Controllers")]
	[SerializeField]
	private MeatlyController m_MeatlyController;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_KnockoutPosition;

	[Header("Interactables")]
	[SerializeField]
	private InteractablePowerLever m_Lever;

	[Header("Pipes")]
	[SerializeField]
	private List<InkPipeController> m_Pipes;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_KnockoutEventTrigger;

	[SerializeField]
	private EventTrigger m_WindowEventTrigger;

	[Header("Sammy")]
	[SerializeField]
	private GameObject m_Sammy;

	[Header("Renderer")]
	[SerializeField]
	private MeshRenderer m_LeverMeshRenderer;

	[Header("Searcher Spawners")]
	[SerializeField]
	private GameObject[] m_SearcherSpawners;

	[SerializeField]
	private GameObject m_SearcherB;

	private AudioClip m_KnockoutClip;

	private AudioClip m_LeverTurn;

	private AudioClip m_HenryClip09;

	public override void Init()
	{
		base.Init();
		for (int i = 0; i < m_Pipes.Count; i++)
		{
			m_Pipes[i].TurnOff();
		}
		m_Sammy.SetActive(false);
	}

	public override void InitOnComplete()
	{
		m_KnockoutClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Henry_HitOnHead");
		m_LeverTurn = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_HenryClip09 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_09");
		GameManager.Instance.Player.OnDeath += HandleOnPlayerDeath;
		for (int i = 0; i < m_Pipes.Count; i++)
		{
			m_Pipes[i].TurnOff();
		}
		m_Sammy.SetActive(false);
		m_KnockoutEventTrigger.SetActive(active: false);
		m_Lever.SetActive(active: false);
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH2Data.LostKeysObjective.IsStarted)
		{
			m_WindowEventTrigger.OnEnter += HandleWindowEventTriggerOnEnter;
		}
	}

	private void HandleWindowEventTriggerOnEnter(object sender, EventArgs e)
	{
		if (((Renderer)m_LeverMeshRenderer).isVisible)
		{
			m_WindowEventTrigger.OnEnter -= HandleWindowEventTriggerOnEnter;
			m_WindowEventTrigger.Dispose();
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip09, "DIACH2/DIA_CH2_HENRY_09"));
		}
	}

	public override void Activate()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_SAMMYS_OFFICE", string.Empty));
		m_Lever.OnInteracted += HandleLeverOnInteracted;
		m_Lever.OnComplete += HandleLeverOnComplete;
		m_Lever.SetActive(active: true);
		if (Object.op_Implicit((Object)(object)m_SearcherB) && !GameManager.Instance.GameData.CurrentSaveFile.CH2Data.HasDied && GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InternecionValue == 0)
		{
			m_SearcherB.SetActive(true);
			BaseAiController component = m_SearcherB.GetComponent<BaseAiController>();
			component.OnDeath += HandleSearcherBOnDeath;
		}
	}

	private void HandleSearcherBOnDeath(object sender, EventArgs e)
	{
		(sender as BaseAiController).OnDeath -= HandleSearcherBOnDeath;
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InternecionValue = -1;
		GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
	}

	public void HandleOnPlayerDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnDeath -= HandleOnPlayerDeath;
		if (Object.op_Implicit((Object)(object)m_SearcherB))
		{
			m_SearcherB.SetActive(false);
		}
	}

	private void ForceComplete()
	{
		m_MeatlyController.Activate();
		SendOnComplete();
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
		GameManager.Instance.AudioManager.Play(m_LeverTurn);
		S13AudioManager.Instance.InvokeEvent("evt_office_lever_thrown");
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SammysOfficeObjective.IsComplete)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SammysOfficeObjective.IsComplete = true;
			GameManager.Instance.GameDataManager.Save();
		}
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		m_Lever.OnComplete -= HandleLeverOnComplete;
		for (int i = 0; i < m_SearcherSpawners.Length; i++)
		{
			m_SearcherSpawners[i].SetActive(false);
		}
		m_MeatlyController.Activate();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_TAKE_STAIRS", string.Empty, 4f));
		for (int j = 0; j < m_Pipes.Count; j++)
		{
			m_Pipes[j].TurnOn();
		}
		m_KnockoutEventTrigger.SetActive(active: true);
		m_KnockoutEventTrigger.OnEnter += HandleKnockoutEventTriggerOnEnter;
		if (Object.op_Implicit((Object)(object)m_SearcherB) && m_SearcherB.activeSelf)
		{
			m_SearcherB.GetComponent<BaseAiController>().SetThought(AiThought.Die);
		}
	}

	private void HandleKnockoutEventTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		m_KnockoutEventTrigger.OnEnter -= HandleKnockoutEventTriggerOnEnter;
		m_KnockoutEventTrigger.Dispose();
		GameManager.Instance.AudioManager.Play(m_KnockoutClip);
		S13AudioManager.Instance.InvokeEvent("evt_sammy_knocks_out_player");
		TweenSettingsExtensions.OnComplete<Sequence>(DOKnockout(), new TweenCallback(base.SendOnComplete));
	}

	private Sequence DOKnockout()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Expected O, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Expected O, but got Unknown
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected O, but got Unknown
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Expected O, but got Unknown
		ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 3f, 0.25f, 12, 90f, false, true);
		GameManager.Instance.Player.SetLock(active: true);
		GameManager.Instance.Player.UnEquipWeapon();
		Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
		GameManager.Instance.HideCrosshair();
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, 0f, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(0f);
		});
		GameCamera gameCam = GameManager.Instance.GameCamera;
		if (gameCam.DoF)
		{
			TweenSettingsExtensions.InsertCallback(val, 0f, (TweenCallback)delegate
			{
				gameCam.UnityDOF.manualDOF = true;
				gameCam.UnityDOF.focalDistance = 0f;
			});
			float blurOff = 0f;
			TweenSettingsExtensions.Insert(val, 1.5f, (Tween)(object)TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => blurOff), (DOSetter<float>)delegate(float value)
			{
				blurOff = value;
			}, 10f, 1f), (Ease)1), (TweenCallback)delegate
			{
				gameCam.UnityDOF.focalDistance = blurOff;
			}));
			float blurOn = 10f;
			TweenSettingsExtensions.Insert(val, 2.5f, (Tween)(object)TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => blurOn), (DOSetter<float>)delegate(float value)
			{
				blurOn = value;
			}, 0f, 1f), (Ease)1), (TweenCallback)delegate
			{
				gameCam.UnityDOF.focalDistance = blurOn;
			}));
		}
		TweenSettingsExtensions.InsertCallback(val, 0.1f, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(1.5f);
		});
		TweenSettingsExtensions.InsertCallback(val, 2f, (TweenCallback)delegate
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.ShowScreenBlocker();
			ShortcutExtensions.DOMove(((Component)GameManager.Instance.GameCamera.Camera).transform, m_KnockoutPosition.position, 1f, false);
			ShortcutExtensions.DORotateQuaternion(((Component)GameManager.Instance.GameCamera.Camera).transform, m_KnockoutPosition.rotation, 1f);
		});
		TweenSettingsExtensions.InsertCallback(val, 3.5f, (TweenCallback)delegate
		{
			m_Sammy.SetActive(true);
			GameManager.Instance.HideScreenBlocker(1.5f);
		});
		if (gameCam.DoF)
		{
			TweenSettingsExtensions.InsertCallback(val, 0f, (TweenCallback)delegate
			{
				gameCam.UnityDOF.manualDOF = true;
				gameCam.UnityDOF.focalDistance = 0f;
			});
			float blurOff2 = 0f;
			TweenSettingsExtensions.Insert(val, 6f, (Tween)(object)TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => blurOff2), (DOSetter<float>)delegate(float value)
			{
				blurOff2 = value;
			}, 10f, 1f), (Ease)1), (TweenCallback)delegate
			{
				gameCam.UnityDOF.focalDistance = blurOff2;
			}));
			float blurOn2 = 10f;
			TweenSettingsExtensions.Insert(val, 7f, (Tween)(object)TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => blurOn2), (DOSetter<float>)delegate(float value)
			{
				blurOn2 = value;
			}, 0f, 0.5f), (Ease)1), (TweenCallback)delegate
			{
				gameCam.UnityDOF.focalDistance = blurOn2;
			}));
		}
		TweenSettingsExtensions.InsertCallback(val, 7.3f, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(0.25f);
		});
		TweenSettingsExtensions.InsertCallback(val, 10f, (TweenCallback)delegate
		{
		});
		return val;
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_KnockoutEventTrigger != (Object)null)
		{
			m_KnockoutEventTrigger.OnEnter -= HandleKnockoutEventTriggerOnEnter;
		}
		if ((Object)(object)m_WindowEventTrigger != (Object)null)
		{
			m_WindowEventTrigger.OnEnter -= HandleWindowEventTriggerOnEnter;
		}
		m_KnockoutClip = null;
		m_LeverTurn = null;
		m_HenryClip09 = null;
		base.OnDisposed();
	}
}
