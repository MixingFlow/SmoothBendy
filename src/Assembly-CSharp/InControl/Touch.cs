using UnityEngine;

namespace InControl;

public class Touch
{
	public static readonly int FingerID_None = -1;

	public static readonly int FingerID_Mouse = -2;

	public int fingerId;

	public TouchPhase phase;

	public int tapCount;

	public Vector2 position;

	public Vector2 deltaPosition;

	public Vector2 lastPosition;

	public float deltaTime;

	public ulong updateTick;

	public TouchType type;

	public float altitudeAngle;

	public float azimuthAngle;

	public float maximumPossiblePressure;

	public float pressure;

	public float radius;

	public float radiusVariance;

	public float normalizedPressure => Mathf.Clamp(pressure / maximumPossiblePressure, 0.001f, 1f);

	internal Touch()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		fingerId = FingerID_None;
		phase = (TouchPhase)3;
	}

	internal void Reset()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		fingerId = FingerID_None;
		phase = (TouchPhase)3;
		tapCount = 0;
		position = Vector2.zero;
		deltaPosition = Vector2.zero;
		lastPosition = Vector2.zero;
		deltaTime = 0f;
		updateTick = 0uL;
		type = TouchType.Direct;
		altitudeAngle = 0f;
		azimuthAngle = 0f;
		maximumPossiblePressure = 1f;
		pressure = 0f;
		radius = 0f;
		radiusVariance = 0f;
	}

	internal void SetWithTouchData(Touch touch, ulong updateTick, float deltaTime)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Invalid comparison between Unknown and I4
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		phase = ((Touch)(ref touch)).phase;
		tapCount = ((Touch)(ref touch)).tapCount;
		altitudeAngle = ((Touch)(ref touch)).altitudeAngle;
		azimuthAngle = ((Touch)(ref touch)).azimuthAngle;
		maximumPossiblePressure = ((Touch)(ref touch)).maximumPossiblePressure;
		pressure = ((Touch)(ref touch)).pressure;
		radius = ((Touch)(ref touch)).radius;
		radiusVariance = ((Touch)(ref touch)).radiusVariance;
		Vector2 val = ((Touch)(ref touch)).position;
		if (val.x < 0f)
		{
			val.x = (float)Screen.width + val.x;
		}
		if ((int)phase == 0)
		{
			deltaPosition = Vector2.zero;
			lastPosition = val;
			position = val;
		}
		else
		{
			if ((int)phase == 2)
			{
				phase = (TouchPhase)1;
			}
			deltaPosition = val - lastPosition;
			lastPosition = position;
			position = val;
		}
		this.deltaTime = deltaTime;
		this.updateTick = updateTick;
	}

	internal bool SetWithMouseData(ulong updateTick, float deltaTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (Input.touchCount > 0)
		{
			return false;
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(Mathf.Round(Input.mousePosition.x), Mathf.Round(Input.mousePosition.y));
		if (Input.GetMouseButtonDown(0))
		{
			phase = (TouchPhase)0;
			pressure = 1f;
			maximumPossiblePressure = 1f;
			tapCount = 1;
			type = TouchType.Mouse;
			deltaPosition = Vector2.zero;
			lastPosition = val;
			position = val;
			this.deltaTime = deltaTime;
			this.updateTick = updateTick;
			return true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			phase = (TouchPhase)3;
			pressure = 0f;
			maximumPossiblePressure = 1f;
			tapCount = 1;
			type = TouchType.Mouse;
			deltaPosition = val - lastPosition;
			lastPosition = position;
			position = val;
			this.deltaTime = deltaTime;
			this.updateTick = updateTick;
			return true;
		}
		if (Input.GetMouseButton(0))
		{
			phase = (TouchPhase)1;
			pressure = 1f;
			maximumPossiblePressure = 1f;
			tapCount = 1;
			type = TouchType.Mouse;
			deltaPosition = val - lastPosition;
			lastPosition = position;
			position = val;
			this.deltaTime = deltaTime;
			this.updateTick = updateTick;
			return true;
		}
		return false;
	}
}
