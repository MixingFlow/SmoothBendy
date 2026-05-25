using DG.Tweening;
using S13Audio;
using TMG.Controls;
using UnityEngine;

public class Minigame_Hammer : WinnableMiniGameBaseController
{
	[Header("Bell")]
	[SerializeField]
	private Transform m_Bell;

	[SerializeField]
	private Transform m_BellBottom;

	[SerializeField]
	private Transform m_BellTop;

	[Header("Button")]
	[SerializeField]
	private Transform m_Button;

	[Header("Audio")]
	[SerializeField]
	private Transform m_AudioProxy;

	private S13Switch m_AudioSwitch;

	private int m_CurrentTry;

	private int m_TryMax = 3;

	private float m_HammerStrength;

	private float m_ClickValue;

	private float m_TimeSinceStart;

	private bool m_GameReset = true;

	private bool m_HasHadFun;

	private AudioClip m_AliceHavingFunClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AliceHavingFunClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Alice/DIA_CH4_ALICE_HAVING_FUN");
		m_AudioSwitch = ((Component)m_AudioProxy).GetComponentInChildren<S13Switch>();
	}

	public override void HandleGameStartupSequence()
	{
		base.HandleGameStartupSequence();
		HandleOnPrepHeldObject();
	}

	public override void BeginGameLoops()
	{
		base.BeginGameLoops();
	}

	protected override void OnPickupObject()
	{
		base.OnPickupObject();
		m_AudioSwitch.Play("pickup");
	}

	public override void Update()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (base.CurrentState == MiniGameState.ACTIVE && m_GameReset)
		{
			m_TimeSinceStart += Time.deltaTime;
			m_HammerStrength = 0.5f * (1f + Mathf.Sin(6.28f * m_TimeSinceStart));
			base.HeldObject.localRotation = Quaternion.Euler(-35f * m_HammerStrength, 0f, 0f);
			if (PlayerInput.Attack())
			{
				m_CurrentTry++;
				m_ClickValue = m_HammerStrength;
				m_GameReset = false;
				DOHammerHit();
			}
		}
	}

	public void DOHammerHit()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		if (base.CurrentState == MiniGameState.ACTIVE)
		{
			float num = 0.5f * m_ClickValue + 0.1f;
			Vector3 val = Vector3.Lerp(m_BellBottom.localPosition, m_BellTop.localPosition, m_ClickValue);
			Sequence val2 = DOTween.Sequence();
			float num2 = 0f;
			TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(base.HeldObject, new Vector3(90f, 0f, 0f), 0.2f, (RotateMode)0), (Ease)5));
			TweenSettingsExtensions.InsertCallback(val2, num2, (TweenCallback)delegate
			{
				m_AudioSwitch.Play("swing");
			});
			num2 += 0.2f;
			TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(base.HeldObject, new Vector3(-35f * m_ClickValue, 0f, 0f), 0.3f, (RotateMode)0), (Ease)7));
			TweenSettingsExtensions.InsertCallback(val2, num2, (TweenCallback)delegate
			{
				m_AudioSwitch.Play("hit");
			});
			TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Bell, val, num, false), (Ease)6));
			TweenSettingsExtensions.InsertCallback(val2, num2, (TweenCallback)delegate
			{
				m_AudioSwitch.Play("slide");
			});
			TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Button, 0f - m_ClickValue, 0.15f, false), (Ease)1));
			TweenSettingsExtensions.Insert(val2, num2 + 0.15f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Button, 0f, 0.5f, false), (Ease)7));
			num2 += num;
			TweenSettingsExtensions.InsertCallback(val2, num2, new TweenCallback(CheckBellHit));
			TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Bell, m_BellBottom.localPosition, num, false), (Ease)5));
			num2 += num;
			TweenSettingsExtensions.InsertCallback(val2, num2 - 0.1f, (TweenCallback)delegate
			{
				m_AudioSwitch.Play("reset");
			});
			TweenSettingsExtensions.OnComplete<Sequence>(val2, new TweenCallback(CheckBellWinAndReset));
		}
	}

	private void CheckBellHit()
	{
		if (m_ClickValue > 0.9f)
		{
			m_AudioSwitch.Play("bell");
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.WASTING_TIME);
		}
	}

	private void CheckBellWinAndReset()
	{
		m_GameReset = true;
		if (m_ClickValue > 0.9f)
		{
			if (!m_HasHadFun)
			{
				m_HasHadFun = true;
				GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceHavingFunClip, "DIACH4/DIA_CH4_ALICE_HAVING_FUN"));
			}
			FinishGame();
		}
		else if (m_CurrentTry >= m_TryMax)
		{
			FinishGame();
		}
	}

	protected override void OnPlaceObject()
	{
		base.OnPlaceObject();
		m_AudioSwitch.Play("drop");
	}

	private void FinishGame()
	{
		m_GameReset = true;
		m_ClickValue = 0f;
		m_CurrentTry = 0;
		SetState(MiniGameState.INACTIVE);
		ExitGame();
	}

	protected override void OnDisposed()
	{
		m_AudioSwitch = null;
		m_AliceHavingFunClip = null;
		base.OnDisposed();
	}
}
