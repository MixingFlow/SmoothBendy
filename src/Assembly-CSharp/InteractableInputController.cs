using System;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

[Serializable]
public class InteractableInputController : TMGAbstractDisposable
{
	[SerializeField]
	private bool m_Active = true;

	[SerializeField]
	private LayerMask m_IgnoreLayers;

	[SerializeField]
	private float m_LookDistance = 8f;

	[SerializeField]
	private float m_SphereCastThickness = 0.1f;

	[SerializeField]
	private bool DebugLines = true;

	public bool HasWeapon;

	public Interactable Interactable { get; private set; }

	public void Init()
	{
	}

	public void UpdateInteraction(Vector3 origin, Vector3 direction)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (m_Active)
		{
			UpdateInteraction(origin, direction, m_LookDistance);
		}
	}

	public void UpdateInteraction(Vector3 origin, Vector3 direction, float distance)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Active)
		{
			return;
		}
		RaycastHit val = default(RaycastHit);
		if (Physics.SphereCast(origin, m_SphereCastThickness, direction, ref val, distance, ~LayerMask.op_Implicit(m_IgnoreLayers)))
		{
			Interactable component = ((Component)((RaycastHit)(ref val)).transform).GetComponent<Interactable>();
			if (Object.op_Implicit((Object)(object)component))
			{
				DrawDebugLine(origin, ((RaycastHit)(ref val)).point, Color.yellow);
				if ((Object)(object)Interactable != (Object)(object)component)
				{
					ExitInteraction();
				}
				Interactable = component;
				if (PlayerInput.InteractOnPressed())
				{
					Interact();
				}
				else
				{
					EnterInteraction();
				}
			}
			else
			{
				ExitInteraction();
			}
		}
		else
		{
			ExitInteraction();
		}
	}

	private void Interact()
	{
		if (Object.op_Implicit((Object)(object)Interactable))
		{
			Interactable.Interact();
		}
	}

	private void EnterInteraction()
	{
		if (Object.op_Implicit((Object)(object)Interactable))
		{
			Interactable.InteractEnter();
		}
	}

	private void ExitInteraction()
	{
		if (Object.op_Implicit((Object)(object)Interactable))
		{
			Interactable.InteractExit();
			Interactable = null;
		}
	}

	private void DrawDebugLine(Vector3 start, Vector3 end, Color color)
	{
	}

	public void SetActive(bool active)
	{
		m_Active = active;
	}

	private void SetDistance(float distance)
	{
		m_LookDistance = distance;
	}

	protected override void OnDisposed()
	{
		Interactable = null;
		base.OnDisposed();
	}
}
