using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH2BendyChaseController : BaseController
{
	private const string CUTOUT = "_Cutout";

	[Header("Transforms")]
	[SerializeField]
	private Transform m_PlankBarricade;

	[SerializeField]
	private Transform m_BendyPoolSpawnPoint;

	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_BreakablePlankBlockage;

	[SerializeField]
	private List<GameObject> m_ActiveGameObjects;

	[SerializeField]
	private List<GameObject> m_DisableGameObjects;

	[SerializeField]
	private List<Breakable> m_Planks;

	[SerializeField]
	private BrokenWeapon m_BrokenAxe;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_BendyTrigger;

	[SerializeField]
	private EventTrigger m_BarricadeTrigger;

	[SerializeField]
	private EventTrigger m_BendyPoolTrigger;

	[Header("Materials")]
	[SerializeField]
	private Material[] m_DissolveMaterials;

	[Header("Doors")]
	[SerializeField]
	private BaseDoorController m_DoorController;

	[Header("Light Fixture")]
	[SerializeField]
	private LightFixtureController m_LightController;

	[Header("Bendy")]
	[SerializeField]
	private BendyAi m_Bendy;

	[SerializeField]
	private MeshRenderer m_BendyRenderer;

	[Header("Spawners")]
	[SerializeField]
	private PlayerSpawnNode m_SacrificeSpawner;

	[SerializeField]
	private PlayerSpawnNode m_BendySpawner;

	[Header("Spawners")]
	[SerializeField]
	private RiverWaves m_WaveA;

	[SerializeField]
	private RiverWaves m_WaveB;

	[Header("Bendy Prefab (i know, just bandaids right now!)")]
	[SerializeField]
	private BendyAi m_BendyPrefab;

	[Header("Spawners")]
	[SerializeField]
	private GameObject[] m_DisableSpawners;

	[SerializeField]
	private GameObject[] m_EnableSpawners;

	private AudioObject m_MusicAudioObject;

	private AudioClip[] m_BendyFootstepClips;

	private AudioClip m_BendyRevealClip;

	private AudioClip m_LittleDevilMusic;

	private AudioClip m_CeilingCollapseClip;

	private AudioClip m_CelingSettleClip;

	private AudioClip m_DoorSlamClip;

	private AudioClip m_BendyAtDoorClip;

	private AudioClip m_HorrorCueClip;

	private AudioClip m_AxePickupClip;

	private bool m_IsClose;

	private bool m_IsBendyGone;

	private RiverWaves m_ActiveWave;

	public override void InitOnComplete()
	{
		for (int i = 0; i < m_EnableSpawners.Length; i++)
		{
			m_EnableSpawners[i].SetActive(false);
		}
		m_BendyTrigger.SetActive(active: false);
		m_BarricadeTrigger.SetActive(active: false);
		m_BendyPoolTrigger.SetActive(active: false);
		m_Bendy.gameObject.SetActive(false);
		m_BendyFootstepClips = GameManager.Instance.GetAudioClips("Audio/SFX/Footsteps/Bendy/Loud");
		m_BendyRevealClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_BendyAppearsFromInk");
		m_LittleDevilMusic = GameManager.Instance.GetAudioClip("Audio/MUS/MUS_Little_Devil_Darling_Remastered");
		m_CeilingCollapseClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Ceiling_Collapse_02");
		m_CelingSettleClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Ceiling_Settle_02");
		m_DoorSlamClip = GameManager.Instance.GetAudioClip("Audio/SFX/Door/SFX_Door_Slam_01");
		m_BendyAtDoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_BendyAtTheDoor");
		m_HorrorCueClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_Horror_Cue_02");
		m_AxePickupClip = GameManager.Instance.GetAudioClip("Audio/SFX/Weapons/Axe/SFX_Axe_Pick_Up_01");
		m_ActiveWave = m_WaveA;
		for (int j = 0; j < m_DisableGameObjects.Count; j++)
		{
			m_DisableGameObjects[j].SetActive(false);
		}
		for (int k = 0; k < m_ActiveGameObjects.Count; k++)
		{
			m_ActiveGameObjects[k].SetActive(true);
		}
		for (int l = 0; l < m_DissolveMaterials.Length; l++)
		{
			Material val = m_DissolveMaterials[l];
			val.SetFloat("_Cutout", 0f);
		}
	}

	public override void Activate()
	{
		for (int i = 0; i < m_Planks.Count; i++)
		{
			m_Planks[i].OnBroken += HandlePlankOnBroken;
		}
		m_BendyTrigger.OnEnter += HandleBendyOnEntered;
		m_BendyTrigger.SetActive(active: true);
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		m_Bendy.ForceKill();
		S13AudioManager.Instance.ToSnapshot("TMGAudioMixer", "mxs_base", 3f);
		for (int i = 0; i < m_DissolveMaterials.Length; i++)
		{
			Material val = m_DissolveMaterials[i];
			val.SetFloat("_Cutout", 0f);
		}
		if (Object.op_Implicit((Object)(object)m_MusicAudioObject))
		{
			m_MusicAudioObject.Clear();
			m_MusicAudioObject = null;
		}
		m_ActiveWave = m_WaveB;
		m_Bendy = Object.Instantiate<BendyAi>(m_BendyPrefab);
		m_Bendy.transform.position = m_BendyPoolSpawnPoint.position;
		m_Bendy.gameObject.SetActive(false);
		m_BendyPoolTrigger.OnEnter += HandleBendyPoolSpawnTriggerOnEnter;
		m_BendyPoolTrigger.ResetTrigger();
		m_BendyPoolTrigger.SetActive(active: true);
	}

	private void HandleBendyPoolSpawnTriggerOnEnter(object sender, EventArgs e)
	{
		m_BendyPoolTrigger.OnEnter -= HandleBendyPoolSpawnTriggerOnEnter;
		m_BendyPoolTrigger.SetActive(active: false);
		BendyReveal();
	}

	private void HandlePlankOnBroken(object sender, EventArgs e)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		Breakable item = (Breakable)sender;
		if (m_Planks.Contains(item))
		{
			m_Planks.Remove(item);
		}
		if (m_Planks.Count <= 0)
		{
			m_BreakablePlankBlockage.SetActive(false);
			GameManager.Instance.AudioManager.Play(m_AxePickupClip);
			BrokenWeapon brokenWeapon = Object.Instantiate<BrokenWeapon>(m_BrokenAxe);
			brokenWeapon.Break(GameManager.Instance.Player.WeaponGameObject.transform, GameManager.Instance.Player.transform.position);
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
			GameManager.Instance.Player.UnEquipWeapon();
			SearcherAi[] array = Resources.FindObjectsOfTypeAll<SearcherAi>();
			foreach (SearcherAi searcherAi in array)
			{
				searcherAi.SetThought(AiThought.Retreat);
			}
		}
	}

	private void HandleBendyOnEntered(object sender, EventArgs e)
	{
		if (((Renderer)m_BendyRenderer).isVisible)
		{
			m_BendyTrigger.OnEnter -= HandleBendyOnEntered;
			m_BendyTrigger.Dispose();
			for (int i = 0; i < m_DisableGameObjects.Count; i++)
			{
				m_DisableGameObjects[i].SetActive(true);
			}
			for (int j = 0; j < m_ActiveGameObjects.Count; j++)
			{
				m_ActiveGameObjects[j].SetActive(false);
			}
			for (int k = 0; k < m_DisableSpawners.Length; k++)
			{
				m_DisableSpawners[k].SetActive(false);
			}
			for (int l = 0; l < m_EnableSpawners.Length; l++)
			{
				m_EnableSpawners[l].SetActive(true);
			}
			GameManager.Instance.AudioManager.Play(m_CeilingCollapseClip);
			GameManager.Instance.AudioManager.Play(m_CelingSettleClip);
			BendyReveal();
			m_DoorController.Open(0f, (Ease)1, -125f);
			m_BarricadeTrigger.SetActive(active: true);
			m_BarricadeTrigger.OnEnter += HandleBarricadeTriggerOnEnter;
			GameManager.Instance.CurrentChapter.DeathController.OnDeath += HandlePlayerOnDeath;
		}
	}

	private void BendyReveal()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		m_Bendy.gameObject.SetActive(true);
		m_Bendy.ForceSetAnimationTrigger("EnemySpotted");
		m_Bendy.SetTarget(GameManager.Instance.Player.transform);
		m_Bendy.ForceFaceDirection();
		m_ActiveWave.AddWave(m_Bendy.transform.position, 7f, 0.04f, 15f);
		S13AudioManager.Instance.ToSnapshot("TMGAudioMixer", "mxs_bendy_overload", 0.2f);
		GameManager.Instance.AudioManager.Play(m_BendyRevealClip);
		m_MusicAudioObject = GameManager.Instance.AudioManager.Play(m_LittleDevilMusic, AudioObjectType.MUSIC, -1);
		m_MusicAudioObject.AudioSource.volume = 0f;
		m_MusicAudioObject.AudioSource.DOFade(1f, 1.5f);
		for (int i = 0; i < m_DissolveMaterials.Length; i++)
		{
			Material val = m_DissolveMaterials[i];
			ShortcutExtensions.DOFloat(val, 0.5f, "_Cutout", 5f);
		}
	}

	private void HandleBarricadeTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		if (Object.op_Implicit((Object)(object)GameManager.Instance.CurrentChapter.DeathController))
		{
			GameManager.Instance.CurrentChapter.DeathController.OnDeath -= HandlePlayerOnDeath;
		}
		m_BarricadeTrigger.OnEnter -= HandleBarricadeTriggerOnEnter;
		m_Bendy.ForceKill();
		GameManager.Instance.Player.SetRun(active: false);
		GameManager.Instance.AudioManager.Play(m_DoorSlamClip);
		m_DoorController.OnClose += HandleDoorCloseOnComplete;
		m_DoorController.Close(0.25f, (Ease)1);
		m_DoorController.Lock();
		GameManager.Instance.AudioManager.PlayAtPosition(m_BendyAtDoorClip, m_DoorController.transform.position);
		S13AudioManager.Instance.ToSnapshot("TMGAudioMixer", "mxs_base", 3f);
		if (!((Object)(object)m_MusicAudioObject != (Object)null))
		{
			return;
		}
		TweenSettingsExtensions.OnComplete<Tweener>(m_MusicAudioObject.AudioSource.DOFade(0f, 3f), (TweenCallback)delegate
		{
			if ((Object)(object)m_MusicAudioObject != (Object)null)
			{
				m_MusicAudioObject.Clear();
				m_MusicAudioObject = null;
			}
		});
	}

	private void HandleDoorCloseOnComplete(object sender, EventArgs e)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		GameManager.Instance.AudioManager.Play(m_HorrorCueClip);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_PlankBarricade, new Vector3(0f, 0f, 70f), 0.75f, (RotateMode)0), (Ease)30), (TweenCallback)delegate
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_BELIEVER);
			SendOnComplete();
		});
	}

	private void PlayFootStepAudio()
	{
		if (m_BendyFootstepClips != null && m_BendyFootstepClips.Length > 0)
		{
			int num = Random.Range(0, m_BendyFootstepClips.Length);
			AudioClip val = m_BendyFootstepClips[num];
			ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.25f, 0.15f, 12, 90f, false, true);
			GameManager.Instance.AudioManager.Play(val);
			m_BendyFootstepClips[num] = m_BendyFootstepClips[0];
			m_BendyFootstepClips[0] = val;
		}
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_BendyTrigger))
		{
			m_BendyTrigger.OnEnter -= HandleBendyOnEntered;
		}
		m_MusicAudioObject = null;
		m_BendyFootstepClips = null;
		m_BendyRevealClip = null;
		m_LittleDevilMusic = null;
		m_CeilingCollapseClip = null;
		m_CelingSettleClip = null;
		m_DoorSlamClip = null;
		m_BendyAtDoorClip = null;
		m_HorrorCueClip = null;
		m_AxePickupClip = null;
		m_EnableSpawners = null;
		m_DisableSpawners = null;
		base.OnDisposed();
	}
}
