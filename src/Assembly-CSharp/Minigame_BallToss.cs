using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Controls;
using UnityEngine;

public class Minigame_BallToss : WinnableMiniGameBaseController
{
	[Header("Startup Lights")]
	[SerializeField]
	private Transform m_BoothGate;

	[Header("Ball Toss Props")]
	[SerializeField]
	private CH4FairGameBottle[] m_Bottles;

	[SerializeField]
	private Rigidbody m_BallObject;

	[Header("Audio")]
	[SerializeField]
	private Transform m_AudioPosition;

	private int m_WinAmount;

	private int m_AchievementAmount;

	private int m_Score;

	private float m_ThrowForce;

	private float m_TimeSinceStart;

	private int m_BallLimit = 3;

	private int m_BallCount;

	private bool m_Reloading;

	private Vector3 m_LastBallPosition;

	private List<Rigidbody> m_Balls = new List<Rigidbody>();

	private AudioClip m_MusicClip;

	private S13Switch m_AudioSwitch;

	private S13ObjectSimple m_AudioSimple;

	public override void Init()
	{
		base.Init();
		m_AchievementAmount = m_Bottles.Length;
		m_WinAmount = 5;
	}

	public override void InitOnComplete()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_InteractGameStart.gameObject.SetActive(false);
		m_BoothGate.localEulerAngles = new Vector3(90f, 0f, 0f);
		m_AudioSimple = ((Component)m_BoothGate).GetComponentInChildren<S13ObjectSimple>();
		m_AudioSwitch = ((Component)m_AudioPosition).GetComponentInChildren<S13Switch>();
		m_MusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_letsplay_loop");
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
		for (int num = 0; num < m_Bottles.Length; num++)
		{
			m_Bottles[num].OnHit += HandleBottleOnHit;
		}
	}

	public void ForceOpen()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		m_BoothGate.localEulerAngles = new Vector3(0f, 0f, 0f);
		m_InteractGameStart.gameObject.SetActive(true);
		for (int i = 0; i < m_Bottles.Length; i++)
		{
			m_Bottles[i].OnHit += HandleBottleOnHit;
		}
	}

	private void HandleBottleOnHit(object sender, EventArgs e)
	{
		m_Score++;
	}

	public override void HandleGameStartupSequence()
	{
		m_BallCount = 0;
		m_Score = 0;
		m_Reloading = false;
		((Component)base.HeldObject).gameObject.SetActive(true);
		for (int i = 0; i < m_Balls.Count; i++)
		{
			Object.Destroy((Object)(object)((Component)m_Balls[i]).gameObject);
		}
		m_Balls.Clear();
		for (int j = 0; j < m_Bottles.Length; j++)
		{
			m_Bottles[j].Reset();
		}
		base.HandleGameStartupSequence();
		HandleOnPrepHeldObject();
	}

	public override void BeginGameLoops()
	{
		base.BeginGameLoops();
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_MusicClip, AudioObjectType.MUSIC, -1);
	}

	protected override void OnPickupObject()
	{
		m_AudioSwitch.Play("pickup");
	}

	protected override void OnPlaceObject()
	{
		m_AudioSwitch.Play("drop");
	}

	public override void Update()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (base.CurrentState != MiniGameState.ACTIVE || m_Reloading)
		{
			return;
		}
		m_TimeSinceStart += Time.deltaTime;
		m_ThrowForce = 0.8f * (1f + Mathf.Sin(6.28f * m_TimeSinceStart));
		base.HeldObject.localPosition = new Vector3(0f, 0.25f * (0f - m_ThrowForce), 0f);
		if (PlayerInput.Attack())
		{
			m_Reloading = true;
			m_AudioSwitch.Play("toss");
			Rigidbody val = Object.Instantiate<Rigidbody>(m_BallObject);
			((Component)val).transform.position = base.HeldObject.position;
			((Component)val).transform.eulerAngles = GameManager.Instance.GameCamera.transform.eulerAngles;
			((Component)val).gameObject.SetActive(true);
			val.velocity = 25f * m_ThrowForce * ((Component)val).transform.forward + ((Component)val).transform.up * 8f - ((Component)val).transform.right;
			val.AddTorque(new Vector3(Random.Range(500f, 1000f), Random.Range(500f, 1000f), Random.Range(500f, 1000f)), (ForceMode)1);
			m_LastBallPosition = base.HeldObject.localPosition;
			((Component)base.HeldObject).gameObject.SetActive(false);
			Transform heldObject = base.HeldObject;
			heldObject.localPosition += new Vector3(0f, -5f, 0f);
			m_Balls.Add(val);
			m_BallCount++;
			SimpleOnHit component = ((Component)val).GetComponent<SimpleOnHit>();
			if ((Object)(object)component != (Object)null)
			{
				component.OnHit += HandleBallOnHit;
				component.OnHitGeneric += HandleBallOnHitGeneric;
			}
			ThrowSequence();
		}
	}

	private void HandleBallOnHit(object sender, EventArgs e)
	{
		(sender as SimpleOnHit).OnHit -= HandleBallOnHit;
		(sender as SimpleOnHit).OnHit -= HandleBallOnHitGeneric;
		m_AudioSwitch.Play("hit");
	}

	private void HandleBallOnHitGeneric(object sender, EventArgs e)
	{
		(sender as SimpleOnHit).OnHit -= HandleBallOnHit;
		(sender as SimpleOnHit).OnHit -= HandleBallOnHitGeneric;
		m_AudioSwitch.Play("bounce");
	}

	public void ThrowSequence()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		if (base.CurrentState == MiniGameState.INACTIVE)
		{
			return;
		}
		if (m_BallCount >= m_BallLimit)
		{
			ExitGameSequence = TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 3.5f, (TweenCallback)delegate
			{
				if (m_Score >= m_WinAmount || m_Score >= m_AchievementAmount)
				{
					S13AudioManager.Instance.PlayAudio("sfx_fair_game_win");
					if (m_Score >= m_AchievementAmount)
					{
						DebugLog("[ACHIEVEMENT] - Ball Toss");
						GameManager.Instance.AchievementManager.SetAchievement(AchievementName.CALL_THE_MILK_MAN);
					}
					SendOnWin();
				}
				else
				{
					S13AudioManager.Instance.PlayAudio("sfx_fair_game_lose");
				}
				SetState(MiniGameState.INACTIVE);
				ExitGame();
			});
		}
		else if (base.CurrentState == MiniGameState.ACTIVE)
		{
			Sequence val = DOTween.Sequence();
			float num = 1f;
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				((Component)base.HeldObject).gameObject.SetActive(true);
			});
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(base.HeldObject, m_LastBallPosition, 0.5f, false), (Ease)6));
			num += 0.5f;
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				m_Reloading = false;
			});
		}
	}

	protected override void OnDisposed()
	{
		m_MusicClip = null;
		m_AudioSimple = null;
		m_AudioSwitch = null;
		base.OnDisposed();
	}
}
