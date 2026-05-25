using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class CH3InkBullet : TMGMonoBehaviour
{
	[SerializeField]
	private ParticleSystem m_Particles;

	private List<ParticleCollisionEvent> m_ParticleCollisionEvents;

	public void OnParticleCollision(GameObject other)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		int safeCollisionEventSize = ParticlePhysicsExtensions.GetSafeCollisionEventSize(m_Particles);
		m_ParticleCollisionEvents = null;
		m_ParticleCollisionEvents = new List<ParticleCollisionEvent>(safeCollisionEventSize);
		int collisionEvents = ParticlePhysicsExtensions.GetCollisionEvents(m_Particles, other, m_ParticleCollisionEvents);
		for (int i = 0; i < collisionEvents; i++)
		{
			ParticleCollisionEvent val = m_ParticleCollisionEvents[i];
			IHittable component = other.GetComponent<IHittable>();
			if (component != null)
			{
				RaycastHit hit = default(RaycastHit);
				((RaycastHit)(ref hit)).point = ((ParticleCollisionEvent)(ref val)).intersection;
				((RaycastHit)(ref hit)).normal = ((ParticleCollisionEvent)(ref val)).normal;
				WeaponInfo weaponInfo = new WeaponInfo();
				weaponInfo.Damage = 2;
				weaponInfo.ImpactType = ImpactType.BLUNT;
				weaponInfo.IsBullet = true;
				weaponInfo.Attacker = GameManager.Instance.Player.gameObject;
				weaponInfo.Audio = new List<AudioClip>();
				weaponInfo.Audio.Add(GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/SFX/Gun/SFX_Ink_Gun_Impact"));
				component.Hit(hit, weaponInfo);
			}
		}
	}

	protected override void OnDisposed()
	{
		m_ParticleCollisionEvents = null;
		base.OnDisposed();
	}
}
