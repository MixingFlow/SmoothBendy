using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class PlayerDeath
{
	[Header("Explosion")]
	[SerializeField]
	public ParticleSystem Explosion;

	[SerializeField]
	public int ExplosionEmitCount;

	[Header("Drops")]
	[SerializeField]
	public ParticleSystem Drops;

	[SerializeField]
	public int DropEmitCount;

	public void Play()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.15f, (TweenCallback)delegate
		{
			Explosion.Emit(ExplosionEmitCount);
			Drops.Emit(DropEmitCount);
		});
	}
}
