using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3WeaponStation : TMGMonoBehaviour
{
	[Header("Door")]
	[SerializeField]
	private Transform m_Door;

	[Header("Weapon Positions")]
	[SerializeField]
	private Transform m_WrenchLocation;

	[SerializeField]
	private Transform m_InkToolLocation;

	[SerializeField]
	private Transform m_PlungerLocation;

	[SerializeField]
	private Transform m_AxeLocation;

	[SerializeField]
	private Transform m_TommyGunLocation;

	private AudioClip m_DoorClip;

	public MeleeWeapon m_Wrench { get; private set; }

	public MeleeWeapon m_InkTool { get; private set; }

	public MeleeWeapon m_Plunger { get; private set; }

	public MeleeWeapon m_Axe { get; private set; }

	public GunWeapon m_TommyGun { get; private set; }

	public CH3MeltingTommyGun m_TommyGunFake { get; private set; }

	public event EventHandler OnOpen;

	public event EventHandler OnClose;

	public override void Init()
	{
		base.Init();
		m_DoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_weapongiver");
		m_Wrench = CreateWeapon<MeleeWeapon>(m_WrenchLocation, "GamePlay/Weapons/Weapon_Wrench");
		m_InkTool = CreateWeapon<MeleeWeapon>(m_InkToolLocation, "GamePlay/Weapons/Weapon_InkTool");
		m_Plunger = CreateWeapon<MeleeWeapon>(m_PlungerLocation, "GamePlay/Weapons/Weapon_Plunger");
		m_Axe = CreateWeapon<MeleeWeapon>(m_AxeLocation, "GamePlay/Weapons/Weapon_Axe");
		m_TommyGun = CreateWeapon<GunWeapon>(m_TommyGunLocation, "GamePlay/Weapons/Weapon_TommyGun");
		m_TommyGunFake = CreateWeapon<CH3MeltingTommyGun>(m_TommyGunLocation, "GamePlay/Weapons/Weapon_TommyGunMelt");
	}

	private T CreateWeapon<T>(Transform location, string asset) where T : Component
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		T val = GameManager.Instance.AssetManager.CreateAsset<T>(asset);
		((Component)val).transform.SetParent(location);
		((Component)val).transform.localPosition = Vector3.zero;
		((Component)val).transform.localEulerAngles = Vector3.zero;
		return ((Component)val/*cast due to constrained. prefix*/).GetComponent<T>();
	}

	public void Open()
	{
		ActualOpen();
	}

	private void ActualOpen()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		GameManager.Instance.AudioManager.PlayAtPosition(m_DoorClip, m_Door.position);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_Door, new Vector3(0f, 0f, -180f), 2f, (RotateMode)3), (Ease)6), new TweenCallback(SendOnOpen));
	}

	private void SendOnOpen()
	{
		this.OnOpen.Send(this);
	}

	public void Close()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		GameManager.Instance.AudioManager.PlayAtPosition(m_DoorClip, m_Door.position);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_Door, new Vector3(0f, 0f, 180f), 2f, (RotateMode)3), (Ease)6), new TweenCallback(SendOnClose));
	}

	private void SendOnClose()
	{
		this.OnClose.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnOpen = null;
		this.OnClose = null;
		m_DoorClip = null;
		base.OnDisposed();
	}
}
