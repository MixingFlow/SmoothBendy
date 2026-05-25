using UnityEngine;

public interface IHittable
{
	void Hit(RaycastHit hit, WeaponInfo weaponInfo = null);
}
