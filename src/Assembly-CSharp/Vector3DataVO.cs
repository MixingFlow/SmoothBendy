using System;
using UnityEngine;

[Serializable]
public class Vector3DataVO
{
	public float X;

	public float Y;

	public float Z;

	public Vector3DataVO(Vector3 vector)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SetVector(vector);
	}

	public void SetVector(Vector3 vector)
	{
		X = vector.x;
		Y = vector.y;
		Z = vector.z;
	}

	public Vector3 GetVector()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(X, Y, Z);
	}
}
