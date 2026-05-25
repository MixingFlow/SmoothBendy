using TMG.Core;
using UnityEngine;

public class PhysicsObjectToWeapon : TMGMonoBehaviour
{
	[SerializeField]
	private BaseWeapon m_WeaponToSpawn;

	[SerializeField]
	private Rigidbody m_Rigidbody;

	private float enableTimer = 1.5f;

	private void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (enableTimer <= 0f)
		{
			Vector3 velocity = m_Rigidbody.velocity;
			if (((Vector3)(ref velocity)).magnitude < 10f)
			{
				BaseWeapon baseWeapon = Object.Instantiate<BaseWeapon>(m_WeaponToSpawn, base.transform.position, base.transform.rotation);
				baseWeapon.gameObject.SetActive(true);
				baseWeapon.Interaction.SetActive(active: true);
				Dispose();
			}
		}
		else
		{
			enableTimer -= Time.deltaTime;
		}
	}
}
