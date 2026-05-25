using System;
using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class CH3Flashlight : TMGMonoBehaviour
{
	[SerializeField]
	private List<Light> m_Lights;

	[SerializeField]
	private Interactable m_Interaction;

	public event EventHandler OnInteracted;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_Lights.Count; i++)
		{
			((Component)m_Lights[i]).gameObject.SetActive(false);
		}
		m_Interaction.SetActive(active: false);
	}

	public void Activate()
	{
		m_Interaction.SetActive(active: true);
		m_Interaction.OnInteracted += HandleInteractionOnInteracted;
	}

	private void HandleInteractionOnInteracted(object sender, EventArgs e)
	{
		Equip();
		this.OnInteracted.Send(this);
	}

	public void Equip()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		m_Interaction.OnInteracted -= HandleInteractionOnInteracted;
		m_Interaction.SetActive(active: false);
		m_Interaction.gameObject.layer = LayerMask.NameToLayer("Weapon");
		GameManager.Instance.Player.WeaponGameObject = base.gameObject;
		GameManager.Instance.Player.UnEquipWeapon();
		base.transform.SetParent(GameManager.Instance.Player.WeaponParent);
		base.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
		for (int i = 0; i < m_Lights.Count; i++)
		{
			((Component)m_Lights[i]).gameObject.SetActive(true);
		}
		GameManager.Instance.Player.ScaleWeapon();
	}

	protected override void OnDisposed()
	{
		m_Interaction.OnInteracted -= HandleInteractionOnInteracted;
		base.OnDisposed();
	}
}
