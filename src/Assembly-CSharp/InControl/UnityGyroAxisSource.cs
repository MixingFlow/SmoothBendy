using UnityEngine;

namespace InControl;

public class UnityGyroAxisSource : InputControlSource
{
	public enum GyroAxis
	{
		X,
		Y
	}

	private static Quaternion zeroAttitude;

	public int Axis;

	public UnityGyroAxisSource()
	{
		Calibrate();
	}

	public UnityGyroAxisSource(GyroAxis axis)
	{
		Axis = (int)axis;
		Calibrate();
	}

	public float GetValue(InputDevice inputDevice)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		Vector3 axis = GetAxis();
		return ((Vector3)(ref axis))[Axis];
	}

	public bool GetState(InputDevice inputDevice)
	{
		return Utility.IsNotZero(GetValue(inputDevice));
	}

	private static Quaternion GetAttitude()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Inverse(zeroAttitude) * Input.gyro.attitude;
	}

	private static Vector3 GetAxis()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GetAttitude() * Vector3.forward;
		float num = ApplyDeadZone(Mathf.Clamp(val.x, -1f, 1f));
		float num2 = ApplyDeadZone(Mathf.Clamp(val.y, -1f, 1f));
		return new Vector3(num, num2);
	}

	private static float ApplyDeadZone(float value)
	{
		return Mathf.InverseLerp(0.05f, 1f, Utility.Abs(value)) * Mathf.Sign(value);
	}

	public static void Calibrate()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		zeroAttitude = Input.gyro.attitude;
	}
}
