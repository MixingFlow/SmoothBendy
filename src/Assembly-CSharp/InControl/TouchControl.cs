using UnityEngine;

namespace InControl;

public abstract class TouchControl : MonoBehaviour
{
	public enum ButtonTarget
	{
		None = 0,
		DPadDown = 12,
		DPadLeft = 13,
		DPadRight = 14,
		DPadUp = 11,
		LeftTrigger = 15,
		RightTrigger = 16,
		LeftBumper = 17,
		RightBumper = 18,
		Action1 = 19,
		Action2 = 20,
		Action3 = 21,
		Action4 = 22,
		Action5 = 23,
		Action6 = 24,
		Action7 = 25,
		Action8 = 26,
		Action9 = 27,
		Action10 = 28,
		Action11 = 29,
		Action12 = 30,
		Menu = 106,
		Button0 = 500,
		Button1 = 501,
		Button2 = 502,
		Button3 = 503,
		Button4 = 504,
		Button5 = 505,
		Button6 = 506,
		Button7 = 507,
		Button8 = 508,
		Button9 = 509,
		Button10 = 510,
		Button11 = 511,
		Button12 = 512,
		Button13 = 513,
		Button14 = 514,
		Button15 = 515,
		Button16 = 516,
		Button17 = 517,
		Button18 = 518,
		Button19 = 519
	}

	public enum AnalogTarget
	{
		None,
		LeftStick,
		RightStick,
		Both
	}

	public enum SnapAngles
	{
		None = 0,
		Four = 4,
		Eight = 8,
		Sixteen = 0x10
	}

	public abstract void CreateControl();

	public abstract void DestroyControl();

	public abstract void ConfigureControl();

	public abstract void SubmitControlState(ulong updateTick, float deltaTime);

	public abstract void CommitControlState(ulong updateTick, float deltaTime);

	public abstract void TouchBegan(Touch touch);

	public abstract void TouchMoved(Touch touch);

	public abstract void TouchEnded(Touch touch);

	public abstract void DrawGizmos();

	private void OnEnable()
	{
		TouchManager.OnSetup += Setup;
	}

	private void OnDisable()
	{
		DestroyControl();
		Resources.UnloadUnusedAssets();
	}

	private void Setup()
	{
		if (((Behaviour)this).enabled)
		{
			CreateControl();
			ConfigureControl();
		}
	}

