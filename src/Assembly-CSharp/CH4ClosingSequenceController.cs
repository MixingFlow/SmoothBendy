using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4ClosingSequenceController : BaseController
{
	private const string MELT_PERCENTAGE = "_MeltPercentage";

	private const string DISSOLVE = "_Dissolve";

	[Header("Locations")]
	[SerializeField]
	private Transform m_BorisLookLocation;

	[SerializeField]
	private Transform m_PlayerCamLocation;

	[SerializeField]
	private Transform m_LookAtLocation;

	[SerializeField]
	private Transform m_BorisLocation;

	[SerializeField]
	private Transform m_FinalLookLocation;

	[Header("Characters")]
	[SerializeField]
	private GameObject m_Tom;

	[SerializeField]
	private GameObject m_Allison;

	[SerializeField]
	private GameObject m_Alice;

	[Header("Effects")]
	[SerializeField]
	private ParticleSystem m_GushParticles;

	[SerializeField]
	private ParticleSystem m_SpurtParticles;

	[SerializeField]
	private GameObject m_InkSplat;

	[SerializeField]
	private Renderer m_BorisBody;

	[Header("Alice Animators")]
	[SerializeField]
	private Animator m_AliceAnimator;

	[SerializeField]
	private AnimationClip m_AliceAnimationClip;

	[SerializeField]
	private Transform m_AliceAudioProxy;

	[Header("Tom Animators")]
	[SerializeField]
	private Animator m_TomAnimator;

	[SerializeField]
	private AnimationClip m_TomAnimationClip;

	[SerializeField]
	private Transform m_TomAudioProxy;

	private Transform m_FreeRoamCamera;

	private AudioClip m_BorisMusicClip;

	private AudioClip m_EndingMusicClip;

	private AudioClip m_AliceDeathClip;

	private S13Switch m_AudioSwitch;

	private S13ObjectComplex m_TomAudioComplex;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_BorisMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_OldEndings");
		m_EndingMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_NewBeginnings");
		m_AliceDeathClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Alice/DIA_CH4_ALICE_DEATH");
		AnimationEventUtil.AddAnimationEvent(ref m_AliceAnimator, ((Object)m_AliceAnimationClip).name, "AliceHit", 55);
		AnimationEventUtil.AddAnimationEvent(ref m_AliceAnimator, ((Object)m_AliceAnimationClip).name, "LookAtAlice", 190);
		AnimationEventUtil.AddAnimationEvent(ref m_AliceAnimator, ((Object)m_AliceAnimationClip).name, "AliceFall", 230);
		AnimationEventUtil.AddAnimationEvent(ref m_TomAnimator, ((Object)m_TomAnimationClip).name, "TomTap", 45);
		AnimationEventUtil.AddAnimationEvent(ref m_TomAnimator, ((Object)m_TomAnimationClip).name, "TomTap", 60);
		m_InkSplat.SetActive(false);
		m_Tom.SetActive(false);
		m_Allison.SetActive(false);
		m_Alice.SetActive(false);
		m_AudioSwitch = ((Component)m_AliceAudioProxy).GetComponentInChildren<S13Switch>();
		m_TomAudioComplex = ((Component)m_TomAudioProxy).GetComponentInChildren<S13ObjectComplex>();
	}

	public override void Activate()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		GameManager.Instance.AudioManager.Play(m_BorisMusicClip, AudioObjectType.MUSIC).OnComplete += HandleBorisMusicOnComplete;
		m_FreeRoamCamera = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.LockPause();
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.Player.SetCameraSway(active: true);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCamera, m_BorisLookLocation.position, 12f, false), (Ease)7);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCamera, m_BorisLookLocation.eulerAngles, 12f, (RotateMode)0), (Ease)7);
		Sequence val = DOTween.Sequence();
		float num = 2f;
		float num2 = 11f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			S13AudioManager.Instance.InvokeEvent("evt_boris_death_melt");
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(m_BorisBody.materials[0], 1f, "_MeltPercentage", num2), (Ease)1));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(m_BorisBody.materials[1], 1f, "_MeltPercentage", num2), (Ease)1));
		num += num2;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(m_BorisBody.materials[0], 1f, "_Dissolve", 4f), (Ease)1));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(m_BorisBody.materials[1], 1f, "_Dissolve", 4f), (Ease)1));
	}

	private void HandleBorisMusicOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleBorisMusicOnComplete;
		GameManager.Instance.AudioManager.Play(m_AliceDeathClip, AudioObjectType.DIALOGUE);
		GameManager.Instance.AudioManager.Play(m_EndingMusicClip, AudioObjectType.MUSIC).OnComplete += HandleMusicOnComplete;
		DOAliceDeath();
	}

	private Sequence DOAliceDeath()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(val, 1f, new TweenCallback(ActualActivate));
		num += 17.1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(0f);
		});
		TweenSettingsExtensions.InsertCallback(val, num + 0.1f, (TweenCallback)delegate
		{
			m_Tom.SetActive(false);
			m_Alice.SetActive(false);
			m_Allison.SetActive(false);
		});
		return val;
	}

	private void ActualActivate()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		m_Alice.SetActive(true);
		m_Allison.SetActive(true);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCamera, m_PlayerCamLocation.position, 1f, false), (Ease)1);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCamera, m_PlayerCamLocation.eulerAngles, 1f, (RotateMode)0), (Ease)1);
		GameManager.Instance.Player.SetFOVValue(45f, 2.5f);
	}

	public void AliceHit()
	{
		m_GushParticles.Emit(10);
		m_SpurtParticles.Emit(10);
		m_InkSplat.SetActive(true);
		m_AudioSwitch.Play("thrust");
	}

	public void AliceFall()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		m_Tom.transform.position = m_BorisLocation.position;
		m_Tom.transform.eulerAngles = m_BorisLocation.eulerAngles;
		m_Tom.SetActive(true);
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLookAt(m_FreeRoamCamera, m_FinalLookLocation.position, 4f, (AxisConstraint)0, (Vector3?)null), 1f), (Ease)7);
	}

	public void LookAtAlice()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(m_FreeRoamCamera, m_LookAtLocation.position, 1f, (AxisConstraint)0, (Vector3?)null), (Ease)7);
		m_AudioSwitch.Play("fall");
	}

	public void TomTap()
	{
		m_TomAudioComplex.Play();
	}

	private void HandleMusicOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleMusicOnComplete;
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_FreeRoamCamera = null;
		m_EndingMusicClip = null;
		m_AliceDeathClip = null;
		m_AudioSwitch = null;
		m_TomAudioComplex = null;
		base.OnDisposed();
	}
}
