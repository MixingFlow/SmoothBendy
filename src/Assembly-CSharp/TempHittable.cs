using System;
using TMG.Core;
using UnityEngine;

public class TempHittable : TMGMonoBehaviour, IHittable
{
	public event EventHandler OnHit;

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo)
	{
		this.OnHit.Send(this);
		Dispose();
	}
}
