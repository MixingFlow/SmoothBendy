using System;
using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3GearPanel : TMGMonoBehaviour
{
	[HideInInspector]
	public int ID;

	[HideInInspector]
	public bool isComplete;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_Panel;

	[SerializeField]
	private List<Transform> m_Bolts;

	[Header("Gears")]
	[SerializeField]
	private Interactable m_Gear;

	[SerializeField]
	private List<Transform> m_Gears;

	[Header("Lights")]
	[SerializeField]
	private LightFlicker m_Light;

	[Header("Options")]
	[SerializeField]
	private bool m_IsEmpty;

	private AudioClip m_OpenClip;

	private AudioClip m_TakeGearClip;

	private Interactable m_Interactable;

	public List<Transform> Bolts => m_Bolts;

	public event EventHandler OnComplete;

	public override void Init()
	{
		base.Init();
		m_Interactable = base.gameObject.GetComponent<Interactable>();
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_OpenClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_gearmission_gearboxcoveropen");
		m_TakeGearClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_gearmission_takegear");
	}

	public void Activate()
	{
		m_Light.TurnOff();
		m_Gear.gameObject.SetActive(!m_IsEmpty);
		if (m_IsEmpty)
		{
			m_Light.TurnOn();
		}
	}

	public void ActivateInteraction()
	{
		m_Interactable.OnInteracted += HandleOnInteracted;
		m_Interactable.SetActive(active: true);
	}

	public void RotateGears()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (!isComplete && !m_IsEmpty)
		{
			Sequence val = DOTween.Sequence();
			for (int i = 0; i < m_Gears.Count; i++)
			{
				Transform val2 = m_Gears[i];
				TweenSettingsExtensions.Insert(val, 0.25f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(val2, new Vector3(0f, 51f, 0f), 0.5f, (RotateMode)3), (Ease)27));
			}
			if ((Object)(object)m_Gear != (Object)null)
			{
				TweenSettingsExtensions.Insert(val, 0.25f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Gear.transform, new Vector3(0f, -51f, 0f), 0.5f, (RotateMode)3), (Ease)27));
			}
			TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(RotateGears));
		}
	}

	private void HandleOnInteracted(object sender, EventArgs e)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		m_Interactable.OnInteracted -= HandleOnInteracted;
		for (int i = 0; i < m_Bolts.Count; i++)
		{
			((Component)m_Bolts[i]).gameObject.SetActive(false);
		}
		GameManager.Instance.AudioManager.Play(m_OpenClip);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Panel, new Vector3(135f, 0f, 0f), 0.65f, (RotateMode)3), (Ease)1), (TweenCallback)delegate
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			((Collider)((Component)this).GetComponent<BoxCollider>()).enabled = false;
			if (m_IsEmpty)
			{
				this.OnComplete.Send(this);
			}
			else
			{
				m_Gear.SetActive(active: true);
				m_Gear.OnInteracted += HandleGearOnInteracted;
			}
			TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(ShortcutExtensions.DOLocalRotate(m_Panel, new Vector3(-10f, 0f, 0f), 2f, (RotateMode)3), 10, (LoopType)1), (Ease)7);
		});
	}

	private void HandleGearOnInteracted(object sender, EventArgs e)
	{
		m_Gear.OnInteracted -= HandleGearOnInteracted;
		m_Gear.Dispose();
		GameManager.Instance.AudioManager.Play(m_TakeGearClip);
		m_Light.TurnOn();
		isComplete = true;
		this.OnComplete.Send(this);
	}

	public void DisableInteraction()
	{
		if (Object.op_Implicit((Object)(object)m_Interactable))
		{
			m_Interactable.SetActive(active: false);
			m_Interactable.OnInteracted -= HandleOnInteracted;
		}
	}

	public void ForceComplete()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		m_Panel.localEulerAngles = new Vector3(135f, 0f, 0f);
		((Collider)((Component)this).GetComponent<BoxCollider>()).enabled = false;
		if (Object.op_Implicit((Object)(object)m_Gear))
		{
			m_Gear.gameObject.SetActive(false);
		}
		for (int i = 0; i < m_Gears.Count; i++)
		{
			Transform val = m_Gears[i];
			if (Object.op_Implicit((Object)(object)val))
			{
				ShortcutExtensions.DOKill((Component)(object)val, false);
			}
		}
		for (int j = 0; j < m_Bolts.Count; j++)
		{
			((Component)m_Bolts[j]).gameObject.SetActive(false);
		}
		isComplete = true;
		m_Light.TurnOn();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_Gear))
		{
			m_Gear.OnInteracted -= HandleGearOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_Interactable))
		{
			m_Interactable.OnInteracted -= HandleOnInteracted;
		}
		base.OnDisposed();
	}
}
