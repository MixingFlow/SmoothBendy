using System.Collections.Generic;
using UnityEngine;

public class PACustomBillboardGenerator : PABillboardParticle, PAICustomParticleGenerator
{
	public List<PACustomParticle> particles;

	[HideInInspector]
	[SerializeField]
	private Mesh particleMesh;

	private Vector2[] particleUVs = (Vector2[])(object)new Vector2[4];

	protected override void UpdateCache(PAParticleField settings)
	{
	}

	protected override int SetParticleCapacity(int count)
	{
		return particles.Count;
	}

	private void Awake()
	{
		PAParticleField component = ((Component)this).GetComponent<PAParticleField>();
		if (Object.op_Implicit((Object)(object)component) && component.generatorType != PAParticleField.ParticleType.Custom)
		{
			component.generatorType = PAParticleField.ParticleType.Custom;
		}
	}

	private void OnValidate()
	{
		PAParticleField component = ((Component)this).GetComponent<PAParticleField>();
		if (Object.op_Implicit((Object)(object)component) && component.generatorType != PAParticleField.ParticleType.Custom)
		{
			component.generatorType = PAParticleField.ParticleType.Custom;
		}
		particles.ForEach(delegate(PACustomParticle obj)
		{
			obj.SetDefaultValuesIfUninitialized();
		});
	}

	[ContextMenu("Apply Particles")]
	public void ApplyParticles()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (particles != null && particles.Count > 0)
		{
			int num = 4;
			SetArraySizes(particles.Count * num, particles.Count * 3 * 2);
			for (int i = 0; i < particles.Count; i++)
			{
				PACustomParticle pACustomParticle = particles[i];
				SetOriginDirection(i, pACustomParticle.originDirection);
				SetSize(i, pACustomParticle.size);
				SetColor(i, pACustomParticle.color);
				SetSpeed(i, pACustomParticle.speed);
				SetSpinSpeed(i, pACustomParticle.spinSpeed);
				SetUV(i, pACustomParticle.uv);
				SetIndex(i, (float)i / (float)particles.Count);
			}
		}
		else
		{
			SetArraySizes(0, 0);
		}
		UpdateTriangles(0);
		if ((Object)(object)particleMesh == (Object)null)
		{
			particleMesh = ((Component)this).GetComponent<MeshFilter>().sharedMesh;
		}
		FillMesh(particleMesh);
	}

	private void SetOriginDirection(int particleIndex, Vector3 direction)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			int num = particleIndex * 4 + i;
			verts[num] = direction;
		}
	}

	private void SetSpeed(int particleIndex, float speed)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = particleIndex * 4 + i;
			normals[num].x = speed;
		}
	}

	private void SetSpinSpeed(int particleIndex, float spinSpeed)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = particleIndex * 4 + i;
			normals[num].y = spinSpeed;
		}
	}

	private void SetColor(int particleIndex, Color color)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			int num = particleIndex * 4 + i;
			colors[num] = color;
		}
	}

	private void SetUV(int particleIndex, Rect uv)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		ref Vector2 reference = ref particleUVs[0];
		reference = new Vector2(((Rect)(ref uv)).x + ((Rect)(ref uv)).width, ((Rect)(ref uv)).y);
		ref Vector2 reference2 = ref particleUVs[1];
		reference2 = new Vector2(((Rect)(ref uv)).x + ((Rect)(ref uv)).width, ((Rect)(ref uv)).y + ((Rect)(ref uv)).height);
		ref Vector2 reference3 = ref particleUVs[2];
		reference3 = new Vector2(((Rect)(ref uv)).x, ((Rect)(ref uv)).y + ((Rect)(ref uv)).height);
		ref Vector2 reference4 = ref particleUVs[3];
		reference4 = new Vector2(((Rect)(ref uv)).x, ((Rect)(ref uv)).y);
		for (int i = 0; i < 4; i++)
		{
			int num = particleIndex * 4 + i;
			ref Vector2 reference5 = ref uv0[num];
			reference5 = particleUVs[i];
		}
	}

	private void SetSize(int particleIndex, float size)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			int num = particleIndex * 4 + i;
			ref Vector2 reference = ref uv1[num];
			reference = PABillboardParticle.quadOffsets[i] * size;
		}
	}

	private void SetIndex(int particleIndex, float normalizedIndex)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = particleIndex * 4 + i;
			normals[num].z = normalizedIndex;
		}
	}
}
