using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponInfo
{
	public int Damage;

	public ImpactType ImpactType;

	public List<AudioClip> Audio;

	public GameObject Attacker;

	public bool IsBullet;
}
