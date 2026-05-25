using UnityEngine;

namespace InControl;

public class TouchSwipeControl : TouchControl
{
	[Header("Position")]
	[SerializeField]
	private TouchUnitType areaUnitType;

	[SerializeField]
	private Rect activeArea = new Rect(25f, 25f, 50f, 50f);

	[Header("Options")]
	[Range(0f, 1f)]
	public float sensitivity = 0.1f;

	public bool oneSwipePerTouch;

	[Header("Analog Target")]
	public AnalogTarget target;

	public SnapAngles snapAngles;

	[Header("Button Targets")]
	public ButtonTarget upTarget;

	public ButtonTarget downTarget;

	public ButtonTarget leftTarget;

	public ButtonTarget rightTarget;

	public ButtonTarget tapTarget;

	private Rect worldActiveArea;

	private Vector3 currentVector;

	private bool currentVectorIsSet;

	private Vector3 beganPosition;

	private Vector3 lastPosition;

	private Touch currentTouch;

	private bool fireButtonTarget;

	private ButtonTarget nextButtonTarget;

	private ButtonTarget lastButtonTarget;

	private bool dirty;

	public Rect ActiveArea
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return activeArea;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (activeArea != value)
			{
				activeArea = value;
				dirty = true;
			}
		}
	}

	public TouchUnitType AreaUnitType
	{
		get
		{
			return areaUnitType;
		}
		set
		{
			if (areaUnitType != value)
			{
				areaUnitType = value;
				dirty = true;
			}
		}
	}

	public override void CreateControl()
	{
	}

	public override void DestroyControl()
	{
		if (currentTouch != null)
		{
			TouchEnded(currentTouch);
			currentTouch = null;
		}
	}

	public override void ConfigureControl()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		worldActiveArea = TouchManager.ConvertToWorld(activeArea, areaUnitType);
	}

	public override void DrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Utility.DrawRectGizmo(worldActiveArea, Color.yellow);
	}

	private void Update()
	{
		if (dirty)
		{
			ConfigureControl();
			dirty = false;
		}
	}

	public override void SubmitControlState(ulong updateTick, float deltaTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = TouchControl.SnapTo(Vector2.op_Implicit(currentVector), snapAngles);
		SubmitAnalogValue(target, Vector2.op_Implicit(val), 0f, 1f, updateTick, deltaTime);
		SubmitButtonState(upTarget, fireButtonTarget && nextButtonTarget == upTarget, updateTick, deltaTime);
		SubmitButtonState(downTarget, fireButtonTarget && nextButtonTarget == downTarget, updateTick, deltaTime);
		SubmitButtonState(leftTarget, fireButtonTarget && nextButtonTarget == leftTarget, updateTick, deltaTime);
		SubmitButtonState(rightTarget, fireButtonTarget && nextButtonTarget == rightTarget, updateTick, deltaTime);
		SubmitButtonState(tapTarget, fireButtonTarget && nextButtonTarget == tapTarget, updateTick, deltaTime);
		if (fireButtonTarget && nextButtonTarget != ButtonTarget.None)
		{
			fireButtonTarget = !oneSwipePerTouch;
			lastButtonTarget = nextButtonTarget;
			nextButtonTarget = ButtonTarget.None;
		}
	}

	public override void CommitControlState(ulong updateTick, float deltaTime)
	{
		CommitAnalog(target);
		CommitButton(upTarget);
		CommitButton(downTarget);
		CommitButton(leftTarget);
		CommitButton(rightTarget);
		CommitButton(tapTarget);
	}

	public override void TouchBegan(Touch touch)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (currentTouch == null)
		{
			beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
			if (((Rect)(ref worldActiveArea)).Contains(beganPosition))
			{
				lastPosition = beganPosition;
				currentTouch = touch;
				currentVector = Vector2.op_Implicit(Vector2.zero);
				currentVectorIsSet = false;
				fireButtonTarget = true;
				nextButtonTarget = ButtonTarget.None;
				lastButtonTarget = ButtonTarget.None;
			}
		}
	}

	public override void TouchMoved(Touch touch)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (currentTouch != touch)
		{
			return;
		}
		Vector3 val = TouchManager.ScreenToWorldPoint(touch.position);
		Vector3 val2 = val - lastPosition;
		if (!(((Vector3)(ref val2)).magnitude >= sensitivity))
		{
			return;
		}
		lastPosition = val;
		if (!oneSwipePerTouch || !currentVectorIsSet)
		{
			currentVector = ((Vector3)(ref val2)).normalized;
			currentVectorIsSet = true;
		}
		if (fireButtonTarget)
		{
			ButtonTarget buttonTargetForVector = GetButtonTargetForVector(Vector2.op_Implicit(currentVector));
			if (buttonTargetForVector != lastButtonTarget)
			{
				nextButtonTarget = buttonTargetForVector;
			}
		}
	}

	public override void TouchEnded(Touch touch)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (currentTouch == touch)
		{
			currentTouch = null;
			currentVector = Vector2.op_Implicit(Vector2.zero);
			currentVectorIsSet = false;
			Vector3 val = TouchManager.ScreenToWorldPoint(touch.position);
			Vector3 val2 = beganPosition - val;
			if (((Vector3)(ref val2)).magnitude < sensitivity)
			{
				fireButtonTarget = true;
				nextButtonTarget = tapTarget;
				lastButtonTarget = ButtonTarget.None;
			}
			else
			{
				fireButtonTarget = false;
				nextButtonTarget = ButtonTarget.None;
				lastButtonTarget = ButtonTarget.None;
			}
		}
	}

	private ButtonTarget GetButtonTargetForVector(Vector2 vector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.op_Implicit(TouchControl.SnapTo(vector, SnapAngles.Four));
		if (val == Vector2.up)
		{
			return upTarget;
		}
		if (val == Vector2.right)
		{
			return rightTarget;
		}
		if (val == -Vector2.up)
		{
			return downTarget;
		}
		if (val == -Vector2.right)
		{
			return leftTarget;
		}
		return ButtonTarget.None;
	}
}
