using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Controls;
using UnityEngine;

public class Minigame_ShootingGallery : WinnableMiniGameBaseController
{
	[Header("Startup Lights")]
	[SerializeField]
	private Transform m_BoothGate;

	[Header("Startup Lights")]
	[SerializeField]
	private LightFixtureController[] m_Lights;

	[Header("Target Objects")]
	[SerializeField]
	private CH4FairGameTarget[] m_Targets;

	[Header("Meter")]
	[SerializeField]
	private Transform m_Meter;

	[SerializeField]
	private Transform m_MeterStart;

	[SerializeField]
	private Transform m_MeterEnd;

	[Header("Game Rules")]
	[SerializeField]
	private Transform m_AudioPosition;

	[SerializeField]
	private int m_HighScore;

	[SerializeField]
	private int m_WinScore;

	[SerializeField]
	private int m_TotalShotCount;

	private RaycastHit hit;

	private Dictionary<Collider, CH4FairGameTarget> m_AllTargets = new Dictionary<Collider, CH4FairGameTarget>();

	private Sequence m_GameSequence;

	private List<int[]> m_GameOrder = new List<int[]>();

	private bool m_IsReloading;

	private int m_CurrentScore;

	private int m_CurrentShotCount;

	private AudioClip m_MusicClip;

	private S13Switch m_AudioSwitch;

	private S13ObjectSimple m_AudioSimple;

	public override void Init()
	{
		base.Init();
		m_GameOrder.Add(new int[10] { 0, 1, 0, 0, 0, 0, 2, 0, 0, 0 });
		m_GameOrder.Add(new int[10] { 0, 0, 0, 2, 0, 0, 0, 0, 0, 1 });
		m_GameOrder.Add(new int[10] { 0, 0, 0, 1, 0, 0, 1, 0, 0, 0 });
		m_GameOrder.Add(new int[10] { 1, 0, 1, 0, 0, 1, 2, 0, 0, 0 });
		m_GameOrder.Add(new int[10] { 0, 0, 0, 0, 2, 0, 0, 2, 0, 0 });
		m_GameOrder.Add(new int[10] { 0, 1, 0, 0, 0, 1, 0, 0, 1, 0 });
		m_GameOrder.Add(new int[10] { 1, 0, 0, 1, 0, 0, 0, 0, 0, 2 });
		m_GameOrder.Add(new int[10] { 0, 0, 1, 0, 0, 0, 1, 0, 1, 0 });
		m_GameOrder.Add(new int[10] { 0, 0, 0, 0, 1, 0, 0, 1, 0, 1 });
		m_GameOrder.Add(new int[10] { 0, 0, 1, 0, 0, 1, 0, 0, 2, 0 });
	}

