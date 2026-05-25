using Ai;
using UnityEngine;

public class LostOneFightAi : BaseAiController
{
	[Header("Lost One Options")]
	[SerializeField]
	private GameObject[] WeaponMeshes;

	private int RandomWeapon;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		RandomWeapon = Random.Range(0, WeaponMeshes.Length);
		for (int i = 0; i < WeaponMeshes.Length; i++)
		{
			WeaponMeshes[i].SetActive(RandomWeapon == i);
		}
		m_InkDeathEffect[0].InkExplosion.ExplodeOnly();
	}

	protected override void T_EnterRetreat()
	{
		base.T_EnterRetreat();
		SetThought(AiThought.Idle);
		SetTarget(null);
	}

	protected override void T_EnterDie()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (WeaponMeshes != null && WeaponMeshes.Length > 0)
		{
			WeaponMeshes[RandomWeapon].SetActive(false);
		}
		SetTarget(null);
		((Component)this).tag = "Dead";
		SetMoveDirection(Vector3.zero);
		((Collider)m_CharacterController).enabled = false;
		CapsuleCollider component = ((Component)this).GetComponent<CapsuleCollider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			((Collider)component).enabled = false;
		}
		SetAnimationTrigger("Dead");
		if (m_InkDeathEffect != null && m_InkDeathEffect.Count > 0)
		{
			InkDeathEffect inkDeathEffect = m_InkDeathEffect[0];
			if (Object.op_Implicit((Object)(object)inkDeathEffect.InkExplosion))
			{
				inkDeathEffect.InkExplosion.OnExplode += HandleDeathOnComplete;
			}
			for (int i = 0; i < m_InkDeathEffect.Count; i++)
			{
				InkDeathEffect inkDeathEffect2 = m_InkDeathEffect[i];
				if (Object.op_Implicit((Object)(object)inkDeathEffect2.InkExplosion))
				{
					inkDeathEffect2.InkExplosion.Activate(inkDeathEffect2.Renderer, 0f, 0.3f);
				}
			}
		}
		SendOnDeath();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
