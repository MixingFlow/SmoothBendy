using UnityEngine;

public class PAMeshParticle : PAParticleMeshGenerator
{
	private const int MAX_VERT_COUNT = 65536;

	[HideInInspector]
	public Mesh inputMesh;

	public override int GetMaximumParticleCount()
	{
		if (Object.op_Implicit((Object)(object)inputMesh))
		{
			return (int)(65536f / (float)inputMesh.vertexCount);
		}
		return 16250;
	}

	public override float GetParticleBaseSize()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)inputMesh))
		{
			Bounds bounds = inputMesh.bounds;
			float x = ((Bounds)(ref bounds)).size.x;
			Bounds bounds2 = inputMesh.bounds;
			float y = ((Bounds)(ref bounds2)).size.y;
			Bounds bounds3 = inputMesh.bounds;
			return Mathf.Max(x, Mathf.Max(y, ((Bounds)(ref bounds3)).size.z));
		}
		return base.GetParticleBaseSize();
	}

	public override void UpdateMesh(Mesh mesh, PAParticleField settings)
	{
		inputMesh = settings.inputMesh;
		if (Object.op_Implicit((Object)(object)inputMesh))
		{
			base.UpdateMesh(mesh, settings);
		}
	}

	protected override int SetParticleCapacity(int count)
	{
		count = GetClampedParticleCount(count);
		int result = count;
		if (count * inputMesh.vertexCount > verts.Length)
		{
			result = verts.Length / inputMesh.vertexCount;
		}
		int vertCount = count * inputMesh.vertexCount;
		int triCount = count * inputMesh.triangles.Length;
		SetArraySizes(vertCount, triCount);
		return result;
	}

	protected override void UpdateDirection(PAParticleField settings, int startAt)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		int clampedParticleCount = GetClampedParticleCount(settings.particleCount);
		SkipRandomCalls(6, startAt);
		for (int i = startAt; i < clampedParticleCount; i++)
		{
			float y = Vector3ToFloat(new Vector3(GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f, 1f)));
			for (int j = 0; j < inputMesh.vertexCount; j++)
			{
				int num = i * inputMesh.vertexCount + j;
				if (num >= normals.Length)
				{
					Debug.Log((object)"An error has occurred in PAMeshParticle", (Object)(object)((Component)this).gameObject);
				}
				uv1[num].y = y;
			}
		}
	}

	protected override void UpdateColor(PAParticleField settings, int startAt)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		int clampedParticleCount = GetClampedParticleCount(settings.particleCount);
		SkipRandomCalls(1, startAt);
		for (int i = startAt; i < clampedParticleCount; i++)
		{
			Color val = settings.colorVariation.Evaluate(GetRandomAndIncrement(0f, 1f));
			for (int j = 0; j < inputMesh.vertexCount; j++)
			{
				int num = i * inputMesh.vertexCount + j;
				if (inputMesh.colors.Length > 0)
				{
					ref Color reference = ref colors[num];
					reference = inputMesh.colors[j] * val;
				}
				else
				{
					colors[num] = val;
				}
			}
		}
	}

	protected override void UpdateSurface(PAParticleField settings, int startAt = 0)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		int clampedParticleCount = GetClampedParticleCount(settings.particleCount);
		float num = ((settings.textureType == PAParticleField.TextureType.Simple) ? 1f : ((float)settings.spriteColumns));
		float num2 = ((settings.textureType == PAParticleField.TextureType.Simple) ? 1f : ((float)settings.spriteRows));
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(1f / num, 1f / num2);
		SkipRandomCalls(3, startAt);
		Vector2 val2 = default(Vector2);
		for (int i = startAt; i < clampedParticleCount; i++)
		{
			float randomAndIncrement = GetRandomAndIncrement(settings.minimumSize, 1f);
			((Vector2)(ref val2))._002Ector((float)(int)GetRandomAndIncrement(0f, num), (float)(int)GetRandomAndIncrement(0f, num2));
			for (int j = 0; j < inputMesh.vertexCount; j++)
			{
				int num3 = i * inputMesh.vertexCount + j;
				ref Vector3 reference = ref verts[num3];
				reference = inputMesh.vertices[j] * randomAndIncrement;
				if (inputMesh.normals.Length > 0)
				{
					uv1[num3].x = Vector3ToFloat(((Vector3)(ref inputMesh.normals[j])).normalized * 0.9f);
				}
				if (inputMesh.tangents.Length > 0)
				{
					ref Vector4 reference2 = ref tangents[num3];
					reference2 = inputMesh.tangents[j];
				}
				if (inputMesh.uv.Length > 0)
				{
					ref Vector2 reference3 = ref uv0[num3];
					reference3 = Vector2.Scale(inputMesh.uv[j] + val2, val);
				}
			}
		}
	}

	protected override void UpdateTriangles(int startAt)
	{
		int num = triangles.Length / inputMesh.triangles.Length;
		for (int i = startAt; i < num; i++)
		{
			for (int j = 0; j < inputMesh.triangles.Length; j++)
			{
				int num2 = i * inputMesh.triangles.Length + j;
				triangles[num2] = inputMesh.triangles[j] + inputMesh.vertexCount * i;
			}
		}
	}

	protected override void UpdateSpeed(PAParticleField settings, int startAt)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		int clampedParticleCount = GetClampedParticleCount(settings.particleCount);
		SkipRandomCalls(2, startAt);
		Vector3 val2 = default(Vector3);
		for (int i = startAt; i < clampedParticleCount; i++)
		{
			Vector2 val = Vector2.op_Implicit(new Vector3(GetRandomAndIncrement(settings.minimumSpeed, 1f), GetRandomAndIncrement(settings.minSpinSpeed, 1f)));
			Vector3 c = settings.rotationAxis;
			if (!settings.customRotationAxis)
			{
				((Vector3)(ref val2))._002Ector(GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f, 1f));
				c = ((Vector3)(ref val2)).normalized;
			}
			float z = Vector3ToFloat(c);
			for (int j = 0; j < inputMesh.vertexCount; j++)
			{
				int num = i * inputMesh.vertexCount + j;
				normals[num].x = val.x;
				normals[num].y = val.y;
				normals[num].z = z;
			}
		}
	}

	public static float Vector3ToFloat(Vector3 c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		c = (c + Vector3.one) * 0.5f;
		return Vector3.Dot(new Vector3(Mathf.Round(c.x * 255f), Mathf.Round(c.y * 255f), Mathf.Round(c.z * 255f)), new Vector3(65536f, 256f, 1f));
	}
}
