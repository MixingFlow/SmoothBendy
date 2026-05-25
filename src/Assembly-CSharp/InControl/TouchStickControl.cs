using UnityEngine;

namespace InControl;

public class TouchStickControl : TouchControl
{
	[Header("Position")]
	[SerializeField]
	private TouchControlAnchor anchor = TouchControlAnchor.BottomLeft;

	[SerializeField]
	private TouchUnitType offsetUnitType;

	[SerializeField]
	private Vector2 offset = new Vector2(20f, 20f);

	[SerializeField]
	private TouchUnitType areaUnitType;

	[SerializeField]
	private Rect activeArea = new Rect(0f, 0f, 50f, 100f);

	[Header("Options")]
	public AnalogTarget target = AnalogTarget.LeftStick;

	public SnapAngles snapAngles;

	public LockAxis lockToAxis;

	[Range(0f, 1f)]
	public float lowerDeadZone = 0.1f;

	[Range(0f, 1f)]
	public float upperDeadZone = 0.9f;

	public AnimationCurve inputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public bool allowDragging;

	public DragAxis allowDraggingAxis;

	public bool snapToInitialTouch = true;

	public bool resetWhenDone = true;

	public float resetDuration = 0.1f;

	[Header("Sprites")]
	public TouchSprite ring = new TouchSprite(20f);

	public TouchSprite knob = new TouchSprite(10f);

	public float knobRange = 7.5f;

	private Vector3 resetPosition;

	private Vector3 beganPosition;

	private Vector3 movedPosition;

	private float ringResetSpeed;

	private float knobResetSpeed;

	private Rect worldActiveArea;

	private float worldKnobRange;

	private Vector3 value;

	private Touch currentTouch;

	private bool dirty;

	public bool IsActive => currentTouch != null;

	public bool IsNotActive => currentTouch == null;

