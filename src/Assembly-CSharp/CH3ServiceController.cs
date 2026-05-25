using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH3ServiceController : BaseController
{
	[Header("Searcher Spawners")]
	[SerializeField]
	private List<Transform> m_SearcherSpawners;

	[Header("Butcher Gang Spawners")]
	[SerializeField]
	private List<Transform> m_ButcherGangSpawners;

	[Header("Searcher Boss Spawners")]
	[SerializeField]
	private Transform m_SearcherBossSpawner;

	[Header("Planks")]
	[SerializeField]
	private GameObject m_FakePlanks;

	[SerializeField]
	private GameObject m_BreakablePlanks;

	[Space]
	[SerializeField]
	private CH3LeverLight m_Lever01;

	[SerializeField]
	private CH3LeverLight m_Lever02;

	[SerializeField]
	private CH3LeverLight m_Lever03;

	[Header("Door")]
	[SerializeField]
	private BaseDoorController m_Door;

	private List<SearcherBossAi> m_Searchers = new List<SearcherBossAi>();

	private List<ButcherGangAi> m_ButcherGang = new List<ButcherGangAi>();

	private SearcherBossAi m_SearcherBoss;

	private AudioObject m_Music;

	private AudioObject m_RumbleAudio;

	private AudioClip m_SearcherMusicClip;

	private AudioClip m_FriendsMusicClip;

	private AudioClip m_LaughingMusicClip;

	private AudioClip m_RumbleClip;

	public override void Init()
	{
		base.Init();
		m_BreakablePlanks.SetActive(false);
		m_FakePlanks.SetActive(true);
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_SearcherMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_The_Searchers");
		m_FriendsMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_oldfriendsnewfaces");
		m_LaughingMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_whoslaughingnow_Loop");
		m_RumbleClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Rumble_Loop_01");
	}

	public override void Activate()
	{
		if (!m_IsActive)
		{
			m_IsActive = true;
			m_BreakablePlanks.SetActive(true);
			m_FakePlanks.SetActive(false);
			if (CheckInternecion() && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.InternecionValue != 414)
			{
				GameManager.Instance.CurrentChapter.DeathController.OnDeath += HandlePlayerOnDeath;
				Initialize();
			}
		}
	}

	private bool CheckInternecion()
	{
		return !GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ChoseDevilsPath && !GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HasDied && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.Toy == 1;
	}

	private void Initialize()
	{
		m_Lever01.OnComplete += HandleLever01OnComplete;
		m_Lever01.Activate();
	}

	private void HandleLever01OnComplete(object sender, EventArgs e)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		m_Lever01.OnComplete -= HandleLever01OnComplete;
		m_Door.Close();
		m_Door.Lock();
		m_Music = GameManager.Instance.AudioManager.Play(m_FriendsMusicClip, AudioObjectType.MUSIC, -1);
		for (int i = 0; i < m_SearcherSpawners.Count; i++)
		{
			SearcherBossAi searcherBossAi = GameManager.Instance.AssetManager.CreateAsset<SearcherBossAi>("GamePlay/Characters/Ai_Searcher_MiniBoss");
			searcherBossAi.transform.position = m_SearcherSpawners[i].position;
			searcherBossAi.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			searcherBossAi.OnDeath += HandleSearcherOnDeath;
			m_Searchers.Add(searcherBossAi);
		}
	}

	private void HandleSearcherOnDeath(object sender, EventArgs e)
	{
		SearcherBossAi searcherBossAi = (SearcherBossAi)sender;
		searcherBossAi.OnDeath -= HandleSearcherOnDeath;
		if (m_Searchers.Contains(searcherBossAi))
		{
			m_Searchers.Remove(searcherBossAi);
		}
		if (m_Searchers.Count <= 0)
		{
			ClearMusic();
			m_Door.Unlock();
			m_Door.Open(1f, (Ease)6, 145f);
			m_Lever02.OnComplete += HandleLever02OnComplete;
			m_Lever02.Activate();
		}
	}

	private void HandleLever02OnComplete(object sender, EventArgs e)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		m_Lever02.OnComplete -= HandleLever02OnComplete;
		m_Door.Close();
		m_Door.Lock();
		m_Music = GameManager.Instance.AudioManager.Play(m_LaughingMusicClip, AudioObjectType.MUSIC, -1);
		string[] array = new string[9] { "GamePlay/Characters/Ai_Striker", "GamePlay/Characters/Ai_Striker", "GamePlay/Characters/Ai_Striker", "GamePlay/Characters/Ai_Piper", "GamePlay/Characters/Ai_Piper", "GamePlay/Characters/Ai_Piper", "GamePlay/Characters/Ai_Fisher", "GamePlay/Characters/Ai_Fisher", "GamePlay/Characters/Ai_Fisher" };
		m_ButcherGangSpawners.Shuffle();
		for (int i = 0; i < m_ButcherGangSpawners.Count; i++)
		{
			ButcherGangAi butcherGangAi = GameManager.Instance.AssetManager.CreateAsset<ButcherGangAi>(array[i]);
			butcherGangAi.transform.position = m_ButcherGangSpawners[i].position;
			butcherGangAi.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			butcherGangAi.OnDeath += HandleButcherGangOnDeath;
			m_ButcherGang.Add(butcherGangAi);
		}
	}

	private void HandleButcherGangOnDeath(object sender, EventArgs e)
	{
		ButcherGangAi butcherGangAi = (ButcherGangAi)sender;
		butcherGangAi.OnDeath -= HandleSearcherOnDeath;
		if (m_ButcherGang.Contains(butcherGangAi))
		{
			m_ButcherGang.Remove(butcherGangAi);
		}
		if (m_ButcherGang.Count <= 0)
		{
			ClearMusic();
			m_Door.Unlock();
			m_Door.Open(1f, (Ease)6, 145f);
			m_Lever03.OnComplete += HandleLever03OnComplete;
			m_Lever03.Activate();
		}
	}

	private void HandleLever03OnComplete(object sender, EventArgs e)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		m_Lever03.OnComplete -= HandleLever03OnComplete;
		m_Door.Close();
		m_Door.Lock();
		m_Music = GameManager.Instance.AudioManager.Play(m_SearcherMusicClip, AudioObjectType.MUSIC, -1);
		m_SearcherBoss = GameManager.Instance.AssetManager.CreateAsset<SearcherBossAi>("GamePlay/Characters/Ai_Searcher_Boss");
		m_SearcherBoss.transform.position = m_SearcherBossSpawner.position;
		m_SearcherBoss.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
		m_SearcherBoss.OnDeath += HandleSearcherBossOnDeath;
	}

	private void HandleSearcherBossOnDeath(object sender, EventArgs e)
	{
		m_SearcherBoss.OnDeath -= HandleSearcherBossOnDeath;
		ClearMusic();
		m_Door.Unlock();
		m_Door.Open(1f, (Ease)6, 145f);
		Complete();
	}

	private void Complete()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		GameManager.Instance.CurrentChapter.DeathController.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.GameData.CurrentSaveFile.Internecions[2] = 1;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.InternecionValue = 414;
		GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
		GameManager.Instance.GameCamera.VisionEffect.BeginEffect();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
		{
			GameManager.Instance.GameCamera.VisionEffect.EndEffect();
		});
		m_RumbleAudio = GameManager.Instance.AudioManager.Play(m_RumbleClip, AudioObjectType.SOUND_EFFECT, -1);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 5f, 0.1f, 15, 90f, false, false), new TweenCallback(ScreenRumbleOnComplete));
	}

	private void ScreenRumbleOnComplete()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		ShortcutExtensions.DOLocalMove(GameManager.Instance.GameCamera.transform, Vector3.zero, 0.5f, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_RumbleAudio.AudioSource.DOFade(0f, 1f), (Ease)1), (TweenCallback)delegate
		{
			m_RumbleAudio.Clear();
			m_RumbleAudio = null;
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.InternecionValue = 414;
			GameManager.Instance.GameDataManager.Save();
			SendOnComplete();
		});
	}

	private void ClearMusic()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		AudioObject audio = m_Music;
		m_Music = null;
		if ((Object)(object)audio != (Object)null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(audio.AudioSource.DOFade(0f, 2f), (Ease)1), (TweenCallback)delegate
			{
				audio.Clear();
			});
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		m_Lever01.Reset();
		m_Lever01.OnComplete -= HandleLever01OnComplete;
		m_Lever01.OnComplete += HandleLever01OnComplete;
		m_Lever02.Disable();
		m_Lever02.OnComplete -= HandleLever02OnComplete;
		m_Lever03.Disable();
		m_Lever03.OnComplete -= HandleLever03OnComplete;
		ClearMusic();
		for (int i = 0; i < m_Searchers.Count; i++)
		{
			m_Searchers[i].Dispose();
		}
		m_Searchers.Clear();
		for (int j = 0; j < m_ButcherGang.Count; j++)
		{
			m_ButcherGang[j].Dispose();
		}
		m_ButcherGang.Clear();
		if ((Object)(object)m_SearcherBoss != (Object)null)
		{
			m_SearcherBoss.Dispose();
		}
		m_SearcherBoss = null;
		m_Door.Unlock();
		m_Door.Open(1f, (Ease)6, 145f);
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_RumbleAudio != (Object)null)
		{
			m_RumbleAudio.Clear();
			m_RumbleAudio = null;
		}
		if ((Object)(object)m_Music != (Object)null)
		{
			m_Music.Clear();
			m_Music = null;
		}
		m_SearcherMusicClip = null;
		m_FriendsMusicClip = null;
		m_LaughingMusicClip = null;
		m_RumbleClip = null;
		base.OnDisposed();
	}
}
