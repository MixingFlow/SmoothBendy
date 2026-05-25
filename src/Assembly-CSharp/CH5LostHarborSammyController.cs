using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH5LostHarborSammyController : BaseController
{
	[SerializeField]
	private SammyLawrence_Ai m_SammyAi;

	[SerializeField]
	private List<Breakable> m_Planks;

	[SerializeField]
	private WeaponInfo m_BoardBreakWeaponInfo;

	[SerializeField]
	private Transform m_SammyMoveToPoint;

	[SerializeField]
	private EventTrigger m_ActivateSammyTrigger;

	[SerializeField]
	private EventTrigger m_DeathTrigger;

	[SerializeField]
	private Transform m_SammyNoMaskPosition;

	[SerializeField]
	private WaypointList m_EndingWaypoints;

	[SerializeField]
	private AllyAiController m_Tom;

	[SerializeField]
	private GameObject m_TomAxe;

	[SerializeField]
	private GameObject m_TomGent;

	[SerializeField]
	private AllyAiController m_Allison;

	[SerializeField]
	private Transform m_HenryEndLocation;

	[SerializeField]
	private GameObject m_SammyAxe;

	[SerializeField]
	private MeleeWeapon m_Axe;

	private LayerMask m_AttackLayers;

	private AudioClip[] m_IntroClips;

	private AudioClip[] m_DeathClips;

	private AudioClip m_NoMaskClip;

	private AudioClip m_JumpscareClip;

	private AudioClip m_NakedMusic;

	private AudioClip m_SongwriterMusic;

	private AudioObject m_MusicObject;

	private int m_BrokenPlanksCount;

	public override void Init()
	{
		base.Init();
		m_Tom.gameObject.SetActive(false);
		m_Allison.gameObject.SetActive(false);
	}

	public override void InitOnComplete()
	{
		m_BrokenPlanksCount = m_Planks.Count;
		m_DeathTrigger.SetActive(active: false);
		m_ActivateSammyTrigger.SetActive(active: false);
		m_IntroClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Sammy/Intro");
		m_DeathClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Sammy/Death");
		m_JumpscareClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_SammyJumpscare");
		m_NoMaskClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/Sammy/DIA_CH5_SAMMY_NO_MASK");
		m_NakedMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_NakedAndAfraid");
		m_SongwriterMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_ASongwriterScorned");
	}

	public void ForceComplete()
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.Player.GoToAndLookAt(m_HenryEndLocation);
		for (int i = 0; i < m_Planks.Count; i++)
		{
			m_Planks[i].Dispose();
		}
		m_SammyAi.Dispose();
		m_Tom.gameObject.SetActive(true);
		Transform val = m_EndingWaypoints.Waypoints[m_EndingWaypoints.Waypoints.Count - 1].transform;
		m_Tom.transform.position = val.position;
		m_Tom.transform.rotation = val.rotation;
		m_TomAxe.SetActive(true);
		m_TomGent.SetActive(false);
		m_Tom.ForceStartIdle();
		m_Tom.EnterCombat();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_Tom.SetThought(AiThought.Idle);
		});
		m_Allison.gameObject.SetActive(true);
		m_Allison.EnterCombat();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_Allison.SetThought(AiThought.Idle);
		});
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			GameManager.Instance.Player.InactiveWeapon = GameManager.Instance.Player.WeaponGameObject;
			GameManager.Instance.Player.WeaponGameObject.SetActive(false);
		}
		MeleeWeapon meleeWeapon = Object.Instantiate<MeleeWeapon>(m_Axe);
		GameManager.Instance.Player.WeaponGameObject = meleeWeapon.gameObject;
		GameManager.Instance.Player.EquipWeapon();
		if (Object.op_Implicit((Object)(object)meleeWeapon) && (Object)(object)meleeWeapon.Interaction != (Object)null)
		{
			meleeWeapon.Interaction.SetActive(active: false);
		}
		meleeWeapon.KillInteraction();
		meleeWeapon.Equip();
		meleeWeapon.transform.SetParent(GameManager.Instance.Player.WeaponParent);
		meleeWeapon.transform.localPosition = Vector3.zero;
		meleeWeapon.transform.localEulerAngles = Vector3.zero;
	}

	public override void Activate()
	{
		m_SammyAi.OnBreakPlank += HandleSammyOnBreakPlank;
		m_SammyAi.OnRemoveMask += HandleSammyOnRemoveMask;
		m_SammyAi.OnGetAxe += HandleOnGetAxe;
		m_ActivateSammyTrigger.OnEnter += ActivateSammy;
		m_ActivateSammyTrigger.SetActive(active: true);
	}

	public void ActivateSammy(object sender, EventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		m_ActivateSammyTrigger.OnEnter -= ActivateSammy;
		GameManager.Instance.AudioManager.Play(m_JumpscareClip);
		m_SammyAi.SetVectorPoint(m_SammyMoveToPoint.position);
		m_SammyAi.SetThought(AiThought.Activate);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
		{
			for (int i = 0; i < m_IntroClips.Length; i++)
			{
				AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_IntroClips[i], SubtitleConstants.DIA_CH5_SAMMY_INTRO[i], isTrimmed: true));
				if (i == m_IntroClips.Length - 1)
				{
					audioObject.OnComplete += delegate
					{
						m_SammyAi.EnableAttackAudio();
					};
				}
			}
		});
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_SongwriterMusic, AudioObjectType.MUSIC, -1);
	}

	private void HandleSammyOnBreakPlank(object sender, EventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit hit = default(RaycastHit);
		((RaycastHit)(ref hit)).point = m_SammyAi.transform.position;
		for (int i = 0; i < m_Planks.Count; i++)
		{
			m_Planks[0].Hit(hit, m_BoardBreakWeaponInfo);
			m_Planks.RemoveAt(0);
			m_BrokenPlanksCount--;
		}
	}

	private void HandleSammyOnRemoveMask(object sender, EventArgs e)
	{
		m_SammyAi.OnRemoveMask -= HandleSammyOnRemoveMask;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_NoMaskClip, "DIACH5/DIA_CH5_SAMMY_NO_MASK", isTrimmed: true));
		m_SammyAi.OnWaypointComplete += HandleSammyOnWaypointComplete;
		m_SammyAi.UpdateWaypointList(m_EndingWaypoints.Waypoints);
	}

	private void HandleSammyOnWaypointComplete(object sender, EventArgs e)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		m_SammyAi.OnWaypointComplete += HandleSammyOnWaypointComplete;
		m_SammyAi.transform.rotation = m_SammyNoMaskPosition.rotation;
		m_SammyAi.SetInactive();
		m_DeathTrigger.OnEnter += HandleDeathTriggerOnEnter;
		m_DeathTrigger.SetActive(active: true);
	}

	private void HandleDeathTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		m_DeathTrigger.OnEnter -= HandleDeathTriggerOnEnter;
		GameManager.Instance.HideCrosshair();
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
		}
		GameManager.Instance.Player.UnEquipWeapon();
		m_Tom.transform.position = m_SammyAi.transform.position;
		m_Tom.transform.rotation = m_SammyAi.transform.rotation;
		m_Tom.gameObject.SetActive(true);
		m_SammyAi.TriggerDeath();
		GameManager.Instance.Player.GoToAndLookAt(m_HenryEndLocation);
		for (int i = 0; i < m_DeathClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_DeathClips[i], SubtitleConstants.DIA_CH5_SAMMY_DEATH[i], isTrimmed: true));
		}
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 1f), (Ease)1), (TweenCallback)delegate
			{
				if (Object.op_Implicit((Object)(object)m_MusicObject))
				{
					m_MusicObject.Clear();
					m_MusicObject = null;
				}
			});
		}
		GameManager.Instance.AudioManager.Play(m_NakedMusic, AudioObjectType.MUSIC);
	}

	private void HandleOnGetAxe(object sender, EventArgs e)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Expected O, but got Unknown
		m_SammyAi.OnGetAxe += HandleOnGetAxe;
		Transform originalAxeParent = m_TomAxe.transform.parent;
		Vector3 originalAxePosition = m_TomAxe.transform.localPosition;
		Vector3 originalAxeEuler = m_TomAxe.transform.localEulerAngles;
		Transform axeParent = new GameObject().transform;
		axeParent.position = m_TomAxe.transform.position;
		axeParent.eulerAngles = new Vector3(170f, 200f, 65f);
		m_TomAxe.transform.SetParent(axeParent);
		Vector3 val = GameManager.Instance.GameCamera.transform.position + Vector3.down * 3f;
		Sequence val2 = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(axeParent, val, 0.5f, false), (Ease)6));
		TweenSettingsExtensions.InsertCallback(val2, num, (TweenCallback)delegate
		{
			m_Allison.gameObject.SetActive(true);
		});
		num += 1f;
		TweenSettingsExtensions.InsertCallback(val2, num, (TweenCallback)delegate
		{
			((Component)axeParent).gameObject.SetActive(false);
		});
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(GameManager.Instance.GameCamera.FreeRoamCam, GameManager.Instance.Player.HeadContainer.position, 1f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(GameManager.Instance.GameCamera.FreeRoamCam, GameManager.Instance.Player.HeadContainer.eulerAngles, 1f, (RotateMode)0), (Ease)7));
		num += 1f;
		TweenSettingsExtensions.InsertCallback(val2, num, (TweenCallback)delegate
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
			m_SammyAxe.SetActive(false);
			m_TomGent.SetActive(false);
			m_TomAxe.transform.SetParent(originalAxeParent);
			m_TomAxe.SetActive(true);
			m_TomAxe.transform.localPosition = originalAxePosition;
			m_TomAxe.transform.localEulerAngles = originalAxeEuler;
			MeleeWeapon meleeWeapon = Object.Instantiate<MeleeWeapon>(m_Axe);
			GameManager.Instance.Player.WeaponGameObject = meleeWeapon.gameObject;
			GameManager.Instance.Player.EquipWeapon();
			if (Object.op_Implicit((Object)(object)meleeWeapon) && (Object)(object)meleeWeapon.Interaction != (Object)null)
			{
				meleeWeapon.Interaction.SetActive(active: false);
			}
			meleeWeapon.KillInteraction();
			meleeWeapon.Equip();
			meleeWeapon.transform.SetParent(GameManager.Instance.Player.WeaponParent);
			meleeWeapon.transform.localPosition = Vector3.zero;
			Transform obj = meleeWeapon.transform;
			obj.localPosition -= Vector3.up * 5f;
			meleeWeapon.transform.localEulerAngles = Vector3.zero;
			ShortcutExtensions.DOLocalMove(meleeWeapon.transform, Vector3.zero, 0.5f, false);
		});
		num += 0.5f;
		TweenSettingsExtensions.InsertCallback(val2, num, new TweenCallback(CompleteController));
	}

	private void CompleteController()
	{
		GameManager.Instance.ShowCrosshair();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_SammyAi))
		{
			m_SammyAi.OnBreakPlank -= HandleSammyOnBreakPlank;
			m_SammyAi.OnRemoveMask -= HandleSammyOnRemoveMask;
		}
		m_DeathClips = null;
		m_NoMaskClip = null;
		m_SongwriterMusic = null;
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			m_MusicObject.Clear();
			m_MusicObject = null;
		}
		base.OnDisposed();
	}
}
