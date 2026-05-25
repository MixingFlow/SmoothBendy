using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using S13Audio;
using UnityEngine;

public class CH1ClosingSequenceController : BaseController
{
	[Header("< END >")]
	[SerializeField]
	private GameObject m_EndSmoke;

	[SerializeField]
	private Transform m_FinalInkEffect;

	[SerializeField]
	private Transform m_FinalEndRotation;

	[SerializeField]
	private EventTrigger m_FinalEventTrigger;

	[SerializeField]
	private Rigidbody m_Axe_Physics;

	private CH1ConclusionModalController m_ConclusionController;

	private Sequence m_Sequence;

	private AudioObject m_RumbleAudio;

	private AudioClip m_RumbleAudioClip;

	private AudioClip m_VisionClip;

	private AudioClip m_FlashClip;

	private float m_Intensity = 0.04f;

	private float m_IntensityIncrease = 0.05f;

	private float m_MaxIntensity = 0.175f;

	private int m_Vibration = 7;

	private int m_VibrationIncrease = 1;

	private int m_MaxVibration = 14;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_FinalEventTrigger.SetActive(active: false);
		m_EndSmoke.SetActive(false);
		m_RumbleAudioClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Rumble_Loop_01");
		m_VisionClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Visions_CH1");
		m_FlashClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Camera_Flash_02");
	}

	public override void Activate()
	{
		S13AudioManager.Instance.InvokeEvent("evt_bendy_finale_scare");
		m_RumbleAudio = GameManager.Instance.AudioManager.Play(m_RumbleAudioClip, AudioObjectType.SOUND_EFFECT, -1);
		ScreenRumble();
		m_FinalEventTrigger.SetActive(active: true);
		m_FinalEventTrigger.OnEnter += HandleFinaleEventTriggerOnEnter;
	}

	private void ScreenRumble()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.5f, m_Intensity, m_Vibration, 90f, false, false), (TweenCallback)delegate
		{
			if (!base.IsDisposed)
			{
				if (m_Vibration < m_MaxVibration)
				{
					m_Vibration += m_VibrationIncrease;
				}
				if (m_Intensity < m_MaxIntensity)
				{
					m_Intensity += m_IntensityIncrease;
				}
				ScreenRumble();
			}
		});
	}

	private void HandleFinaleEventTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_FinalEventTrigger.OnEnter -= HandleFinaleEventTriggerOnEnter;
		TweenSettingsExtensions.OnComplete<Sequence>(DOSequence(), new TweenCallback(SequenceOnComplete));
	}

	private Sequence DOSequence()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Expected O, but got Unknown
		ResetSequence();
		float num = 0f;
		float num2 = 1f;
		GameManager.Instance.Player.SetSlowed(active: true);
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.AudioManager.Play(m_VisionClip);
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			m_ConclusionController = GameManager.Instance.UIManager.Show<CH1ConclusionModalController>("UI/Modals/CH1ConclusionModalController", "MODAL");
			m_ConclusionController.ShowImage(0);
		});
		num += num2;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_FlashClip);
			m_ConclusionController.ShowImage(1);
			m_EndSmoke.SetActive(true);
		});
		num += num2;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)m_RumbleAudio.AudioSource.DOFade(0f, 2.5f));
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_FlashClip);
			m_ConclusionController.ShowImage(2);
			RenderSettings.ambientIntensity = 0f;
			ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		});
		num += num2;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.Player.SetLock(active: true);
			GameManager.Instance.Player.SetLockedMovement(active: true);
			GameManager.Instance.Player.UnEquipWeapon();
			ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.75f, m_Intensity, m_Vibration, 90f, false, true);
		});
		GameCamera gameCam = GameManager.Instance.GameCamera;
		if (gameCam.DoF)
		{
			gameCam.UnityDOF.manualDOF = true;
			float distance = gameCam.UnityDOF.focalDistance;
			TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => distance), (DOSetter<float>)delegate(float value)
			{
				distance = value;
			}, 0f, 0.75f), (Ease)1), (TweenCallback)delegate
			{
				gameCam.UnityDOF.focalDistance = distance;
			}));
		}
		GameManager.Instance.ShowScreenBlocker(0.95f, num);
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody val = Object.Instantiate<Rigidbody>(m_Axe_Physics);
			((Component)val).transform.position = GameManager.Instance.Player.WeaponGameObject.transform.position;
			((Component)val).transform.rotation = GameManager.Instance.Player.WeaponGameObject.transform.rotation;
			val.AddForce(GameManager.Instance.Player.transform.forward * 5f, (ForceMode)1);
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
		});
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(GameManager.Instance.GameCamera.transform, m_FinalEndRotation.position.y, 2f, false), (Ease)5));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotateQuaternion(GameManager.Instance.GameCamera.transform, m_FinalEndRotation.rotation, 1.5f), (Ease)5));
		num += 1f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
		});
		return m_Sequence;
	}

	private void SequenceOnComplete()
	{
		KillSequence();
		if ((Object)(object)m_RumbleAudio != (Object)null)
		{
			m_RumbleAudio.Clear();
			m_RumbleAudio = null;
		}
		if ((Object)(object)m_ConclusionController != (Object)null)
		{
			m_ConclusionController.Dispose();
		}
		SendOnComplete();
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		m_ConclusionController = null;
		m_RumbleAudio = null;
		m_RumbleAudioClip = null;
		m_VisionClip = null;
		m_FlashClip = null;
		if ((Object)(object)m_FinalEventTrigger != (Object)null)
		{
			m_FinalEventTrigger.OnEnter -= HandleFinaleEventTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
