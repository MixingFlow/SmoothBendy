using System;
using S13Audio;
using UnityEngine;

public class Holdable : Interactable
{
	private enum HoldableType
	{
		INK
	}

	[SerializeField]
	private HoldableType m_HoldableType;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_HoldModel;

	[SerializeField]
	private Transform m_HoldTransform;

	[Header("Effects")]
	[SerializeField]
	private GameObject m_PickUpEffects;

	public event EventHandler OnHeld;

	public override void OnInteract()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject) && Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			ResetInteraction();
			return;
		}
		base.OnInteract();
		if (Object.op_Implicit((Object)(object)m_PickUpEffects))
		{
			m_PickUpEffects.transform.SetParent((Transform)null);
			m_PickUpEffects.SetActive(true);
			Object.Destroy((Object)(object)m_PickUpEffects, 3f);
		}
		m_HoldModel.SetParent(m_HoldTransform);
		m_HoldModel.localPosition = Vector3.zero;
		m_HoldModel.localEulerAngles = Vector3.zero;
		base.transform.SetParent(GameManager.Instance.Player.WeaponParent);
		base.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localScale = Vector3.one;
		SetActive(active: false);
		PlayerController player = GameManager.Instance.Player;
		if (Object.op_Implicit((Object)(object)player.WeaponGameObject))
		{
			player.InactiveWeapon = player.WeaponGameObject;
			player.InactiveWeapon.SetActive(false);
		}
		player.WeaponGameObject = base.gameObject;
		player.UnEquipWeapon();
		if (Object.op_Implicit((Object)(object)m_HoldModel))
		{
			((Component)m_HoldModel).gameObject.layer = LayerMask.NameToLayer("Weapon");
			Transform[] componentsInChildren = ((Component)m_HoldModel).GetComponentsInChildren<Transform>(true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					((Component)componentsInChildren[i]).gameObject.layer = LayerMask.NameToLayer("Weapon");
				}
			}
		}
		if (m_HoldableType == HoldableType.INK)
		{
			S13AudioManager.Instance.InvokeEvent("evt_ink_collected");
		}
		this.OnHeld.Send(this);
	}

	public void Remove()
	{
		if (m_HoldableType == HoldableType.INK)
		{
			S13AudioManager.Instance.InvokeEvent("evt_ink_deposited");
		}
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			GameManager.Instance.Player.WeaponGameObject = GameManager.Instance.Player.InactiveWeapon;
			GameManager.Instance.Player.WeaponGameObject.SetActive(true);
			GameManager.Instance.Player.EquipWeapon();
			GameManager.Instance.Player.InactiveWeapon = null;
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
