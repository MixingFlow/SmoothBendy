using UnityEngine;

public class PABillboardParticle : PAParticleMeshGenerator
{
	private const int MAX_PARTICLE_COUNT = 16250;

	protected static readonly Vector2[] quadUVs = (Vector2[])(object)new Vector2[4]
	{
		new Vector2(1f, 0f),
		new Vector2(1f, 1f),
		new Vector2(0f, 1f),
		new Vector2(0f, 0f)
	};

	protected static readonly Vector2[] quadOffsets = (Vector2[])(object)new Vector2[4]
	{
		new Vector2(-0.5f, -0.5f),
		new Vector2(-0.5f, 0.5f),
		new Vector2(0.5f, 0.5f),
		new Vector2(0.5f, -0.5f)
	};

	public override int GetMaximumParticleCount()
	{
		return 16250;
	}

	protected override int SetParticleCapacity(int count)
	{
		count = GetClampedParticleCount(count);
		int result = count;
		if (count * 4 > verts.Length)
		{
			result = verts.Length / 4;
		}
		SetArraySizes(count * 4, count * 6);
		return result;
	}

	protected override void UpdateDirection(PAParticleField settings, int startAt)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		SkipRandomCalls(3, startAt);
		Vector3 val = default(Vector3);
		for (int i = startAt; i < settings.particleCount; i++)
		{
			((Vector3)(ref val))._002Ector(GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f, 1f));
			for (int j = 0; j < 4; j++)
			{
				int num = i * 4 + j;
				verts[num] = val;
			}
		}
	}

	protected override void UpdateColor(PAParticleField settings, int startAt)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		SkipRandomCalls(1, startAt);
		for (int i = startAt; i < settings.particleCount; i++)
		{
			Color val = settings.colorVariation.Evaluate(GetRandomAndIncrement(0f, 1f));
			for (int j = 0; j < 4; j++)
			{
				int num = i * 4 + j;
				colors[num] = val;
			}
		}
	}

	protected override void UpdateSpeed(PAParticleField settings, int startAt)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		SkipRandomCalls(2, startAt);
		for (int i = startAt; i < settings.particleCount; i++)
		{
			Vector2 val = Vector2.op_Implicit(new Vector3(GetRandomAndIncrement(settings.minimumSpeed, 1f), GetRandomAndIncrement(settings.minSpinSpeed, 1f)));
			for (int j = 0; j < 4; j++)
			{
				int num = i * 4 + j;
				normals[num].x = val.x;
				normals[num].y = val.y;
			}
		}
	}

	protected override void UpdateSurface(PAParticleField settings, int startAt)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		SkipRandomCalls(3, startAt);
		float num = ((settings.textureType == PAParticleField.TextureType.Simple) ? 1f : ((float)settings.spriteColumns));
		float num2 = ((settings.textureType == PAParticleField.TextureType.Simple) ? 1f : ((float)settings.spriteRows));
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(1f / num, 1f / num2);
		Vector2 val2 = default(Vector2);
		for (int i = startAt; i < settings.particleCount; i++)
		{
			((Vector2)(ref val2))._002Ector((float)(int)GetRandomAndIncrement(0f, num), (float)(int)GetRandomAndIncrement(0f, num2));
			float randomAndIncrement = GetRandomAndIncrement(settings.minimumSize, 1f);
			for (int j = 0; j < 4; j++)
			{
				int num3 = i * 4 + j;
				ref Vector2 reference = ref uv0[num3];
				reference = Vector2.Scale(quadUVs[j] + val2, val);
				ref Vector2 reference2 = ref uv1[num3];
				reference2 = quadOffsets[j] * randomAndIncrement + settings.pivotOffset * randomAndIncrement;
			}
		}
	}

	protected override void UpdateTriangles(int startAt)
	{
		for (int i = startAt; i < triangles.Length / 6; i++)
		{
			triangles[i * 6] = i * 4 + 2;
			triangles[i * 6 + 1] = i * 4 + 1;
			triangles[i * 6 + 2] = i * 4;
			triangles[i * 6 + 3] = i * 4 + 2;
			triangles[i * 6 + 4] = i * 4;
			triangles[i * 6 + 5] = i * 4 + 3;
		}
	}

	protected override void UpdateIndicies()
	{
		float num = normals.Length / 4;
		for (int i = 0; i < normals.Length; i += 4)
		{
			float z = (float)(i / 4) / num;
			normals[i].z = z;
			normals[i + 1].z = z;
			normals[i + 2].z = z;
			normals[i + 3].z = z;
		}
	}
}
