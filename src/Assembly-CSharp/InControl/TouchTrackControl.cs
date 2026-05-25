using UnityEngine;

namespace InControl;

public class TouchTrackControl : TouchControl
{
	[Header("Dimensions")]
	[SerializeField]
	private TouchUnitType areaUnitType;

	[SerializeField]
	private Rect activeArea = new Rect(25f, 25f, 50f, 50f);

	[Header("Analog Target")]
	public AnalogTarget target = AnalogTarget.LeftStick;

	public float scale = 1f;

	[Header("Button Target")]
	public ButtonTarget tapTarget;

	public float maxTapDuration = 0.5f;

	public float maxTapMovement = 1f;

	private Rect worldActiveArea;

	private Vector3 lastPosition;

	private Vector3 thisPosition;

	private Touch currentTouch;

	private bool dirty;

	private bool fireButtonTarget;

	private float beganTime;

	private Vector3 beganPosition;

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
		ConfigureControl();
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

	private void OnValidate()
	{
		if (maxTapDuration < 0f)
		{
			maxTapDuration = 0f;
		}
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = thisPosition - lastPosition;
		SubmitRawAnalogValue(target, Vector2.op_Implicit(val * scale), updateTick, deltaTime);
		lastPosition = thisPosition;
		SubmitButtonState(tapTarget, fireButtonTarget, updateTick, deltaTime);
		fireButtonTarget = false;
	}

	public override void CommitControlState(ulong updateTick, float deltaTime)
	{
		CommitAnalog(target);
		CommitButton(tapTarget);
	}

	public override void TouchBegan(Touch touch)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (currentTouch == null)
		{
			beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
			if (((Rect)(ref worldActiveArea)).Contains(beganPosition))
			{
				thisPosition = TouchManager.ScreenToViewPoint(touch.position * 100f);
				lastPosition = thisPosition;
				currentTouch = touch;
				beganTime = Time.realtimeSinceStartup;
			}
		}
	}

	public override void TouchMoved(Touch touch)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (currentTouch == touch)
		{
			thisPosition = TouchManager.ScreenToViewPoint(touch.position * 100f);
		}
	}

	public override void TouchEnded(Touch touch)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (currentTouch == touch)
		{
			Vector3 val = TouchManager.ScreenToWorldPoint(touch.position);
			Vector3 val2 = val - beganPosition;
			float num = Time.realtimeSinceStartup - beganTime;
			if (((Vector3)(ref val2)).magnitude <= maxTapMovement && num <= maxTapDuration && tapTarget != ButtonTarget.None)
			{
				fireButtonTarget = true;
			}
			thisPosition = Vector3.zero;
			lastPosition = Vector3.zero;
			currentTouch = null;
		}
	}
}
