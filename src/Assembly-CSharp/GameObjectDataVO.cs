using System;
using UnityEngine;

[Serializable]
public class GameObjectDataVO
{
	public bool isEnabled;

	public float PositionX;

	public float PositionY;

	public float PositionZ;

	public float RotationX;

	public float RotationY;

	public float RotationZ;

	public float ScaleX;

	public float ScaleY;

	public float ScaleZ;

	public Vector3 Position => new Vector3(PositionX, PositionY, PositionZ);

	public Vector3 Rotation => new Vector3(RotationX, RotationY, RotationZ);

	public Vector3 Scale => new Vector3(ScaleX, ScaleY, ScaleZ);

	public void UpdateData(bool _isEnabled, Transform transform)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		isEnabled = _isEnabled;
		PositionX = transform.position.x;
		PositionY = transform.position.y;
		PositionZ = transform.position.z;
		RotationX = transform.eulerAngles.x;
		RotationY = transform.eulerAngles.y;
		RotationZ = transform.eulerAngles.z;
		ScaleX = transform.localScale.x;
		ScaleY = transform.localScale.y;
		ScaleZ = transform.localScale.z;
	}
}
