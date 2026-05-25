using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro;

public struct TMP_MeshInfo
{
	private static readonly Color32 s_DefaultColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private static readonly Vector3 s_DefaultNormal = new Vector3(0f, 0f, -1f);

	private static readonly Vector4 s_DefaultTangent = new Vector4(-1f, 0f, 0f, 1f);

	public Mesh mesh;

	public int vertexCount;

	public Vector3[] vertices;

	public Vector3[] normals;

	public Vector4[] tangents;

	public Vector2[] uvs0;

	public Vector2[] uvs2;

	public Color32[] colors32;

	public int[] triangles;

	public TMP_MeshInfo(Mesh mesh, int size)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mesh == (Object)null)
		{
			mesh = new Mesh();
		}
		else
		{
			mesh.Clear();
		}
		this.mesh = mesh;
		size = Mathf.Min(size, 16383);
		int num = size * 4;
		int num2 = size * 6;
		vertexCount = 0;
		vertices = (Vector3[])(object)new Vector3[num];
		uvs0 = (Vector2[])(object)new Vector2[num];
		uvs2 = (Vector2[])(object)new Vector2[num];
		colors32 = (Color32[])(object)new Color32[num];
		normals = (Vector3[])(object)new Vector3[num];
		tangents = (Vector4[])(object)new Vector4[num];
		triangles = new int[num2];
		int num3 = 0;
		int num4 = 0;
		while (num4 / 4 < size)
		{
			for (int i = 0; i < 4; i++)
			{
				ref Vector3 reference = ref vertices[num4 + i];
				reference = Vector3.zero;
				ref Vector2 reference2 = ref uvs0[num4 + i];
				reference2 = Vector2.zero;
				ref Vector2 reference3 = ref uvs2[num4 + i];
				reference3 = Vector2.zero;
				ref Color32 reference4 = ref colors32[num4 + i];
				reference4 = s_DefaultColor;
				ref Vector3 reference5 = ref normals[num4 + i];
				reference5 = s_DefaultNormal;
				ref Vector4 reference6 = ref tangents[num4 + i];
				reference6 = s_DefaultTangent;
			}
			triangles[num3] = num4;
			triangles[num3 + 1] = num4 + 1;
			triangles[num3 + 2] = num4 + 2;
			triangles[num3 + 3] = num4 + 2;
			triangles[num3 + 4] = num4 + 3;
			triangles[num3 + 5] = num4;
			num4 += 4;
			num3 += 6;
		}
		this.mesh.vertices = vertices;
		this.mesh.normals = normals;
		this.mesh.tangents = tangents;
		this.mesh.triangles = triangles;
		this.mesh.bounds = new Bounds(Vector3.zero, new Vector3(3840f, 2160f, 0f));
	}

	public TMP_MeshInfo(Mesh mesh, int size, bool isVolumetric)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mesh == (Object)null)
		{
			mesh = new Mesh();
		}
		else
		{
			mesh.Clear();
		}
		this.mesh = mesh;
		int num = (isVolumetric ? 8 : 4);
		int num2 = (isVolumetric ? 36 : 6);
		size = Mathf.Min(size, 65532 / num);
		int num3 = size * num;
		int num4 = size * num2;
		vertexCount = 0;
		vertices = (Vector3[])(object)new Vector3[num3];
		uvs0 = (Vector2[])(object)new Vector2[num3];
		uvs2 = (Vector2[])(object)new Vector2[num3];
		colors32 = (Color32[])(object)new Color32[num3];
		normals = (Vector3[])(object)new Vector3[num3];
		tangents = (Vector4[])(object)new Vector4[num3];
		triangles = new int[num4];
		int num5 = 0;
		int num6 = 0;
		while (num5 / num < size)
		{
			for (int i = 0; i < num; i++)
			{
				ref Vector3 reference = ref vertices[num5 + i];
				reference = Vector3.zero;
				ref Vector2 reference2 = ref uvs0[num5 + i];
				reference2 = Vector2.zero;
				ref Vector2 reference3 = ref uvs2[num5 + i];
				reference3 = Vector2.zero;
				ref Color32 reference4 = ref colors32[num5 + i];
				reference4 = s_DefaultColor;
				ref Vector3 reference5 = ref normals[num5 + i];
				reference5 = s_DefaultNormal;
				ref Vector4 reference6 = ref tangents[num5 + i];
				reference6 = s_DefaultTangent;
			}
			triangles[num6] = num5;
			triangles[num6 + 1] = num5 + 1;
			triangles[num6 + 2] = num5 + 2;
			triangles[num6 + 3] = num5 + 2;
			triangles[num6 + 4] = num5 + 3;
			triangles[num6 + 5] = num5;
			if (isVolumetric)
			{
				triangles[num6 + 6] = num5 + 4;
				triangles[num6 + 7] = num5 + 5;
				triangles[num6 + 8] = num5 + 1;
				triangles[num6 + 9] = num5 + 1;
				triangles[num6 + 10] = num5;
				triangles[num6 + 11] = num5 + 4;
				triangles[num6 + 12] = num5 + 3;
				triangles[num6 + 13] = num5 + 2;
				triangles[num6 + 14] = num5 + 6;
				triangles[num6 + 15] = num5 + 6;
				triangles[num6 + 16] = num5 + 7;
				triangles[num6 + 17] = num5 + 3;
				triangles[num6 + 18] = num5 + 1;
				triangles[num6 + 19] = num5 + 5;
				triangles[num6 + 20] = num5 + 6;
				triangles[num6 + 21] = num5 + 6;
				triangles[num6 + 22] = num5 + 2;
				triangles[num6 + 23] = num5 + 1;
				triangles[num6 + 24] = num5 + 4;
				triangles[num6 + 25] = num5;
				triangles[num6 + 26] = num5 + 3;
				triangles[num6 + 27] = num5 + 3;
				triangles[num6 + 28] = num5 + 7;
				triangles[num6 + 29] = num5 + 4;
				triangles[num6 + 30] = num5 + 7;
				triangles[num6 + 31] = num5 + 6;
				triangles[num6 + 32] = num5 + 5;
				triangles[num6 + 33] = num5 + 5;
				triangles[num6 + 34] = num5 + 4;
				triangles[num6 + 35] = num5 + 7;
			}
			num5 += num;
			num6 += num2;
		}
		this.mesh.vertices = vertices;
		this.mesh.normals = normals;
		this.mesh.tangents = tangents;
		this.mesh.triangles = triangles;
		this.mesh.bounds = new Bounds(Vector3.zero, new Vector3(3840f, 2160f, 64f));
	}

	public void ResizeMeshInfo(int size)
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		size = Mathf.Min(size, 16383);
		int newSize = size * 4;
		int newSize2 = size * 6;
		int num = vertices.Length / 4;
		Array.Resize(ref vertices, newSize);
		Array.Resize(ref normals, newSize);
		Array.Resize(ref tangents, newSize);
		Array.Resize(ref uvs0, newSize);
		Array.Resize(ref uvs2, newSize);
		Array.Resize(ref colors32, newSize);
		Array.Resize(ref triangles, newSize2);
		if (size <= num)
		{
			mesh.triangles = triangles;
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.tangents = tangents;
			return;
		}
		for (int i = num; i < size; i++)
		{
			int num2 = i * 4;
			int num3 = i * 6;
			ref Vector3 reference = ref normals[num2];
			reference = s_DefaultNormal;
			ref Vector3 reference2 = ref normals[1 + num2];
			reference2 = s_DefaultNormal;
			ref Vector3 reference3 = ref normals[2 + num2];
			reference3 = s_DefaultNormal;
			ref Vector3 reference4 = ref normals[3 + num2];
			reference4 = s_DefaultNormal;
			ref Vector4 reference5 = ref tangents[num2];
			reference5 = s_DefaultTangent;
			ref Vector4 reference6 = ref tangents[1 + num2];
			reference6 = s_DefaultTangent;
			ref Vector4 reference7 = ref tangents[2 + num2];
			reference7 = s_DefaultTangent;
			ref Vector4 reference8 = ref tangents[3 + num2];
			reference8 = s_DefaultTangent;
			triangles[num3] = num2;
			triangles[1 + num3] = 1 + num2;
			triangles[2 + num3] = 2 + num2;
			triangles[3 + num3] = 2 + num2;
			triangles[4 + num3] = 3 + num2;
			triangles[5 + num3] = num2;
		}
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.tangents = tangents;
		mesh.triangles = triangles;
	}

	public void ResizeMeshInfo(int size, bool isVolumetric)
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		int num = (isVolumetric ? 8 : 4);
		int num2 = (isVolumetric ? 36 : 6);
		size = Mathf.Min(size, 65532 / num);
		int newSize = size * num;
		int newSize2 = size * num2;
		int num3 = vertices.Length / num;
		Array.Resize(ref vertices, newSize);
		Array.Resize(ref normals, newSize);
		Array.Resize(ref tangents, newSize);
		Array.Resize(ref uvs0, newSize);
		Array.Resize(ref uvs2, newSize);
		Array.Resize(ref colors32, newSize);
		Array.Resize(ref triangles, newSize2);
		if (size <= num3)
		{
			mesh.triangles = triangles;
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.tangents = tangents;
			return;
		}
		for (int i = num3; i < size; i++)
		{
			int num4 = i * num;
			int num5 = i * num2;
			ref Vector3 reference = ref normals[num4];
			reference = s_DefaultNormal;
			ref Vector3 reference2 = ref normals[1 + num4];
			reference2 = s_DefaultNormal;
			ref Vector3 reference3 = ref normals[2 + num4];
			reference3 = s_DefaultNormal;
			ref Vector3 reference4 = ref normals[3 + num4];
			reference4 = s_DefaultNormal;
			ref Vector4 reference5 = ref tangents[num4];
			reference5 = s_DefaultTangent;
			ref Vector4 reference6 = ref tangents[1 + num4];
			reference6 = s_DefaultTangent;
			ref Vector4 reference7 = ref tangents[2 + num4];
			reference7 = s_DefaultTangent;
			ref Vector4 reference8 = ref tangents[3 + num4];
			reference8 = s_DefaultTangent;
			if (isVolumetric)
			{
				ref Vector3 reference9 = ref normals[4 + num4];
				reference9 = s_DefaultNormal;
				ref Vector3 reference10 = ref normals[5 + num4];
				reference10 = s_DefaultNormal;
				ref Vector3 reference11 = ref normals[6 + num4];
				reference11 = s_DefaultNormal;
				ref Vector3 reference12 = ref normals[7 + num4];
				reference12 = s_DefaultNormal;
				ref Vector4 reference13 = ref tangents[4 + num4];
				reference13 = s_DefaultTangent;
				ref Vector4 reference14 = ref tangents[5 + num4];
				reference14 = s_DefaultTangent;
				ref Vector4 reference15 = ref tangents[6 + num4];
				reference15 = s_DefaultTangent;
				ref Vector4 reference16 = ref tangents[7 + num4];
				reference16 = s_DefaultTangent;
			}
			triangles[num5] = num4;
			triangles[1 + num5] = 1 + num4;
			triangles[2 + num5] = 2 + num4;
			triangles[3 + num5] = 2 + num4;
			triangles[4 + num5] = 3 + num4;
			triangles[5 + num5] = num4;
			if (isVolumetric)
			{
				triangles[num5 + 6] = num4 + 4;
				triangles[num5 + 7] = num4 + 5;
				triangles[num5 + 8] = num4 + 1;
				triangles[num5 + 9] = num4 + 1;
				triangles[num5 + 10] = num4;
				triangles[num5 + 11] = num4 + 4;
				triangles[num5 + 12] = num4 + 3;
				triangles[num5 + 13] = num4 + 2;
				triangles[num5 + 14] = num4 + 6;
				triangles[num5 + 15] = num4 + 6;
				triangles[num5 + 16] = num4 + 7;
				triangles[num5 + 17] = num4 + 3;
				triangles[num5 + 18] = num4 + 1;
				triangles[num5 + 19] = num4 + 5;
				triangles[num5 + 20] = num4 + 6;
				triangles[num5 + 21] = num4 + 6;
				triangles[num5 + 22] = num4 + 2;
				triangles[num5 + 23] = num4 + 1;
				triangles[num5 + 24] = num4 + 4;
				triangles[num5 + 25] = num4;
				triangles[num5 + 26] = num4 + 3;
				triangles[num5 + 27] = num4 + 3;
				triangles[num5 + 28] = num4 + 7;
				triangles[num5 + 29] = num4 + 4;
				triangles[num5 + 30] = num4 + 7;
				triangles[num5 + 31] = num4 + 6;
				triangles[num5 + 32] = num4 + 5;
				triangles[num5 + 33] = num4 + 5;
				triangles[num5 + 34] = num4 + 4;
				triangles[num5 + 35] = num4 + 7;
			}
		}
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.tangents = tangents;
		mesh.triangles = triangles;
	}

	public void Clear()
	{
		if (vertices != null)
		{
			Array.Clear(vertices, 0, vertices.Length);
			vertexCount = 0;
			if ((Object)(object)mesh != (Object)null)
			{
				mesh.vertices = vertices;
			}
		}
	}

	public void Clear(bool uploadChanges)
	{
		if (vertices != null)
		{
			Array.Clear(vertices, 0, vertices.Length);
			vertexCount = 0;
			if (uploadChanges && (Object)(object)mesh != (Object)null)
			{
				mesh.vertices = vertices;
			}
		}
	}

	public void ClearUnusedVertices()
	{
		int num = vertices.Length - vertexCount;
		if (num > 0)
		{
			Array.Clear(vertices, vertexCount, num);
		}
	}

	public void ClearUnusedVertices(int startIndex)
	{
		int num = vertices.Length - startIndex;
		if (num > 0)
		{
			Array.Clear(vertices, startIndex, num);
		}
	}

	public void ClearUnusedVertices(int startIndex, bool updateMesh)
	{
		int num = vertices.Length - startIndex;
		if (num > 0)
		{
			Array.Clear(vertices, startIndex, num);
		}
		if (updateMesh && (Object)(object)mesh != (Object)null)
		{
			mesh.vertices = vertices;
		}
	}

	public void SortGeometry(VertexSortingOrder order)
	{
		switch (order)
		{
		}
	}

	public void SortGeometry(IList<int> sortingOrder)
	{
		int count = sortingOrder.Count;
		if (count * 4 > vertices.Length)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			int num;
			for (num = sortingOrder[i]; num < i; num = sortingOrder[num])
			{
			}
			if (num != i)
			{
				SwapVertexData(num * 4, i * 4);
			}
		}
	}

	public void SwapVertexData(int src, int dst)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = vertices[dst];
		ref Vector3 reference = ref vertices[dst];
		reference = vertices[src];
		vertices[src] = val;
		val = vertices[dst + 1];
		ref Vector3 reference2 = ref vertices[dst + 1];
		reference2 = vertices[src + 1];
		vertices[src + 1] = val;
		val = vertices[dst + 2];
		ref Vector3 reference3 = ref vertices[dst + 2];
		reference3 = vertices[src + 2];
		vertices[src + 2] = val;
		val = vertices[dst + 3];
		ref Vector3 reference4 = ref vertices[dst + 3];
		reference4 = vertices[src + 3];
		vertices[src + 3] = val;
		Vector2 val2 = uvs0[dst];
		ref Vector2 reference5 = ref uvs0[dst];
		reference5 = uvs0[src];
		uvs0[src] = val2;
		val2 = uvs0[dst + 1];
		ref Vector2 reference6 = ref uvs0[dst + 1];
		reference6 = uvs0[src + 1];
		uvs0[src + 1] = val2;
		val2 = uvs0[dst + 2];
		ref Vector2 reference7 = ref uvs0[dst + 2];
		reference7 = uvs0[src + 2];
		uvs0[src + 2] = val2;
		val2 = uvs0[dst + 3];
		ref Vector2 reference8 = ref uvs0[dst + 3];
		reference8 = uvs0[src + 3];
		uvs0[src + 3] = val2;
		val2 = uvs2[dst];
		ref Vector2 reference9 = ref uvs2[dst];
		reference9 = uvs2[src];
		uvs2[src] = val2;
		val2 = uvs2[dst + 1];
		ref Vector2 reference10 = ref uvs2[dst + 1];
		reference10 = uvs2[src + 1];
		uvs2[src + 1] = val2;
		val2 = uvs2[dst + 2];
		ref Vector2 reference11 = ref uvs2[dst + 2];
		reference11 = uvs2[src + 2];
		uvs2[src + 2] = val2;
		val2 = uvs2[dst + 3];
		ref Vector2 reference12 = ref uvs2[dst + 3];
		reference12 = uvs2[src + 3];
		uvs2[src + 3] = val2;
		Color32 val3 = colors32[dst];
		ref Color32 reference13 = ref colors32[dst];
		reference13 = colors32[src];
		colors32[src] = val3;
		val3 = colors32[dst + 1];
		ref Color32 reference14 = ref colors32[dst + 1];
		reference14 = colors32[src + 1];
		colors32[src + 1] = val3;
		val3 = colors32[dst + 2];
		ref Color32 reference15 = ref colors32[dst + 2];
		reference15 = colors32[src + 2];
		colors32[src + 2] = val3;
		val3 = colors32[dst + 3];
		ref Color32 reference16 = ref colors32[dst + 3];
		reference16 = colors32[src + 3];
		colors32[src + 3] = val3;
	}
}
