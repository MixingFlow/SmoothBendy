using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH5TheEnd : BaseController
{
	private const string MELT_PERCENTAGE = "_MeltPercentage";

	private const string DISSOLVE = "_Dissolve";

	[Header("The End")]
	[SerializeField]
	private CH5ReelStation m_ReelStation;

	[SerializeField]
	private Transform m_CameraLook;

	[SerializeField]
	private GameObject m_Cartoons;

	[SerializeField]
	private GameObject m_TheEnd1Blank;

	[SerializeField]
	private GameObject m_TheEnd1;

	[SerializeField]
	private GameObject m_TheEnd2Blank;

	[SerializeField]
	private GameObject m_TheEnd2;

	[SerializeField]
	private GameObject m_TheEndRest;

	[SerializeField]
	private Light m_EndLight;

	[Header("Bendy")]
	[SerializeField]
	private GameObject m_Bendy;

	[SerializeField]
	private Animator m_BendyAnimator;

	[SerializeField]
	private AnimationClip m_BendyClip;

	[SerializeField]
	private ParticleSystem m_BendyParticles;

	[SerializeField]
	private Renderer m_BendyBody;

	private AudioClip m_EndMusic;

	private AudioClip m_InkDemonMusic;

	private AudioObject m_MusicObject;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		AnimationEventUtil.AddAnimationEvent(ref m_BendyAnimator, ((Object)m_BendyClip).name, "BlankScreen1", 75);
		AnimationEventUtil.AddAnimationEvent(ref m_BendyAnimator, ((Object)m_BendyClip).name, "BlankScreen2", 281);
		AnimationEventUtil.AddAnimationEvent(ref m_BendyAnimator, ((Object)m_BendyClip).name, "TheEnd1", 231);
		AnimationEventUtil.AddAnimationEvent(ref m_BendyAnimator, ((Object)m_BendyClip).name, "TheEnd2", 315);
		AnimationEventUtil.AddAnimationEvent(ref m_BendyAnimator, ((Object)m_BendyClip).name, "TheEndRest", 498);
		AnimationEventUtil.AddAnimationEvent(ref m_BendyAnimator, ((Object)m_BendyClip).name, "THE_END", 770);
		m_Bendy.SetActive(false);
		m_TheEnd1.SetActive(false);
		m_TheEnd2.SetActive(false);
		m_TheEnd1Blank.SetActive(false);
		m_TheEnd2Blank.SetActive(false);
		m_TheEndRest.SetActive(false);
		m_EndMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_TheEndOfAllThings");
		m_InkDemonMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_TheInkDemon_Underscore");
	}

	public override void Activate()
	{
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_InkDemonMusic, AudioObjectType.MUSIC, -1);
		m_ReelStation.OnInteracted += HandleReelStationOnInteracted;
		m_ReelStation.OnComplete += HandleReelStationOnComplete;
		m_ReelStation.Activate();
	}

	private void HandleReelStationOnInteracted(object sender, EventArgs e)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		m_ReelStation.OnInteracted -= HandleReelStationOnInteracted;
		GameManager.Instance.Player.SetLock(active: true);
		if (!Object.op_Implicit((Object)(object)m_MusicObject))
		{
			return;
		}
		TweenSettingsExtensions.OnComplete<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 3f), (TweenCallback)delegate
		{
			if (Object.op_Implicit((Object)(object)m_MusicObject))
			{
				m_MusicObject.Clear();
				m_MusicObject = null;
			}
		});
	}

	private void HandleReelStationOnComplete(object sender, EventArgs e)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		m_ReelStation.OnComplete -= HandleReelStationOnComplete;
		GameManager.Instance.HideCrosshair();
		m_Cartoons.SetActive(false);
		Transform freeRoamCamera = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		m_Bendy.SetActive(true);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			freeRoamCamera.SetParent(m_CameraLook);
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(freeRoamCamera, Vector3.zero, 0.75f, false), (Ease)7);
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(freeRoamCamera, new Vector3(0f, 90f, 90f), 2f, (RotateMode)0), (Ease)7);
		});
		GameManager.Instance.AudioManager.Play(m_EndMusic, AudioObjectType.MUSIC);
		S13AudioManager.Instance.PlayAudio("sfx_beast_death_anim");
	}

	public void BlankScreen1()
	{
		m_TheEnd1Blank.SetActive(true);
	}

	public void BlankScreen2()
	{
		m_TheEnd2Blank.SetActive(true);
	}

	public void TheEnd1()
	{
		m_TheEnd1Blank.SetActive(false);
		m_TheEnd1.SetActive(true);
	}

	public void TheEnd2()
	{
		m_TheEnd2Blank.SetActive(false);
		m_TheEnd2.SetActive(true);
	}

	public void TheEndRest()
	{
		GameManager.Instance.GameCamera.VisionEffect.BeginEffect();
		m_TheEndRest.SetActive(true);
		TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOIntensity(m_EndLight, 10f, 20f), 2f);
	}

	public void THE_END()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		m_BendyParticles.Stop();
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(m_BendyBody.materials[0], 1f, "_Dissolve", 4f), (Ease)1));
		TweenSettingsExtensions.InsertCallback(val, 1f, (TweenCallback)delegate
		{
			GameManager.Instance.ShowWhiteScreenBlocker(4f);
		});
		TweenSettingsExtensions.InsertCallback(val, 3f, (TweenCallback)delegate
		{
			((Component)m_BendyParticles).gameObject.SetActive(false);
		});
		TweenSettingsExtensions.InsertCallback(val, 4f, (TweenCallback)delegate
		{
			GameManager.Instance.GameCamera.VisionEffect.EndEffect();
		});
		TweenSettingsExtensions.InsertCallback(val, 4.1f, new TweenCallback(base.SendOnComplete));
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
