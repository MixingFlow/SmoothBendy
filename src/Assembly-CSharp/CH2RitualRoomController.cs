using System;
using UnityEngine;

public class CH2RitualRoomController : BaseController
{
	[Header("Objective: Leave the room!")]
	[SerializeField]
	private Breakable m_Plank;

	[SerializeField]
	private BaseDoorController m_Door;

	[Header("Jumpscare: Plank Break")]
	[SerializeField]
	private Breakable m_ScarePlank;

	[SerializeField]
	private EventTrigger m_ScareTrigger;

	[Header("Weapon")]
	[SerializeField]
	private MeleeWeapon m_Axe;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ScareTrigger.SetActive(active: false);
		m_Door.Lock();
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.RitualObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_Door.Lock();
		m_Plank.OnBroken += HandlePlankOnBroken;
	}

	private void HandlePlankOnBroken(object sender, EventArgs e)
	{
		m_Plank.OnBroken -= HandlePlankOnBroken;
		m_Door.Unlock();
		m_ScareTrigger.SetActive(active: true);
		m_ScareTrigger.OnEnter += HandleScareTriggerOnEnter;
	}

	private void HandleScareTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		m_ScareTrigger.OnEnter -= HandleScareTriggerOnEnter;
		m_ScarePlank.Destroy(m_ScarePlank.transform.position + Vector3.up * 0.5f);
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.RitualObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void ForceComplete()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		GameManager.Instance.Player.WeaponGameObject = m_Axe.gameObject;
		GameManager.Instance.Player.EquipWeapon();
		if (Object.op_Implicit((Object)(object)m_Axe) && (Object)(object)m_Axe.Interaction != (Object)null)
		{
			m_Axe.Interaction.SetActive(active: false);
		}
		m_Axe.KillInteraction();
		m_Axe.Equip();
		m_Axe.transform.SetParent(GameManager.Instance.Player.WeaponParent);
		m_Axe.transform.localPosition = Vector3.zero;
		m_Axe.transform.localEulerAngles = Vector3.zero;
		m_Plank.gameObject.SetActive(false);
		m_ScarePlank.gameObject.SetActive(false);
		m_Door.ForceOpen(145f);
		m_Door.Lock();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
