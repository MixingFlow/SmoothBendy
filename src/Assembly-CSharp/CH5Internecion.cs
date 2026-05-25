using System;
using Ai;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5Internecion : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_InternecionWeapon;

	[SerializeField]
	private MeleeWeapon m_Weapon;

	[SerializeField]
	private BrokenWeapon m_WeaponBroken;

	[SerializeField]
	private GameObject m_Searchers;

	[SerializeField]
	private SearcherAi m_SammySearcher;

	[SerializeField]
	private ButcherGangAi[] m_ButcherGang;

	[SerializeField]
	private AudioClip[] m_BrokenClips;

	private int m_ButcherGangCount;

	private AudioClip m_RumbleClip;

	private AudioObject m_RumbleAudio;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (!GameManager.Instance.GameData.CurrentSaveFile.HasDied && GameManager.Instance.GameData.CurrentSaveFile.Internecions[0] == 0 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[1] == 4 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[2] == 1 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[3] == 4 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[4] == 0)
		{
			m_InternecionWeapon.SetActive(true);
			m_Searchers.SetActive(false);
			m_Weapon.gameObject.SetActive(true);
			m_Weapon.Interaction.SetActive(active: true);
			m_Weapon.OnInteract += HandleWeaponOnInteract;
			for (int i = 0; i < m_ButcherGang.Length; i++)
			{
				m_ButcherGang[i].OnDeath += HandleButcherGangOnDeath;
			}
			GameManager.Instance.CurrentChapter.DeathController.OnDeath += HandlePlayerOnDeath;
			m_RumbleClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Rumble_Loop_01");
		}
		else
		{
			m_InternecionWeapon.SetActive(false);
			m_Searchers.SetActive(false);
			m_Weapon.Interaction.SetActive(active: false);
			m_Weapon.gameObject.SetActive(false);
			m_SammySearcher.gameObject.SetActive(false);
			Dispose();
		}
	}

	private void HandleWeaponOnInteract(object sender, EventArgs e)
	{
		m_Weapon.OnInteract -= HandleWeaponOnInteract;
		m_Searchers.SetActive(true);
	}

	private void HandleButcherGangOnDeath(object sender, EventArgs e)
	{
		ButcherGangAi butcherGangAi = sender as ButcherGangAi;
		butcherGangAi.OnDeath -= HandleButcherGangOnDeath;
		m_ButcherGangCount++;
		if (m_ButcherGangCount >= m_ButcherGang.Length)
		{
			m_SammySearcher.gameObject.SetActive(true);
			m_SammySearcher.OnDeath += HandleSammySearcherOnDeath;
		}
	}

	private void HandleSammySearcherOnDeath(object sender, EventArgs e)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		m_SammySearcher.OnDeath -= HandleSammySearcherOnDeath;
		BrokenWeapon brokenWeapon = Object.Instantiate<BrokenWeapon>(m_WeaponBroken);
		brokenWeapon.Break(m_Weapon.transform, m_Weapon.transform.position - m_Weapon.transform.forward * 2f);
		GameManager.Instance.Player.UnEquipWeapon();
		m_Weapon.Dispose();
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.InternecionValue = 414;
		GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
		GameManager.Instance.GameCamera.VisionEffect.BeginEffect();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
		{
			GameManager.Instance.GameCamera.VisionEffect.EndEffect();
		});
		m_RumbleAudio = GameManager.Instance.AudioManager.Play(m_RumbleClip, AudioObjectType.SOUND_EFFECT, -1);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 5f, 0.1f, 15, 90f, false, false), new TweenCallback(ScreenRumbleOnComplete));
		for (int num = 0; num < m_BrokenClips.Length; num++)
		{
			GameManager.Instance.AudioManager.Play(m_BrokenClips[num]);
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.CurrentChapter.DeathController.OnDeath -= HandlePlayerOnDeath;
		m_InternecionWeapon.SetActive(false);
		if ((Object)(object)m_Weapon != (Object)null)
		{
			m_Weapon.Dispose();
		}
		if ((Object)(object)GameManager.Instance.Player.WeaponGameObject != (Object)null)
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
		}
		Dispose();
	}

	private void ScreenRumbleOnComplete()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		ShortcutExtensions.DOLocalMove(GameManager.Instance.GameCamera.transform, Vector3.zero, 0.5f, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_RumbleAudio.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
		{
			if ((Object)(object)m_RumbleAudio != (Object)null)
			{
				m_RumbleAudio.Clear();
				m_RumbleAudio = null;
			}
			Dispose();
		});
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.CurrentChapter.DeathController.OnDeath -= HandlePlayerOnDeath;
		if ((Object)(object)m_Weapon != (Object)null)
		{
			m_Weapon.OnInteract -= HandleWeaponOnInteract;
		}
		if ((Object)(object)m_SammySearcher != (Object)null)
		{
			m_SammySearcher.OnDeath -= HandleSammySearcherOnDeath;
		}
		for (int i = 0; i < m_ButcherGang.Length; i++)
		{
			if ((Object)(object)m_ButcherGang[i] != (Object)null)
			{
				m_ButcherGang[i].OnDeath -= HandleButcherGangOnDeath;
			}
		}
		if ((Object)(object)m_RumbleAudio != (Object)null)
		{
			m_RumbleAudio.Clear();
			m_RumbleAudio = null;
		}
		base.OnDisposed();
	}
}
