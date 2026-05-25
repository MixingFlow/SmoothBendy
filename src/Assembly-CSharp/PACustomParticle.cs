using System;
using UnityEngine;

[Serializable]
public class PACustomParticle
{
	public Vector3 originDirection;

	public float size = 1f;

	public Color color = Color.white;

	public float speed;

	public float spinSpeed;

	public Rect uv = new Rect(0f, 0f, 1f, 1f);

	public PACustomParticle()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		originDirection = new Vector3(0.5f, 0.5f, 0.5f);
		size = 1f;
		color = Color.white;
		speed = 0f;
		spinSpeed = 0f;
		uv = new Rect(0f, 0f, 1f, 1f);
	}

	public PACustomParticle(Vector3 originDirection, Color color, float size, float speed, float spinSpeed, Rect uv)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		this.originDirection = originDirection;
		this.size = size;
		this.speed = speed;
		this.spinSpeed = spinSpeed;
		this.color = color;
		this.uv = uv;
	}

	public PACustomParticle(Vector3 originDirection, Color color, float size = 1f, float speed = 0f, float spinSpeed = 0f)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		this.originDirection = originDirection;
		this.size = size;
		this.speed = speed;
		this.spinSpeed = spinSpeed;
		this.color = color;
		uv = new Rect(0f, 0f, 1f, 1f);
	}

	public PACustomParticle(Vector3 originDirection)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		this.originDirection = originDirection;
		size = 1f;
		speed = 0f;
		spinSpeed = 0f;
		color = Color.white;
		uv = new Rect(0f, 0f, 1f, 1f);
	}

	public void SetDefaultValuesIfUninitialized()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (originDirection == default(Vector3) && size == 0f && color == default(Color) && speed == 0f && spinSpeed == 0f && uv == default(Rect))
		{
			originDirection = Vector3.zero;
			size = 1f;
			color = Color.white;
			speed = 0f;
			spinSpeed = 0f;
			uv = new Rect(0f, 0f, 1f, 1f);
		}
	}
}
