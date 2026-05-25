using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH5LostHarbourBattleController : BaseController
{
	public class Wave
	{
		public float m_WaveStartDelay;

		public BaseAiController[] m_AiStack;

		public Wave(float _Delay, BaseAiController[] _AiStack)
		{
			m_WaveStartDelay = _Delay;
			m_AiStack = _AiStack;
		}
	}

	[SerializeField]
	private AllyAiController m_Allison;

	[SerializeField]
	private BaseAiController m_Searcher;

	[SerializeField]
	private BaseAiController m_LostOne;

	[SerializeField]
	private BaseAiController m_LostOneWeapon;

	[SerializeField]
	private BaseAiController m_Miner;

	[SerializeField]
	private Transform m_PoolParent;

	private SpawnerNode[] m_SpawnNodes;

	private List<Wave> m_AttackWaves = new List<Wave>();

	private List<Wave> m_AttackWavesTemplate = new List<Wave>();

	private List<SpawnerNode> m_OpenSpawnNodes = new List<SpawnerNode>();

	private List<SpawnerNode> m_UsedSpawnNodes = new List<SpawnerNode>();

	private List<SpecialSpawnerNode> m_LostOnesNodes = new List<SpecialSpawnerNode>();

	private List<BaseAiController> m_AiStackToSpawn = new List<BaseAiController>();

	private List<BaseAiController> m_CurrentActiveEnemies = new List<BaseAiController>();

	private List<Transform> m_SpawnPuddles = new List<Transform>();

	private AudioClip m_StandTogetherClip;

	private AudioClip m_StandTogetherFinisherClip;

	private AudioObject m_MusicObject;

	private AudioClip[] m_AllisonBattleClips;

	private float m_SpawnEnemyDelayTimer;

	private int m_AllyKillCount;

	private int m_AllyKillMax = 15;

	private bool m_HasAchievement;

	private int m_PlayerKillCount;

	private int m_PlayerKillMax = 25;

	private bool m_IsDead;

	private bool m_IsBattleComplete;

	private float m_Timer;

	private float m_TimerMax = 150f;

	private bool m_HasDied;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_SpawnNodes = ((Component)this).GetComponentsInChildren<SpawnerNode>();
		m_OpenSpawnNodes.AddRange(m_SpawnNodes);
		SpecialSpawnerNode[] componentsInChildren = ((Component)this).GetComponentsInChildren<SpecialSpawnerNode>();
		m_LostOnesNodes.AddRange(componentsInChildren);
		CreateWave(2f, m_Searcher, m_Searcher, m_Searcher, m_Searcher, m_Searcher, m_Searcher, m_Searcher, m_Searcher, m_Searcher, m_Searcher);
		CreateWave(2f, m_Searcher, m_Searcher, m_LostOne, m_Searcher, m_Searcher, m_LostOne, m_Searcher, m_Searcher, m_LostOne, m_Searcher);
		CreateWave(3f, m_Searcher, m_Searcher, m_LostOneWeapon, m_Searcher, m_Searcher, m_LostOne, m_Searcher, m_LostOne, m_Searcher, m_LostOne);
		CreateWave(4f, m_Searcher, m_LostOneWeapon, m_Searcher, m_LostOne, m_Searcher, m_LostOne, m_Searcher, m_LostOneWeapon, m_Searcher, m_LostOne);
		CreateWave(4f, m_LostOne, m_LostOne, m_LostOne, m_LostOne, m_LostOne, m_LostOneWeapon, m_LostOneWeapon, m_LostOneWeapon, m_LostOneWeapon, m_LostOneWeapon);
		CreateWave(5f, m_Miner, m_Miner, m_Miner);
		m_AttackWaves = new List<Wave>(m_AttackWavesTemplate);
		m_StandTogetherClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_StandingTogether");
		m_StandTogetherFinisherClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_StandingTogether_Finisher");
		m_AllisonBattleClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/AliceA/Battle");
	}

	public override void Activate()
	{
		m_IsActive = true;
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned += HandlePlayerOnSpawned;
		GameManager.Instance.CurrentChapter.DeathController.OnDeath += HandlePlayerOnDeath;
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		m_HasDied = true;
		m_Timer += 200f;
		m_IsDead = true;
	}

	public void EnableMusic()
	{
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_StandTogetherClip, AudioObjectType.MUSIC, -1);
		m_MusicObject.AudioSource.volume = 0f;
		m_MusicObject.AudioSource.DOFade(1f, 2f);
	}

	private void Update()
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Instance.isPaused || !m_IsActive)
		{
			return;
		}
		if (m_SpawnEnemyDelayTimer <= 0f)
		{
			if (m_AiStackToSpawn.Count > 0)
			{
				SpawnEnemy();
				if (m_CurrentActiveEnemies.Count >= 2)
				{
					m_SpawnEnemyDelayTimer = 0.75f;
				}
			}
			else if (m_CurrentActiveEnemies.Count == 0)
			{
				if (m_IsBattleComplete)
				{
					m_IsActive = false;
					if (Object.op_Implicit((Object)(object)m_MusicObject))
					{
						m_MusicObject.Clear();
						m_MusicObject = null;
					}
					GameManager.Instance.AudioManager.Play(m_StandTogetherFinisherClip, AudioObjectType.MUSIC);
					TweenSettingsExtensions.OnComplete<Sequence>(BeginBattleComplete(), new TweenCallback(BattleOnComplete));
					return;
				}
				if (m_AttackWaves.Count <= 0)
				{
					m_AttackWaves = new List<Wave>(m_AttackWavesTemplate);
				}
				m_AiStackToSpawn.AddRange(m_AttackWaves[0].m_AiStack);
				m_AttackWaves.RemoveAt(0);
				AudioClip val = null;
				if (m_AttackWaves.Count == 4)
				{
					val = m_AllisonBattleClips[0];
				}
				else if (m_AttackWaves.Count == 3)
				{
					val = m_AllisonBattleClips[1];
				}
				else if (m_AttackWaves.Count == 2)
				{
					val = m_AllisonBattleClips[2];
				}
				else if (m_AttackWaves.Count == 1)
				{
					val = m_AllisonBattleClips[3];
				}
				if ((Object)(object)val != (Object)null)
				{
					m_Allison.DoSpeak(val.length);
					GameManager.Instance.AudioManager.PlayAtPosition(val, m_Allison.transform.position, AudioObjectType.DIALOGUE, 0, isQueued: false, m_Allison.transform);
				}
			}
		}
		else
		{
			m_SpawnEnemyDelayTimer -= Time.deltaTime;
		}
		m_Timer += Time.deltaTime;
	}

	private Sequence BeginBattleComplete()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		foreach (Transform item in m_PoolParent)
		{
			Transform val2 = item;
			TweenSettingsExtensions.Insert(val, 0.25f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(val2, 0f, Random.Range(3f, 5f)), (Ease)5));
		}
		return val;
	}

	private void BattleOnComplete()
	{
		((Component)m_PoolParent).gameObject.SetActive(false);
		if (!m_IsDead)
		{
			EndBattle();
		}
	}

	private void EndBattle()
	{
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		GameManager.Instance.CurrentChapter.DeathController.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.SHADOWS_AND_SUFFERING);
		if (!m_HasDied && m_Timer < m_TimerMax)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.AGGRESSION);
		}
		SendOnComplete();
	}

	private void HandlePlayerOnSpawned(object sender, EventArgs e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		m_IsDead = false;
		if (m_IsBattleComplete)
		{
			TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, new TweenCallback(EndBattle));
		}
	}

	private void CreateWave(float delay, params BaseAiController[] ai)
	{
		m_AttackWavesTemplate.Add(new Wave(delay, ai));
	}

	private void SpawnEnemy()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Transform val = null;
		val = GetSpawnNode(m_AiStackToSpawn[0] is LostOneFightAi);
		if (!m_SpawnPuddles.Contains(val))
		{
			m_SpawnPuddles.Add(val);
			Transform val2 = GameManager.Instance.AssetManager.CreateAsset<Transform>("GamePlay/Particles/Searcher_SpawnPool");
			val2.position = val.position;
			val2.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			if (Object.op_Implicit((Object)(object)m_PoolParent))
			{
				val2.SetParent(m_PoolParent);
			}
		}
		BaseAiController baseAiController = Object.Instantiate<BaseAiController>(m_AiStackToSpawn[0], ((Component)val).transform.position, ((Component)val).transform.rotation);
		baseAiController.OnHide += HandleEnemyOnHide;
		baseAiController.OnDeath += EnemyOnDeath;
		m_CurrentActiveEnemies.Add(baseAiController);
		m_AiStackToSpawn.RemoveAt(0);
	}

	private Transform GetSpawnNode(bool _isLostOneSpawner)
	{
		Transform val = null;
		if (_isLostOneSpawner)
		{
			SpecialSpawnerNode specialSpawnerNode = m_LostOnesNodes[0];
			m_LostOnesNodes.Add(specialSpawnerNode);
			m_LostOnesNodes.RemoveAt(0);
			return specialSpawnerNode.transform;
		}
		SpawnerNode spawnerNode = m_OpenSpawnNodes[0];
		if (m_OpenSpawnNodes.Count == 1)
		{
			m_OpenSpawnNodes.AddRange(m_UsedSpawnNodes);
			m_UsedSpawnNodes.Clear();
			m_UsedSpawnNodes.Add(spawnerNode);
		}
		m_UsedSpawnNodes.Add(spawnerNode);
		m_OpenSpawnNodes.Remove(spawnerNode);
		return spawnerNode.transform;
	}

	private void HandleEnemyOnHide(object sender, EventArgs e)
	{
		BaseAiController baseAiController = sender as BaseAiController;
		baseAiController.OnHide -= HandleEnemyOnHide;
		m_CurrentActiveEnemies.Remove(baseAiController);
		GameManager.Instance.AiGlobalNetwork.RemoveAllyTarget(base.transform);
	}

	public void EnemyOnDeath(object sender, EventArgs e)
	{
		BaseAiController baseAiController = sender as BaseAiController;
		baseAiController.OnDeath -= EnemyOnDeath;
		if ((Object)(object)baseAiController.Attacker != (Object)(object)GameManager.Instance.Player.gameObject)
		{
			if (!m_HasAchievement)
			{
				m_AllyKillCount++;
				if (m_AllyKillCount >= m_AllyKillMax)
				{
					GameManager.Instance.AchievementManager.SetAchievement(AchievementName.GOLDBRICKING);
					DebugLog("ACHIEVEMENT UNLOCKED: GOLDBRICKING");
					m_HasAchievement = true;
				}
			}
		}
		else
		{
			m_PlayerKillCount++;
			if (m_PlayerKillCount >= m_PlayerKillMax)
			{
				m_IsBattleComplete = true;
			}
		}
		m_CurrentActiveEnemies.Remove(baseAiController);
		GameManager.Instance.AiGlobalNetwork.RemoveAllyTarget(base.transform);
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)GameManager.Instance.CurrentChapter.DeathController != (Object)null)
		{
			GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		}
		if ((Object)(object)GameManager.Instance.CurrentChapter.DeathController != (Object)null)
		{
			GameManager.Instance.CurrentChapter.DeathController.OnDeath -= HandlePlayerOnDeath;
		}
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			m_MusicObject.Clear();
			m_MusicObject = null;
		}
		base.OnDisposed();
	}
}
