using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class InkExplosionEffect : TMGMonoBehaviour
{
	private const string MELT_PERCENTAGE = "_MeltPercentage";

	[SerializeField]
	private ParticleSystem m_Explosion;

	[SerializeField]
	private ParticleSystem m_Drops;

	private Renderer m_Renderer;

	public event EventHandler OnExplode;

	public override void Init()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		CollisionModule collision = m_Drops.collision;
		((CollisionModule)(ref collision)).quality = (ParticleSystemCollisionQuality)0;
		((CollisionModule)(ref collision)).radiusScale = 0.001f;
	}

	public void Birth(Renderer _renderer)
	{
		if ((Object)(object)m_Renderer == (Object)null)
		{
			m_Renderer = _renderer;
		}
		Explode(10, 10);
	}

	public void ExplodeOnly()
	{
		Explode(10, 10);
	}

	public void Activate(Renderer _renderer, float meltStart = 2f, float meltDuration = 2f)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		if ((Object)(object)m_Renderer == (Object)null)
		{
			m_Renderer = _renderer;
		}
		Sequence val = DOTween.Sequence();
		float num = meltStart;
		if (m_Renderer.material.HasProperty("_MeltPercentage"))
		{
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(m_Renderer.material, 1f, "_MeltPercentage", meltDuration), (Ease)1));
		}
		num += meltDuration;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(DeathExplode));
		num += 2f;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(SendOnExplode));
	}

	private void DeathExplode()
	{
		Explode(20, 10);
		m_Renderer.enabled = false;
	}

	private void Explode(int explosionCount, int dropCount)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.transform.SetParent((Transform)null);
		ShapeModule shape = m_Explosion.shape;
		Renderer renderer = m_Renderer;
		((ShapeModule)(ref shape)).skinnedMeshRenderer = (SkinnedMeshRenderer)(object)((renderer is SkinnedMeshRenderer) ? renderer : null);
		m_Explosion.Emit(explosionCount);
		ShapeModule shape2 = m_Drops.shape;
		Renderer renderer2 = m_Renderer;
		((ShapeModule)(ref shape2)).skinnedMeshRenderer = (SkinnedMeshRenderer)(object)((renderer2 is SkinnedMeshRenderer) ? renderer2 : null);
		m_Drops.Emit(dropCount);
	}

	private void SendOnExplode()
	{
		this.OnExplode.Send(this);
		Dispose();
	}

	public void Reset()
	{
		this.OnExplode = null;
		m_Renderer.material.SetFloat("_MeltPercentage", 0f);
		m_Renderer.enabled = true;
	}

	protected override void OnDisposed()
	{
		m_Renderer = null;
		this.OnExplode = null;
		base.OnDisposed();
	}
}
