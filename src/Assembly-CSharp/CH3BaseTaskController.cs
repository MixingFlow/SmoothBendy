using System;
using UnityEngine;

public class CH3BaseTaskController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	protected CH3WeaponStationController m_WeaponStationController;

	[SerializeField]
	protected CH3BendyController m_BendyController;

	[SerializeField]
	protected CH3SearcherController m_SearcherController;

	[SerializeField]
	protected CH3GangsterController m_GangsterController;

	[HideInInspector]
	public int ID;

	protected BaseWeapon m_Weapon;

	public BaseWeapon Weapon => m_Weapon;

	public event EventHandler OnReady;

	public event EventHandler OnBegin;

	public event EventHandler OnRestart;

	public override void Activate()
	{
		if (Object.op_Implicit((Object)(object)m_Weapon))
		{
			m_Weapon.gameObject.SetActive(true);
		}
	}

	protected void SetWeapon(BaseWeapon weapon)
	{
		m_Weapon = weapon;
		if (Object.op_Implicit((Object)(object)m_Weapon))
		{
			m_Weapon.Interaction.SetActive(active: false);
			m_Weapon.gameObject.SetActive(false);
		}
	}

	protected void EnableWeapon()
	{
		if (Object.op_Implicit((Object)(object)m_Weapon))
		{
			m_WeaponStationController.OnOpenComplete += HandleOnOpenComplete;
			m_WeaponStationController.Open();
		}
	}

	private void HandleOnOpenComplete(object sender, EventArgs e)
	{
		m_WeaponStationController.OnOpenComplete -= HandleOnOpenComplete;
		m_Weapon.OnEquipped += HandleWeaponOnEquipped;
		m_Weapon.Interaction.SetActive(active: true);
	}

	private void HandleWeaponOnEquipped(object sender, EventArgs e)
	{
		m_Weapon.OnEquipped -= HandleWeaponOnEquipped;
		m_WeaponStationController.Close();
		m_WeaponStationController.Unblock();
		BeginTask();
	}

	protected virtual void BeginTask()
	{
	}

	protected void ActivateDropbox()
	{
		m_WeaponStationController.OnTaskComplete += HandleTaskOnComplete;
		m_WeaponStationController.ActivateDropbox();
	}

	private void HandleTaskOnComplete(object sender, EventArgs e)
	{
		m_WeaponStationController.OnTaskComplete -= HandleTaskOnComplete;
		SendOnComplete();
	}

	public void SendOnBegin()
	{
		this.OnBegin.Send(this);
	}

	public void SendOnReady()
	{
		this.OnReady.Send(this);
	}

	public void SendOnRestart()
	{
		this.OnRestart.Send(this);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
