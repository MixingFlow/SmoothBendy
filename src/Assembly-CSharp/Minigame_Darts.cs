using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Controls;
using UnityEngine;

public class Minigame_Darts : WinnableMiniGameBaseController
{
	[Header("Object References")]
	[SerializeField]
	private ThrowableObject m_ThrowableDart;

	[SerializeField]
	private Transform m_AudioProxy;

	private int m_DartCount;

	private int m_DartMax = 3;

	private float m_ThrowForce;

	private float m_TimeSinceStart;

	public IHittable[] RingColliders;

	private bool m_Reloading;

	private S13Switch m_AudioSwitch;

	private List<ThrowableObject> ActiveDarts = new List<ThrowableObject>();

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AudioSwitch = ((Component)m_AudioProxy).GetComponentInChildren<S13Switch>();
	}

	public override void HandleGameStartupSequence()
	{
		base.HandleGameStartupSequence();
		for (int num = ActiveDarts.Count - 1; num >= 0; num--)
		{
			ActiveDarts[num].Dispose();
		}
		HandleOnPrepHeldObject();
	}

	public override void BeginGameLoops()
	{
		m_DartCount = 0;
		m_Reloading = false;
		((Component)base.HeldObject).gameObject.SetActive(true);
		base.BeginGameLoops();
		GameManager.Instance.GameCamera.UnityDOF.manualDOF = false;
	}

	protected override void OnPickupObject()
	{
		m_AudioSwitch.Play("pickup");
		GameManager.Instance.GameCamera.UnityDOF.manualDOF = true;
		GameManager.Instance.GameCamera.UnityDOF.focalDistance = 10f;
	}

	public override void Update()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (base.CurrentState == MiniGameState.ACTIVE && !m_Reloading)
		{
			m_TimeSinceStart += Time.deltaTime;
			m_ThrowForce = 0.75f * (1f + Mathf.Sin(6.28f * m_TimeSinceStart));
			base.HeldObject.localPosition = new Vector3(0f, 0f, 0.5f * (0f - m_ThrowForce));
			if (PlayerInput.Attack())
			{
				m_DartCount++;
				m_Reloading = true;
				ThrowableObject throwableObject = Object.Instantiate<ThrowableObject>(m_ThrowableDart);
				throwableObject.transform.position = base.HeldObject.position;
				throwableObject.transform.eulerAngles = GameManager.Instance.GameCamera.FreeRoamCam.eulerAngles;
				throwableObject.gameObject.SetActive(true);
				throwableObject.Initialize(throwableObject.WeaponInfo, throwableObject.transform.forward * throwableObject.Force);
				throwableObject.OnHit += HandleDartOnHit;
				throwableObject.Throw();
				ActiveDarts.Add(throwableObject);
				((Component)base.HeldObject).gameObject.SetActive(false);
				Transform heldObject = base.HeldObject;
				heldObject.localPosition += new Vector3(0f, -5f, 0f);
				Reload();
				m_AudioSwitch.Play("fire");
			}
		}
	}

	private void HandleDartOnHit(object sender, EventArgs e)
	{
		ThrowableObject throwableObject = sender as ThrowableObject;
		throwableObject.OnHit -= HandleDartOnHit;
		throwableObject.HitPlaySoundSimple();
		m_AudioSwitch.Stop("fire");
	}

	public override void AddScore(int Ammount)
	{
		base.AddScore(Ammount);
	}

	protected override void ForceExitGame()
	{
		TweenExtensions.Kill((Tween)(object)ExitGameSequence, false);
		base.ForceExitGame();
	}

	protected override void OnPlaceObject()
	{
		m_AudioSwitch.Play("drop");
	}

	private void Reload()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		if (base.CurrentState != MiniGameState.ACTIVE)
		{
			return;
		}
		if (m_DartCount >= m_DartMax)
		{
			m_DartCount = 0;
			ExitGameSequence = DOTween.Sequence();
			TweenSettingsExtensions.InsertCallback(ExitGameSequence, 2f, (TweenCallback)delegate
			{
				SetState(MiniGameState.INACTIVE);
				ExitGame();
			});
			return;
		}
		Sequence val = DOTween.Sequence();
		float num = 1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			((Component)base.HeldObject).gameObject.SetActive(true);
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(base.HeldObject, Vector3.zero, 0.5f, false), (Ease)6));
		num += 0.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_Reloading = false;
		});
	}
}
