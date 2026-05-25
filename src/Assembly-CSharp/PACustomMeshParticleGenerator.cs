using System.Collections.Generic;
using UnityEngine;

public class PACustomMeshParticleGenerator : PAMeshParticle, PAICustomParticleGenerator
{
	public List<PACustomMeshParticle> particles;

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
		particles.ForEach(delegate(PACustomMeshParticle obj)
		{
			obj.SetDefaultValuesIfUninitialized();
		});
	}

	protected override void UpdateCache(PAParticleField settings)
	{
	}

	protected override int SetParticleCapacity(int count)
	{
		return particles.Count;
	}

	[ContextMenu("Apply Particles")]
	public void ApplyParticles()
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		Mesh sharedMesh = ((Component)this).GetComponent<MeshFilter>().sharedMesh;
		if (!((Object)(object)sharedMesh != (Object)null) || particles == null || particles.Count <= 0)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<Vector4> list3 = new List<Vector4>();
		List<Vector2> list4 = new List<Vector2>();
		List<Vector2> list5 = new List<Vector2>();
		List<Color> list6 = new List<Color>();
		List<int> list7 = new List<int>();
		for (int i = 0; i < particles.Count; i++)
		{
			PACustomMeshParticle pACustomMeshParticle = particles[i];
			if ((Object)(object)pACustomMeshParticle.mesh == (Object)null)
			{
				continue;
			}
			for (int j = 0; j < pACustomMeshParticle.mesh.triangles.Length; j++)
			{
				list7.Add(pACustomMeshParticle.mesh.triangles[j] + list.Count);
			}
			for (int k = 0; k < pACustomMeshParticle.mesh.vertexCount; k++)
			{
				list.Add(pACustomMeshParticle.mesh.vertices[k] * pACustomMeshParticle.size);
				if (pACustomMeshParticle.mesh.tangents != null && pACustomMeshParticle.mesh.tangents.Length > 0)
				{
					list3.Add(pACustomMeshParticle.mesh.tangents[k]);
				}
				else
				{
					list3.Add(new Vector4(0f, 1f, 0f, 1f));
				}
				Vector2 item = Vector2.Scale(pACustomMeshParticle.mesh.uv[k], ((Rect)(ref pACustomMeshParticle.uv)).size) + ((Rect)(ref pACustomMeshParticle.uv)).position;
				list4.Add(item);
				Vector2 zero = Vector2.zero;
				zero.x = PAMeshParticle.Vector3ToFloat(pACustomMeshParticle.mesh.normals[k]);
				zero.y = PAMeshParticle.Vector3ToFloat(pACustomMeshParticle.originDirection);
				list5.Add(zero);
				Color val = Color.white;
				if (pACustomMeshParticle.mesh.colors != null && pACustomMeshParticle.mesh.colors.Length > 0)
				{
					val = pACustomMeshParticle.mesh.colors[k];
				}
				list6.Add(val * pACustomMeshParticle.color);
				Vector3 zero2 = Vector3.zero;
				zero2.x = pACustomMeshParticle.speed;
				zero2.y = pACustomMeshParticle.spinSpeed;
				zero2.z = PAMeshParticle.Vector3ToFloat(pACustomMeshParticle.spinAxis);
				list2.Add(zero2);
			}
		}
		verts = list.ToArray();
		normals = list2.ToArray();
		tangents = list3.ToArray();
		uv0 = list4.ToArray();
		uv1 = list5.ToArray();
		colors = list6.ToArray();
		triangles = list7.ToArray();
		FillMesh(sharedMesh);
	}
}
