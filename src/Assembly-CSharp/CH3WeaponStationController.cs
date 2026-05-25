using System;
using DG.Tweening;
using UnityEngine;

public class CH3WeaponStationController : BaseController
{
	[Header("Weapon Station")]
	[SerializeField]
	private CH3WeaponStation m_WeaponStation;

	[SerializeField]
	private CH3Dropbox m_Dropbox;

	[SerializeField]
	private CH3BridgeBlocker m_BridgeBlocker;

	private AudioClip m_DropboxClip;

	public CH3WeaponStation WeaponStation => m_WeaponStation;

	public event EventHandler OnTaskComplete;

	public event EventHandler OnOpenComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_DropboxClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_dropboxinteract");
		m_BridgeBlocker.ForceOpen();
	}

	public void Block()
	{
		m_BridgeBlocker.Close();
	}

	public void Unblock()
	{
		m_BridgeBlocker.Open();
	}

	public void Open()
	{
		m_WeaponStation.OnOpen += HandleWeaponStationOnOpen;
		m_WeaponStation.Open();
	}

	private void HandleWeaponStationOnOpen(object sender, EventArgs e)
	{
		m_WeaponStation.OnOpen -= HandleWeaponStationOnOpen;
		this.OnOpenComplete.Send(this);
	}

	public void Close()
	{
		m_WeaponStation.OnClose += HandleWeaponStationOnClose;
		m_WeaponStation.Close();
	}

	private void HandleWeaponStationOnClose(object sender, EventArgs e)
	{
		m_WeaponStation.OnClose -= HandleWeaponStationOnClose;
	}

	public void ActivateDropbox()
	{
		m_Dropbox.OnInteracted += HandleDropboxOnInteracted;
		m_Dropbox.OnDopped += HandleDropboxOnDropped;
		m_Dropbox.SetActive(active: true);
	}

	private void HandleDropboxOnInteracted(object sender, EventArgs e)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		m_Dropbox.OnInteracted -= HandleDropboxOnInteracted;
		m_Dropbox.SetActive(active: false);
		GameManager.Instance.AudioManager.PlayAtPosition(m_DropboxClip, m_Dropbox.transform.position);
		PlayerController player = GameManager.Instance.Player;
		if (Object.op_Implicit((Object)(object)player.InactiveWeapon))
		{
			Object.Destroy((Object)(object)player.WeaponGameObject);
			player.WeaponGameObject = player.InactiveWeapon;
			player.WeaponGameObject.SetActive(true);
			player.WeaponGameObject.transform.localPosition = Vector3.zero;
			player.WeaponGameObject.transform.localEulerAngles = Vector3.zero;
			player.InactiveWeapon = null;
		}
	}

	private void HandleDropboxOnDropped(object sender, EventArgs e)
	{
		m_Dropbox.OnDopped -= HandleDropboxOnDropped;
		this.OnTaskComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnTaskComplete = null;
		if (Object.op_Implicit((Object)(object)m_WeaponStation))
		{
			m_WeaponStation.OnOpen -= HandleWeaponStationOnOpen;
		}
		if (Object.op_Implicit((Object)(object)m_WeaponStation))
		{
			m_WeaponStation.OnClose -= HandleWeaponStationOnClose;
		}
		if (Object.op_Implicit((Object)(object)m_Dropbox))
		{
			m_Dropbox.OnInteracted -= HandleDropboxOnInteracted;
			m_Dropbox.OnDopped -= HandleDropboxOnDropped;
		}
		m_DropboxClip = null;
		ShortcutExtensions.DOKill((Component)(object)m_BridgeBlocker, false);
		base.OnDisposed();
	}
}
