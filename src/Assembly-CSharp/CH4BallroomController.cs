using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4BallroomController : BaseController
{
	[Header("Boris")]
	[SerializeField]
	private BruteBorisAi m_Boris;

	[Header("[Ink Maker]")]
	[SerializeField]
	private InkMakerController m_InkMakerController;

	[SerializeField]
	private Holdable m_ThickInkPrefab;

	[Header("Scene")]
	[SerializeField]
	private GameObject m_DarknessSecondary;

	[SerializeField]
	private GenericDoorController m_Door;

	[SerializeField]
	private GameObject m_GoodDoor;

	[SerializeField]
	private GameObject m_BrokenDoor;

	[SerializeField]
	private Transform m_PlayerEndLocation;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject m_DestructableObjectPrefab;

	[SerializeField]
	private BrokenWeapon m_BrokenGentPrefab;

	[SerializeField]
	private BrokenWeapon m_BrokenPlungerPrefab;

	private GameObject m_DestructableInstance;

	private MeshDisablerController m_MeshDisabler;

	private Holdable m_ThickInk;

	private AudioClip m_GentBreakClip;

	private AudioClip m_PlungerBreakClip;

	private AudioClip m_MusicDeathOfAFriend;

	private AudioClip m_MusicDeathOfAFriendFinisher;

	private AudioObject m_MusicLoop;

	private AudioClip[] m_AliceDialogueClips;

	private AudioClip[] m_AliceFinaleClips;

	private bool m_HasPlunger;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AliceDialogueClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH4/Alice/BorisReveal");
		m_AliceFinaleClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH4/Alice/Finale");
		m_GentBreakClip = GameManager.Instance.GetAudioClip("Audio/SFX/Weapons/GentPipe/SFX_GentPipe_Pickup");
		m_PlungerBreakClip = GameManager.Instance.GetAudioClip("Audio/SFX/Weapons/Plunger/SFX_weapon_Plunger_Pickup");
		m_MusicDeathOfAFriend = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_DeathOfAFriend");
		m_MusicDeathOfAFriendFinisher = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_DeathOfAFriendFinisher");
		m_MeshDisabler = m_Boris.gameObject.AddComponent<MeshDisablerController>();
		m_MeshDisabler.SetSkinnedMeshEnabled(isEnabled: false);
		ResetDestructableObjects();
		m_DestructableObjectPrefab.SetActive(false);
		m_DarknessSecondary.SetActive(false);
		m_BrokenDoor.SetActive(false);
	}

	public override void Activate()
	{
		m_HasPlunger = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HasPlunger;
		if (m_HasPlunger)
		{
			m_InkMakerController.GivePlunger();
		}
		GameManager.Instance.CurrentChapter.DeathController.OnDeath += HandleDeathControllerOnDeath;
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned += HandleDeathControllerOnSpawned;
		m_Boris.OnBegin += HandleBorisOnBegin;
		m_Boris.OnCartSmashed += HandleBorisOnCartSmashed;
		m_Boris.OnDoorSmashed += HandleBorisOnDoorSmashed;
		m_Boris.OnTired += HandleBorisOnTired;
		m_Boris.OnHit += HandleBorisOnHit;
		m_Boris.OnDeath += HandleBorisOnDeath;
		m_Boris.OnComplete += HandleBorisOnComplete;
		m_Boris.DoRevealSequence();
		m_MeshDisabler.SetSkinnedMeshEnabled(isEnabled: true);
	}

	private void HandleBorisOnBegin(object sender, EventArgs e)
	{
		m_Boris.OnBegin -= HandleBorisOnBegin;
		m_DarknessSecondary.SetActive(true);
		m_Door.Close();
		m_MusicLoop = GameManager.Instance.AudioManager.Play(m_MusicDeathOfAFriend, AudioObjectType.MUSIC, -1);
		S13AudioManager.Instance.ToSnapshot("TMGAudioMixer", "mxs_alice_monologues", 1f);
		for (int i = 0; i < m_AliceDialogueClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceDialogueClips[i], SubtitleConstants.DIA_CH4_ALICE_BORIS_REVEAL[i], isTrimmed: true));
			if (i >= m_AliceDialogueClips.Length - 1)
			{
				audioObject.OnComplete += HandleAliceDialogueOnComplete;
			}
		}
	}

	private void HandleAliceDialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleAliceDialogueOnComplete;
		S13AudioManager.Instance.ToSnapshot("TMGAudioMixer", "mxs_base", 1f);
	}

	private void HandleBorisOnCartSmashed(object sender, EventArgs e)
	{
		m_Boris.OnCartSmashed -= HandleBorisOnCartSmashed;
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.REUNITED);
	}

	private void HandleBorisOnDoorSmashed(object sender, EventArgs e)
	{
		m_Boris.OnDoorSmashed -= HandleBorisOnDoorSmashed;
		m_GoodDoor.SetActive(false);
		m_BrokenDoor.SetActive(true);
	}

	private void HandleBorisOnTired(object sender, EventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		if (!m_Boris.CanSpawnInk)
		{
			return;
		}
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(m_Boris.transform.position, m_Boris.transform.forward, ref val, 10f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				Physics.Raycast(((RaycastHit)(ref val)).point, Vector3.down, ref val, 50f, LayerMask.GetMask(new string[1] { "Default" }));
			}
			else
			{
				Physics.Raycast(m_Boris.transform.position + m_Boris.transform.forward * 10f + Vector3.up * 5f, Vector3.down, ref val, 50f, LayerMask.GetMask(new string[1] { "Default" }));
			}
			if (!Object.op_Implicit((Object)(object)m_ThickInk))
			{
				m_ThickInk = Object.Instantiate<Holdable>(m_ThickInkPrefab, ((RaycastHit)(ref val)).point, Quaternion.identity);
				m_ThickInk.OnHeld += HandleThickInkOnHeld;
			}
			else if (Object.op_Implicit((Object)(object)m_ThickInk) && !Object.op_Implicit((Object)(object)m_ThickInk.transform.parent))
			{
				m_ThickInk.transform.position = ((RaycastHit)(ref val)).point;
			}
		});
	}

	private void HandleBorisOnHit(object sender, EventArgs e)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.AudioManager.Play((!m_HasPlunger) ? m_GentBreakClip : m_PlungerBreakClip);
		PlayerController player = GameManager.Instance.Player;
		if (Object.op_Implicit((Object)(object)player.WeaponGameObject) && Object.op_Implicit((Object)(object)player.WeaponGameObject.GetComponent<MeleeWeapon>()))
		{
			BrokenWeapon brokenWeapon = Object.Instantiate<BrokenWeapon>((!m_HasPlunger) ? m_BrokenGentPrefab : m_BrokenPlungerPrefab);
			brokenWeapon.Break(player.WeaponGameObject.transform, player.transform.position);
			Object.Destroy((Object)(object)player.WeaponGameObject);
			if (Object.op_Implicit((Object)(object)player.InactiveWeapon))
			{
				player.WeaponGameObject = player.InactiveWeapon;
				player.WeaponGameObject.SetActive(true);
				player.InactiveWeapon = null;
			}
			else
			{
				player.UnEquipWeapon();
			}
		}
	}

	private void HandleBorisOnDeath(object sender, EventArgs e)
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		m_Boris.OnDeath -= HandleBorisOnDeath;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HasPlunger)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.UNLIKELY_VICTORY);
		}
		if (Object.op_Implicit((Object)(object)m_MusicLoop))
		{
			m_MusicLoop.Stop();
			m_MusicLoop = null;
		}
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			GameManager.Instance.Player.WeaponGameObject.SetActive(false);
		}
		ClearThickInk();
		GameManager.Instance.AudioManager.Play(m_MusicDeathOfAFriendFinisher, AudioObjectType.MUSIC);
		for (int i = 0; i < m_AliceFinaleClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceFinaleClips[i], SubtitleConstants.DIA_CH4_ALICE_FINALE[i], isTrimmed: true));
		}
		GameManager.Instance.LockPause();
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.SetCameraSway(active: true);
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		Vector3 position = m_Boris.transform.position;
		position -= m_Boris.transform.forward * 5f;
		Sequence val2 = DOTween.Sequence();
		float num = 0.5f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, m_PlayerEndLocation.position, 2f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_PlayerEndLocation.eulerAngles, 2f, (RotateMode)0), (Ease)7));
		num += 2f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(val, position, 2f, (AxisConstraint)0, (Vector3?)null), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(val2, new TweenCallback(base.SendOnComplete));
	}

	private void HandleBorisOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnComplete -= HandleBorisOnComplete;
		((Behaviour)m_Boris).enabled = false;
		m_Door.ForceOpen();
	}

	private void HandleThickInkOnHeld(object sender, EventArgs e)
	{
		m_ThickInk.OnHeld -= HandleThickInkOnHeld;
		m_Boris.CanSpawnInk = false;
		m_InkMakerController.OnTrayInteracted += HandleInkMakerOnTrayInteracted;
		m_InkMakerController.OnComplete += HandleInkMakerOnComplete;
		m_InkMakerController.Activate();
	}

	private void HandleInkMakerOnTrayInteracted(object sender, EventArgs e)
	{
		m_InkMakerController.OnTrayInteracted -= HandleInkMakerOnTrayInteracted;
		ClearThickInk();
	}

	private void HandleInkMakerOnComplete(object sender, EventArgs e)
	{
		m_InkMakerController.OnComplete -= HandleInkMakerOnComplete;
		m_Boris.CanSpawnInk = true;
		m_InkMakerController.Deactivate();
	}

	private void ClearThickInk()
	{
		if (Object.op_Implicit((Object)(object)m_ThickInk))
		{
			m_ThickInk.Remove();
			m_ThickInk.Dispose();
		}
	}

	private void ResetDestructableObjects()
	{
		if (Object.op_Implicit((Object)(object)m_DestructableInstance))
		{
			Object.Destroy((Object)(object)m_DestructableInstance);
		}
		m_DestructableInstance = Object.Instantiate<GameObject>(m_DestructableObjectPrefab);
		m_DestructableInstance.SetActive(true);
		Broken_Mesh_Dissolve[] array = Object.FindObjectsOfType<Broken_Mesh_Dissolve>();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			if (array[num].gameObject.activeSelf)
			{
				array[num].Dispose();
			}
		}
		m_GoodDoor.SetActive(true);
		m_BrokenDoor.SetActive(false);
	}

	private void HandleDeathControllerOnDeath(object sender, EventArgs e)
	{
		if (Object.op_Implicit((Object)(object)m_ThickInk))
		{
			m_ThickInk.Remove();
			m_ThickInk.Dispose();
		}
		PlayerController player = GameManager.Instance.Player;
		if (Object.op_Implicit((Object)(object)player.WeaponGameObject))
		{
			Object.Destroy((Object)(object)player.WeaponGameObject);
		}
		if (Object.op_Implicit((Object)(object)player.InactiveWeapon))
		{
			Object.Destroy((Object)(object)player.InactiveWeapon);
		}
		player.UnEquipWeapon();
		m_Boris.Reset();
		ResetDestructableObjects();
	}

	private void HandleDeathControllerOnSpawned(object sender, EventArgs e)
	{
		m_Boris.WarpToStartLocation();
		m_Boris.SetThought(BruteBorisAi.BorisThought.PHASE1);
		m_Boris.OnDoorSmashed += HandleBorisOnDoorSmashed;
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.CurrentChapter.DeathController.OnDeath -= HandleDeathControllerOnDeath;
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandleDeathControllerOnSpawned;
		if (Object.op_Implicit((Object)(object)m_Boris))
		{
			m_Boris.OnBegin -= HandleBorisOnBegin;
			m_Boris.OnCartSmashed -= HandleBorisOnCartSmashed;
			m_Boris.OnDoorSmashed -= HandleBorisOnDoorSmashed;
			m_Boris.OnTired -= HandleBorisOnTired;
			m_Boris.OnHit -= HandleBorisOnHit;
			m_Boris.OnDeath -= HandleBorisOnDeath;
			m_Boris.OnComplete -= HandleBorisOnComplete;
		}
		base.OnDisposed();
	}
}