	protected Vector3 OffsetToWorldPosition(TouchControlAnchor anchor, Vector2 offset, TouchUnitType offsetUnitType, bool lockAspectRatio)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((offsetUnitType == TouchUnitType.Pixels) ? Vector2.op_Implicit(TouchUtility.RoundVector(offset) * TouchManager.PixelToWorld) : ((!lockAspectRatio) ? Vector3.Scale(Vector2.op_Implicit(offset), TouchManager.ViewSize) : (Vector2.op_Implicit(offset) * TouchManager.PercentToWorld)));
		return TouchManager.ViewToWorldPoint(TouchUtility.AnchorToViewPoint(anchor)) + val;
	}

	protected void SubmitButtonState(ButtonTarget target, bool state, ulong updateTick, float deltaTime)
	{
		if (TouchManager.Device != null && target != ButtonTarget.None)
		{
			InputControl control = TouchManager.Device.GetControl((InputControlType)target);
			if (control != null && control != InputControl.Null)
			{
				control.UpdateWithState(state, updateTick, deltaTime);
			}
		}
	}

	protected void SubmitButtonValue(ButtonTarget target, float value, ulong updateTick, float deltaTime)
	{
		if (TouchManager.Device != null && target != ButtonTarget.None)
		{
			InputControl control = TouchManager.Device.GetControl((InputControlType)target);
			if (control != null && control != InputControl.Null)
			{
				control.UpdateWithValue(value, updateTick, deltaTime);
			}
		}
	}

	protected void CommitButton(ButtonTarget target)
	{
		if (TouchManager.Device != null && target != ButtonTarget.None)
		{
			InputControl control = TouchManager.Device.GetControl((InputControlType)target);
			if (control != null && control != InputControl.Null)
			{
				control.Commit();
			}
		}
	}

	protected void SubmitAnalogValue(AnalogTarget target, Vector2 value, float lowerDeadZone, float upperDeadZone, ulong updateTick, float deltaTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (TouchManager.Device != null && target != AnalogTarget.None)
		{
			Vector2 value2 = DeadZone.Circular(value.x, value.y, lowerDeadZone, upperDeadZone);
			if (target == AnalogTarget.LeftStick || target == AnalogTarget.Both)
			{
				TouchManager.Device.UpdateLeftStickWithValue(value2, updateTick, deltaTime);
			}
			if (target == AnalogTarget.RightStick || target == AnalogTarget.Both)
			{
				TouchManager.Device.UpdateRightStickWithValue(value2, updateTick, deltaTime);
			}
		}
	}

	protected void CommitAnalog(AnalogTarget target)
	{
		if (TouchManager.Device != null)
		{
			switch (target)
			{
			case AnalogTarget.None:
				return;
			case AnalogTarget.LeftStick:
			case AnalogTarget.Both:
				TouchManager.Device.CommitLeftStick();
				break;
			}
			if (target == AnalogTarget.RightStick || target == AnalogTarget.Both)
			{
				TouchManager.Device.CommitRightStick();
			}
		}
	}

	protected void SubmitRawAnalogValue(AnalogTarget target, Vector2 rawValue, ulong updateTick, float deltaTime)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (TouchManager.Device != null)
		{
			switch (target)
			{
			case AnalogTarget.None:
				return;
			case AnalogTarget.LeftStick:
			case AnalogTarget.Both:
				TouchManager.Device.UpdateLeftStickWithRawValue(rawValue, updateTick, deltaTime);
				break;
			}
			if (target == AnalogTarget.RightStick || target == AnalogTarget.Both)
			{
				TouchManager.Device.UpdateRightStickWithRawValue(rawValue, updateTick, deltaTime);
			}
		}
	}

	protected static Vector3 SnapTo(Vector2 vector, SnapAngles snapAngles)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (snapAngles == SnapAngles.None)
		{
			return Vector2.op_Implicit(vector);
		}
		float snapAngle = 360f / (float)snapAngles;
		return SnapTo(vector, snapAngle);
	}

	protected static Vector3 SnapTo(Vector2 vector, float snapAngle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector2.Angle(vector, Vector2.up);
		if (num < snapAngle / 2f)
		{
			return Vector2.op_Implicit(Vector2.up * ((Vector2)(ref vector)).magnitude);
		}
		if (num > 180f - snapAngle / 2f)
		{
			return Vector2.op_Implicit(-Vector2.up * ((Vector2)(ref vector)).magnitude);
		}
		float num2 = Mathf.Round(num / snapAngle);
		float num3 = num2 * snapAngle - num;
		Vector3 val = Vector3.Cross(Vector2.op_Implicit(Vector2.up), Vector2.op_Implicit(vector));
		Quaternion val2 = Quaternion.AngleAxis(num3, val);
		return val2 * Vector2.op_Implicit(vector);
	}

	private void OnDrawGizmosSelected()
	{
		if (((Behaviour)this).enabled && TouchManager.ControlsShowGizmos == TouchManager.GizmoShowOption.WhenSelected && !Utility.GameObjectIsCulledOnCurrentCamera(((Component)this).gameObject))
		{
			if (!Application.isPlaying)
			{
				ConfigureControl();
			}
			DrawGizmos();
		}
	}

	private void OnDrawGizmos()
	{
		if (!((Behaviour)this).enabled)
		{
			return;
		}
		if (TouchManager.ControlsShowGizmos == TouchManager.GizmoShowOption.UnlessPlaying)
		{
			if (Application.isPlaying)
			{
				return;
			}
		}
		else if (TouchManager.ControlsShowGizmos != TouchManager.GizmoShowOption.Always)
		{
			return;
		}
		if (!Utility.GameObjectIsCulledOnCurrentCamera(((Component)this).gameObject))
		{
			if (!Application.isPlaying)
			{
				ConfigureControl();
			}
			DrawGizmos();
		}
	}
}
