using UnityEngine;

public class PAParticleMeshGenerator : MonoBehaviour, ISerializationCallbackReceiver
{
	public static class RandomWrapper
	{
		private static int m_Seed;

		public static State cachedState;

		public static int seed => m_Seed;

		public static void CacheState()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			cachedState = Random.state;
		}

		public static void RestoreState()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Random.state = cachedState;
		}

		public static void SetState(int seed)
		{
			Random.InitState(seed);
			m_Seed = seed;
			Random.Range(0f, 0f);
		}
	}

	private const int DATA_STRUCTURE_VERSION = 2;

	[HideInInspector]
	[SerializeField]
	protected Vector3[] verts;

	[HideInInspector]
	[SerializeField]
	protected Vector3[] normals;

	[HideInInspector]
	[SerializeField]
	protected Vector4[] tangents;

	[HideInInspector]
	[SerializeField]
	protected Vector2[] uv0;

	[HideInInspector]
	[SerializeField]
	protected Vector2[] uv1;

	[HideInInspector]
	[SerializeField]
	protected Color[] colors;

	[HideInInspector]
	[SerializeField]
	protected int[] triangles;

	[SerializeField]
	[HideInInspector]
	private int dataStructureVersion;

	public void OnBeforeSerialize()
	{
		PAParticleField component = ((Component)this).GetComponent<PAParticleField>();
		if (Object.op_Implicit((Object)(object)component) && component.clearCacheInBuilds)
		{
			ClearCache();
		}
	}

	public void OnAfterDeserialize()
	{
	}

	protected void CacheSeed()
	{
		RandomWrapper.CacheState();
	}

	private void SetSeed(int seed)
	{
		RandomWrapper.SetState(seed);
	}

	protected float GetRandomAndIncrement(float min, float max)
	{
		float result = Random.Range(min, max);
		RandomWrapper.SetState(RandomWrapper.seed + 1);
		return result;
	}

	protected void ResetSeed()
	{
		RandomWrapper.RestoreState();
	}

	public virtual int GetMaximumParticleCount()
	{
		return 16250;
	}

	public virtual float GetParticleBaseSize()
	{
		return 1f;
	}

	protected void SkipRandomCalls(int callsPerParticle, int count)
	{
		for (int i = 0; i < callsPerParticle * count; i++)
		{
			GetRandomAndIncrement(0f, 0f);
		}
	}

	public virtual void UpdateMesh(Mesh mesh, PAParticleField settings)
	{
		UpdateCache(settings);
		FillMesh(mesh);
	}

	protected virtual void UpdateCache(PAParticleField settings)
	{
		CacheSeed();
		int clampedParticleCount = GetClampedParticleCount(settings.particleCount);
		int num = -1;
		if (verts == null || verts.Length == 0 || dataStructureVersion != 2)
		{
			verts = (Vector3[])(object)new Vector3[0];
			normals = (Vector3[])(object)new Vector3[0];
			tangents = (Vector4[])(object)new Vector4[0];
			uv0 = (Vector2[])(object)new Vector2[0];
			uv1 = (Vector2[])(object)new Vector2[0];
			colors = (Color[])(object)new Color[0];
			triangles = new int[0];
			num = 0;
		}
		if ((settings.meshIsDirtyMask & MeshFlags.Count) != MeshFlags.None || num == 0)
		{
			num = SetParticleCapacity(clampedParticleCount);
			if (num == clampedParticleCount)
			{
				return;
			}
			UpdateIndicies();
			UpdateTriangles(num);
		}
		if ((settings.meshIsDirtyMask & MeshFlags.Seed) != MeshFlags.None || num != -1)
		{
			SetSeed(settings.seed);
			UpdateDirection(settings, Mathf.Max(0, num));
		}
		if ((settings.meshIsDirtyMask & MeshFlags.Surface) != MeshFlags.None || num != -1)
		{
			SetSeed(settings.seed);
			UpdateSurface(settings, Mathf.Max(0, num));
		}
		if ((settings.meshIsDirtyMask & MeshFlags.Speed) != MeshFlags.None || num != -1)
		{
			SetSeed(settings.seed);
			UpdateSpeed(settings, Mathf.Max(0, num));
		}
		if ((settings.meshIsDirtyMask & MeshFlags.Color) != MeshFlags.None || num != -1)
		{
			SetSeed(settings.seed);
			UpdateColor(settings, Mathf.Max(0, num));
		}
		ResetSeed();
		dataStructureVersion = 2;
	}

	protected void FillMesh(Mesh mesh)
	{
		mesh.Clear();
		mesh.vertices = verts;
		mesh.normals = normals;
		mesh.tangents = tangents;
		mesh.uv = uv0;
		mesh.uv2 = uv1;
		mesh.colors = colors;
		mesh.triangles = triangles;
	}

	public int GetClampedParticleCount(int count)
	{
		int maximumParticleCount = GetMaximumParticleCount();
		if (count > maximumParticleCount)
		{
			return maximumParticleCount;
		}
		return count;
	}

	protected virtual int SetParticleCapacity(int count)
	{
		return -1;
	}

	protected void SetArraySizes(int vertCount, int triCount)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[vertCount];
		Vector3[] array2 = (Vector3[])(object)new Vector3[vertCount];
		Vector4[] array3 = (Vector4[])(object)new Vector4[vertCount];
		Vector2[] array4 = (Vector2[])(object)new Vector2[vertCount];
		Vector2[] array5 = (Vector2[])(object)new Vector2[vertCount];
		Color[] array6 = (Color[])(object)new Color[vertCount];
		int[] array7 = new int[triCount];
		int num = Mathf.Min(verts.Length, vertCount);
		for (int i = 0; i < num; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = verts[i];
			ref Vector3 reference2 = ref array2[i];
			reference2 = normals[i];
			ref Vector4 reference3 = ref array3[i];
			reference3 = tangents[i];
			ref Vector2 reference4 = ref array4[i];
			reference4 = uv0[i];
			ref Vector2 reference5 = ref array5[i];
			reference5 = uv1[i];
			ref Color reference6 = ref array6[i];
			reference6 = colors[i];
		}
		num = Mathf.Min(triangles.Length, triCount);
		for (int j = 0; j < num; j++)
		{
			array7[j] = triangles[j];
		}
		verts = array;
		normals = array2;
		tangents = array3;
		uv0 = array4;
		uv1 = array5;
		colors = array6;
		triangles = array7;
	}

	protected virtual void UpdateDirection(PAParticleField settings, int startAt)
	{
	}

	protected virtual void UpdateSurface(PAParticleField settings, int startAt)
	{
	}

	protected virtual void UpdateSpeed(PAParticleField settings, int startAt)
	{
	}

	protected virtual void UpdateColor(PAParticleField settings, int startAt)
	{
	}

	protected virtual void UpdateIndicies()
	{
	}

	protected virtual void UpdateTriangles(int startAt)
	{
	}

	public void ClearCache()
	{
		verts = (Vector3[])(object)new Vector3[0];
		normals = (Vector3[])(object)new Vector3[0];
		tangents = (Vector4[])(object)new Vector4[0];
		uv0 = (Vector2[])(object)new Vector2[0];
		uv1 = (Vector2[])(object)new Vector2[0];
		colors = (Color[])(object)new Color[0];
		triangles = new int[0];
	}

	public void CheckDataStructureVersion()
	{
		if (dataStructureVersion != 2)
		{
			UpdateCache(((Component)this).GetComponent<PAParticleField>());
		}
	}
}
