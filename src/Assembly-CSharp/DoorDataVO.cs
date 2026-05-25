using System;
using UnityEngine;

[Serializable]
public class DoorDataVO : GameObjectDataVO
{
	public bool isClosed;

	public bool isLocked;

	public void UpdateData(bool _isClosed, bool _isLocked, Vector3 rotation)
	{
		isClosed = _isClosed;
		isLocked = _isLocked;
		RotationX = rotation.x;
		RotationY = rotation.y;
		RotationZ = rotation.z;
	}
}