	public Vector3 RingPosition
	{
		get
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return (!ring.Ready) ? ((Component)this).transform.position : ring.Position;
		}
		set
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (ring.Ready)
			{
				ring.Position = value;
			}
		}
	}

	public Vector3 KnobPosition
	{
		get
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return (!knob.Ready) ? ((Component)this).transform.position : knob.Position;
		}
		set
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (knob.Ready)
			{
				knob.Position = value;
			}
		}
	}

	public TouchControlAnchor Anchor
	{
		get
		{
			return anchor;
		}
		set
		{
			if (anchor != value)
			{
				anchor = value;
				dirty = true;
			}
		}
	}

	public Vector2 Offset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return offset;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (offset != value)
			{
				offset = value;
				dirty = true;
			}
		}
	}

	public TouchUnitType OffsetUnitType
	{
		get
		{
			return offsetUnitType;
		}
		set
		{
			if (offsetUnitType != value)
			{
				offsetUnitType = value;
				dirty = true;
			}
		}
	}

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
		ring.Create("Ring", ((Component)this).transform, 1000);
		knob.Create("Knob", ((Component)this).transform, 1001);
	}

	public override void DestroyControl()
	{
		ring.Delete();
		knob.Delete();
		if (currentTouch != null)
		{
			TouchEnded(currentTouch);
			currentTouch = null;
		}
	}

	public override void ConfigureControl()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		resetPosition = OffsetToWorldPosition(anchor, offset, offsetUnitType, lockAspectRatio: true);
		((Component)this).transform.position = resetPosition;
		ring.Update(forceUpdate: true);
		knob.Update(forceUpdate: true);
		worldActiveArea = TouchManager.ConvertToWorld(activeArea, areaUnitType);
		worldKnobRange = TouchManager.ConvertToWorld(knobRange, knob.SizeUnitType);
	}

	public override void DrawGizmos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ring.DrawGizmos(RingPosition, Color.yellow);
		knob.DrawGizmos(KnobPosition, Color.yellow);
		Utility.DrawCircleGizmo(Vector2.op_Implicit(RingPosition), worldKnobRange, Color.red);
		Utility.DrawRectGizmo(worldActiveArea, Color.green);
	}

	private void Update()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			ConfigureControl();
			dirty = false;
		}
		else
		{
			ring.Update();
			knob.Update();
		}
		if (IsNotActive)
		{
			if (resetWhenDone && KnobPosition != resetPosition)
			{
				Vector3 val = KnobPosition - RingPosition;
				RingPosition = Vector3.MoveTowards(RingPosition, resetPosition, ringResetSpeed * Time.deltaTime);
				KnobPosition = RingPosition + val;
			}
			if (KnobPosition != RingPosition)
			{
				KnobPosition = Vector3.MoveTowards(KnobPosition, RingPosition, knobResetSpeed * Time.deltaTime);
			}
		}
	}

	public override void SubmitControlState(ulong updateTick, float deltaTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SubmitAnalogValue(target, Vector2.op_Implicit(value), lowerDeadZone, upperDeadZone, updateTick, deltaTime);
	}

	public override void CommitControlState(ulong updateTick, float deltaTime)
	{
		CommitAnalog(target);
	}

	public override void TouchBegan(Touch touch)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!IsActive)
		{
			beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
			bool flag = ((Rect)(ref worldActiveArea)).Contains(beganPosition);
			bool flag2 = ring.Contains(Vector2.op_Implicit(beganPosition));
			if (snapToInitialTouch && (flag || flag2))
			{
				RingPosition = beganPosition;
				KnobPosition = beganPosition;
				currentTouch = touch;
			}
			else if (flag2)
			{
				KnobPosition = beganPosition;
				beganPosition = RingPosition;
				currentTouch = touch;
			}
			if (IsActive)
			{
				TouchMoved(touch);
				ring.State = true;
				knob.State = true;
			}
		}
	}

	public override void TouchMoved(Touch touch)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (currentTouch != touch)
		{
			return;
		}
		movedPosition = TouchManager.ScreenToWorldPoint(touch.position);
		if (lockToAxis == LockAxis.Horizontal && allowDraggingAxis == DragAxis.Horizontal)
		{
			movedPosition.y = beganPosition.y;
		}
		else if (lockToAxis == LockAxis.Vertical && allowDraggingAxis == DragAxis.Vertical)
		{
			movedPosition.x = beganPosition.x;
		}
		Vector3 val = movedPosition - beganPosition;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		float magnitude = ((Vector3)(ref val)).magnitude;
		if (allowDragging)
		{
			float num = magnitude - worldKnobRange;
			if (num < 0f)
			{
				num = 0f;
			}
			Vector3 val2 = num * normalized;
			if (allowDraggingAxis == DragAxis.Horizontal)
			{
				val2.y = 0f;
			}
			else if (allowDraggingAxis == DragAxis.Vertical)
			{
				val2.x = 0f;
			}
			beganPosition += val2;
			RingPosition = beganPosition;
		}
		movedPosition = beganPosition + Mathf.Clamp(magnitude, 0f, worldKnobRange) * normalized;
		if (lockToAxis == LockAxis.Horizontal)
		{
			movedPosition.y = beganPosition.y;
		}
		else if (lockToAxis == LockAxis.Vertical)
		{
			movedPosition.x = beganPosition.x;
		}
		if (snapAngles != SnapAngles.None)
		{
			movedPosition = TouchControl.SnapTo(Vector2.op_Implicit(movedPosition - beganPosition), snapAngles) + beganPosition;
		}
		RingPosition = beganPosition;
		KnobPosition = movedPosition;
		value = (movedPosition - beganPosition) / worldKnobRange;
		value.x = inputCurve.Evaluate(Utility.Abs(value.x)) * Mathf.Sign(value.x);
		value.y = inputCurve.Evaluate(Utility.Abs(value.y)) * Mathf.Sign(value.y);
	}

	public override void TouchEnded(Touch touch)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (currentTouch == touch)
		{
			value = Vector3.zero;
			Vector3 val = resetPosition - RingPosition;
			float magnitude = ((Vector3)(ref val)).magnitude;
			ringResetSpeed = ((!Utility.IsZero(resetDuration)) ? (magnitude / resetDuration) : magnitude);
			Vector3 val2 = RingPosition - KnobPosition;
			float magnitude2 = ((Vector3)(ref val2)).magnitude;
			knobResetSpeed = ((!Utility.IsZero(resetDuration)) ? (magnitude2 / resetDuration) : knobRange);
			currentTouch = null;
			ring.State = false;
			knob.State = false;
		}
	}
}