	public override void InitOnComplete()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		for (int i = 0; i < m_Targets.Length; i++)
		{
			CH4FairGameTarget cH4FairGameTarget = m_Targets[i];
			m_AllTargets.Add(cH4FairGameTarget.HitCollider, cH4FairGameTarget);
		}
		m_InteractGameStart.gameObject.SetActive(false);
		m_BoothGate.localEulerAngles = new Vector3(90f, 0f, 0f);
		m_MusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_letsplay_loop");
		m_AudioSwitch = ((Component)m_AudioPosition).GetComponentInChildren<S13Switch>();
		m_AudioSimple = ((Component)m_BoothGate).GetComponentInChildren<S13ObjectSimple>();
	}

	public void Activate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		m_AudioSimple.Play();
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_BoothGate, new Vector3(-90f, 0f, 0f), 3f, (RotateMode)3), (Ease)7);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.65f, (TweenCallback)delegate
		{
			m_InteractGameStart.gameObject.SetActive(true);
		});
	}

	public void ForceOpen()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		m_BoothGate.localEulerAngles = new Vector3(0f, 0f, 0f);
		m_InteractGameStart.gameObject.SetActive(true);
	}

	public override void HandleGameStartupSequence()
	{
		base.HandleGameStartupSequence();
		m_CurrentScore = 0;
		m_CurrentShotCount = m_TotalShotCount;
		HandleOnPrepHeldObject();
	}

	public override void BeginGameLoops()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		base.BeginGameLoops();
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_MusicClip, AudioObjectType.MUSIC, -1);
		if (m_Meter.localPosition != m_MeterStart.localPosition)
		{
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Meter, m_MeterStart.localPosition, 1f, false), (Ease)1);
			Sequence val = DOTween.Sequence();
			int num = 1;
			float num2 = 0f;
			for (int i = 0; i < 8; i++)
			{
				TweenSettingsExtensions.Insert(val, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Meter, new Vector3(0f, 0f, 8f * (float)num), 0.125f, (RotateMode)0), (Ease)1));
				num2 += 0.125f;
				num *= -1;
			}
		}
		ResetGameSequence();
		if (base.CurrentState == MiniGameState.ACTIVE)
		{
			for (int j = 0; j < m_Targets.Length; j++)
			{
				TweenSettingsExtensions.Insert(m_GameSequence, 0f, (Tween)(object)m_Targets[j].Setup());
			}
			TweenSettingsExtensions.InsertCallback(m_GameSequence, 1.5f, (TweenCallback)delegate
			{
			});
			TweenSettingsExtensions.OnComplete<Sequence>(m_GameSequence, (TweenCallback)delegate
			{
				DOGameSequence();
			});
		}
	}

	protected override void OnPickupObject()
	{
		m_AudioSwitch.Play("pickup");
	}

	public override void Update()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (base.CurrentState != MiniGameState.ACTIVE || m_IsReloading || !PlayerInput.Attack() || m_CurrentShotCount <= 0)
		{
			return;
		}
		m_IsReloading = true;
		m_AudioSwitch.Play("fire");
		if (Physics.Raycast(GameManager.Instance.GameCamera.transform.position, GameManager.Instance.GameCamera.transform.forward, ref hit, 100f))
		{
			Collider collider = ((RaycastHit)(ref hit)).collider;
			if (m_AllTargets.ContainsKey(collider))
			{
				CH4FairGameTarget cH4FairGameTarget = m_AllTargets[collider];
				cH4FairGameTarget.Hit();
				if (cH4FairGameTarget.IsBad)
				{
					m_CurrentScore--;
					if (m_CurrentScore <= 0)
					{
						m_CurrentScore = 0;
					}
					m_AudioSwitch.Play("bad");
				}
				else
				{
					m_CurrentScore++;
					m_AudioSwitch.Play("good");
				}
				Vector3 val = Vector3.Lerp(m_MeterStart.localPosition, m_MeterEnd.localPosition, (float)m_CurrentScore / (float)m_HighScore);
				ShortcutExtensions.DOKill((Component)(object)m_Meter, false);
				TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Meter, val, 0.35f, false), (Ease)7);
				Sequence val2 = DOTween.Sequence();
				int num = 1;
				float num2 = 0f;
				for (int i = 0; i < 4; i++)
				{
					TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Meter, new Vector3(0f, 0f, 8f * (float)num), 0.125f, (RotateMode)0), (Ease)1));
					num2 += 0.125f;
					num *= -1;
				}
			}
		}
		m_CurrentShotCount--;
		DOReload();
	}

	private void DOReload()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0.01f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.HeldObject, -0.25f, 0.06f, false), (Ease)7));
		num += 0.1f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.HeldObject, 0f, 0.06f, false), (Ease)7));
		num += 0.1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_IsReloading = false;
		});
	}

	public void DOGameSequence(int index = 0)
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		if (base.CurrentState == MiniGameState.INACTIVE)
		{
			return;
		}
		if (index >= m_GameOrder.Count)
		{
			GameComplete();
			return;
		}
		m_AudioSwitch.Play("reset");
		ResetGameSequence();
		float num = 0f;
		for (int i = 0; i < m_GameOrder[index].Length; i++)
		{
			int num2 = m_GameOrder[index][i];
			if (num2 == 1 || num2 == 2)
			{
				CH4FairGameTarget cH4FairGameTarget = m_Targets[i];
				TweenSettingsExtensions.InsertCallback(m_GameSequence, num, new TweenCallback(cH4FairGameTarget.EnableCollider));
				TweenSettingsExtensions.Insert(m_GameSequence, num, (Tween)(object)((num2 != 1) ? cH4FairGameTarget.DOShowBad() : cH4FairGameTarget.DOShowGood()));
				TweenSettingsExtensions.Insert(m_GameSequence, num + 1.6f, (Tween)(object)cH4FairGameTarget.DOHide());
				TweenSettingsExtensions.InsertCallback(m_GameSequence, num + 1.7f, new TweenCallback(cH4FairGameTarget.DisableCollider));
			}
		}
		TweenSettingsExtensions.OnComplete<Sequence>(m_GameSequence, (TweenCallback)delegate
		{
			DOGameSequence(index + 1);
		});
	}

	private void GameComplete()
	{
		if (m_CurrentScore >= m_HighScore || m_CurrentScore >= m_WinScore)
		{
			S13AudioManager.Instance.PlayAudio("sfx_fair_game_win");
			if (m_CurrentScore >= m_HighScore)
			{
				DebugLog("[ACHIEVEMENT] - Shooting Gallery");
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.BULLS_EYE);
			}
			SendOnWin();
		}
		else
		{
			S13AudioManager.Instance.PlayAudio("sfx_fair_game_lose");
		}
		SetState(MiniGameState.INACTIVE);
		ExitGame();
	}

	protected override void OnPlaceObject()
	{
		m_AudioSwitch.Play("drop");
	}

	private void KillGameSequence()
	{
		if (m_GameSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_GameSequence, false);
			m_GameSequence = null;
		}
	}

	private void ResetGameSequence()
	{
		KillGameSequence();
		m_GameSequence = DOTween.Sequence();
	}

	protected override void OnDisposed()
	{
		KillGameSequence();
		m_MusicClip = null;
		m_AudioSwitch = null;
		m_AudioSimple = null;
		base.OnDisposed();
	}
}
