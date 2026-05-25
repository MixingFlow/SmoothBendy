using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH4PictureTaker : TMGMonoBehaviour
{
	[SerializeField]
	private MeshRenderer m_LightMesh;

	[SerializeField]
	private GameObject m_Light;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private Transform m_AudioLocation;

	[SerializeField]
	private LayerMask m_IgnoreLayers;

	private AudioClip m_FlashClip;

	private bool m_CanFlash;

	private bool m_IsInView;

	private bool m_IsFlashing;

	private float m_FlashTimerNext;

	private float m_FlashTimerRate = 60f;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_FlashClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Camera_Flash_02");
		m_CanFlash = true;
		m_IsInView = false;
		((Renderer)m_LightMesh).material.SetInt("_LightOn", 0);
		m_Light.SetActive(false);
		EnableFlash();
	}

	private void Update()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		if (m_CanFlash)
		{
			if (!m_IsInView || m_IsFlashing)
			{
				return;
			}
			if ((Object)(object)GameManager.Instance.GameCamera == (Object)null || (Object)(object)GameManager.Instance.GameCamera.transform == (Object)null)
			{
				m_CanFlash = false;
				return;
			}
			Transform val = GameManager.Instance.GameCamera.transform;
			RaycastHit val2 = default(RaycastHit);
			if (Physics.SphereCast(val.position, 0.1f, val.forward, ref val2, 100f, ~LayerMask.op_Implicit(m_IgnoreLayers)))
			{
				Debug.DrawLine(val.position, ((RaycastHit)(ref val2)).point, Color.yellow);
				if (((object)((Component)((RaycastHit)(ref val2)).collider).gameObject).Equals((object)base.gameObject))
				{
					m_IsFlashing = true;
					TweenSettingsExtensions.OnComplete<Sequence>(DOFlash(), new TweenCallback(DisableFlash));
				}
			}
		}
		else if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null) && Time.time > m_FlashTimerNext)
		{
			m_FlashTimerNext = Time.time + m_FlashTimerRate;
			EnableFlash();
		}
	}

	private Sequence DOFlash()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, 0f, (TweenCallback)delegate
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.AudioManager.PlayAtPosition(m_FlashClip, m_AudioLocation.position);
			((Renderer)m_LightMesh).material.SetInt("_LightOn", 1);
			m_Light.SetActive(true);
		});
		TweenSettingsExtensions.InsertCallback(val, 0.2f, (TweenCallback)delegate
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.A_LITTLE_SOUVENIR);
			((Renderer)m_LightMesh).material.SetInt("_LightOn", 0);
			m_Light.SetActive(false);
		});
		return val;
	}

	private void EnableFlash()
	{
		m_CanFlash = true;
		m_IsFlashing = false;
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
		m_EventTrigger.OnExit += HandleEventTriggerOnExit;
	}

	private void DisableFlash()
	{
		m_CanFlash = false;
		m_FlashTimerNext = Time.time + m_FlashTimerRate;
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		m_EventTrigger.OnExit -= HandleEventTriggerOnExit;
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		Debug.Log((object)"OnEnter");
		m_IsInView = true;
	}

	private void HandleEventTriggerOnExit(object sender, EventArgs e)
	{
		Debug.Log((object)"OnExit");
		m_IsInView = false;
	}

	protected override void OnDisposed()
	{
		m_FlashClip = null;
		base.OnDisposed();
	}
}
