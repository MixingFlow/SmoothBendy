using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace InControl;

[AddComponentMenu("Event/InControl Input Module")]
public class InControlInputModule : PointerInputModule
{
	public enum Button
	{
		Action1 = 19,
		Action2,
		Action3,
		Action4
	}

	public Button submitButton = Button.Action1;

	public Button cancelButton = Button.Action2;

	[Range(0.1f, 0.9f)]
	public float analogMoveThreshold = 0.5f;

	public float moveRepeatFirstDuration = 0.8f;

	public float moveRepeatDelayDuration = 0.1f;

	[FormerlySerializedAs("allowMobileDevice")]
	public bool forceModuleActive;

	public bool allowMouseInput = true;

	public bool focusOnMouseHover;

	public bool allowTouchInput = true;

	private InputDevice inputDevice;

	private Vector3 thisMousePosition;

	private Vector3 lastMousePosition;

	private Vector2 thisVectorState;

	private Vector2 lastVectorState;

	private bool thisSubmitState;

	private bool lastSubmitState;

	private bool thisCancelState;

	private bool lastCancelState;

	private float nextMoveRepeatTime;

	private float lastVectorPressedTime;

	private TwoAxisInputControl direction;

	public PlayerAction SubmitAction { get; set; }

	public PlayerAction CancelAction { get; set; }

	public PlayerTwoAxisAction MoveAction { get; set; }

	public InputDevice Device
	{
		get
		{
			return inputDevice ?? InputManager.ActiveDevice;
		}
		set
		{
			inputDevice = value;
		}
	}

	private InputControl SubmitButton => Device.GetControl((InputControlType)submitButton);

	private InputControl CancelButton => Device.GetControl((InputControlType)cancelButton);

	private bool VectorIsPressed => thisVectorState != Vector2.zero;

	private bool VectorIsReleased => thisVectorState == Vector2.zero;

	private bool VectorHasChanged => thisVectorState != lastVectorState;

	private bool VectorWasPressed
	{
		get
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (VectorIsPressed && Time.realtimeSinceStartup > nextMoveRepeatTime)
			{
				return true;
			}
			return VectorIsPressed && lastVectorState == Vector2.zero;
		}
	}

	private bool SubmitWasPressed => thisSubmitState && thisSubmitState != lastSubmitState;

	private bool SubmitWasReleased => !thisSubmitState && thisSubmitState != lastSubmitState;

	private bool CancelWasPressed => thisCancelState && thisCancelState != lastCancelState;

	private bool MouseHasMoved
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = thisMousePosition - lastMousePosition;
			return ((Vector3)(ref val)).sqrMagnitude > 0f;
		}
	}

	private bool MouseButtonIsPressed => Input.GetMouseButtonDown(0);

	protected InControlInputModule()
	{
		direction = new TwoAxisInputControl();
		direction.StateThreshold = analogMoveThreshold;
	}

	public override void UpdateModule()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		lastMousePosition = thisMousePosition;
		thisMousePosition = Input.mousePosition;
	}

	public override bool IsModuleSupported()
	{
		if (forceModuleActive || Input.mousePresent || Input.touchSupported)
		{
			return true;
		}
		return false;
	}

	public override bool ShouldActivateModule()
	{
		if (!((Behaviour)this).enabled || !((Component)this).gameObject.activeInHierarchy)
		{
			return false;
		}
		UpdateInputState();
		bool flag = false;
		flag |= SubmitWasPressed;
		flag |= CancelWasPressed;
		flag |= VectorWasPressed;
		if (allowMouseInput)
		{
			flag |= MouseHasMoved;
			flag |= MouseButtonIsPressed;
		}
		if (allowTouchInput)
		{
			flag |= Input.touchCount > 0;
		}
		return flag;
	}

	public override void ActivateModule()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((BaseInputModule)this).ActivateModule();
		thisMousePosition = Input.mousePosition;
		lastMousePosition = Input.mousePosition;
		GameObject val = ((BaseInputModule)this).eventSystem.currentSelectedGameObject;
		if ((Object)(object)val == (Object)null)
		{
			val = ((BaseInputModule)this).eventSystem.firstSelectedGameObject;
		}
		((BaseInputModule)this).eventSystem.SetSelectedGameObject(val, ((BaseInputModule)this).GetBaseEventData());
	}

	public override void Process()
	{
		bool flag = SendUpdateEventToSelectedObject();
		if (((BaseInputModule)this).eventSystem.sendNavigationEvents)
		{
			if (!flag)
			{
				flag = SendVectorEventToSelectedObject();
			}
			if (!flag)
			{
				SendButtonEventToSelectedObject();
			}
		}
		if ((!allowTouchInput || !ProcessTouchEvents()) && allowMouseInput)
		{
			ProcessMouseEvent();
		}
	}

	private bool ProcessTouchEvents()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		int touchCount = Input.touchCount;
		bool pressed = default(bool);
		bool flag = default(bool);
		for (int i = 0; i < touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if ((int)((Touch)(ref touch)).type != 1)
			{
				PointerEventData touchPointerEventData = ((PointerInputModule)this).GetTouchPointerEventData(touch, ref pressed, ref flag);
				ProcessTouchPress(touchPointerEventData, pressed, flag);
				if (!flag)
				{
					((PointerInputModule)this).ProcessMove(touchPointerEventData);
					((PointerInputModule)this).ProcessDrag(touchPointerEventData);
				}
				else
				{
					((PointerInputModule)this).RemovePointerData(touchPointerEventData);
				}
			}
		}
		return touchCount > 0;
	}

	private bool SendButtonEventToSelectedObject()
	{
		if ((Object)(object)((BaseInputModule)this).eventSystem.currentSelectedGameObject == (Object)null)
		{
			return false;
		}
		BaseEventData baseEventData = ((BaseInputModule)this).GetBaseEventData();
		if (SubmitWasPressed)
		{
			ExecuteEvents.Execute<ISubmitHandler>(((BaseInputModule)this).eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
		}
		else if (!SubmitWasReleased)
		{
		}
		if (CancelWasPressed)
		{
			ExecuteEvents.Execute<ICancelHandler>(((BaseInputModule)this).eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
		}
		return ((AbstractEventData)baseEventData).used;
	}

	private bool SendVectorEventToSelectedObject()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		if (!VectorWasPressed)
		{
			return false;
		}
		AxisEventData axisEventData = ((BaseInputModule)this).GetAxisEventData(thisVectorState.x, thisVectorState.y, 0.5f);
		if ((int)axisEventData.moveDir != 4)
		{
			if ((Object)(object)((BaseInputModule)this).eventSystem.currentSelectedGameObject == (Object)null)
			{
				((BaseInputModule)this).eventSystem.SetSelectedGameObject(((BaseInputModule)this).eventSystem.firstSelectedGameObject, ((BaseInputModule)this).GetBaseEventData());
			}
			else
			{
				ExecuteEvents.Execute<IMoveHandler>(((BaseInputModule)this).eventSystem.currentSelectedGameObject, (BaseEventData)(object)axisEventData, ExecuteEvents.moveHandler);
			}
			SetVectorRepeatTimer();
		}
		return ((AbstractEventData)axisEventData).used;
	}

	protected override void ProcessMove(PointerEventData pointerEvent)
	{
		GameObject pointerEnter = pointerEvent.pointerEnter;
		((PointerInputModule)this).ProcessMove(pointerEvent);
		if (focusOnMouseHover && (Object)(object)pointerEnter != (Object)(object)pointerEvent.pointerEnter)
		{
			GameObject eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(pointerEvent.pointerEnter);
			((BaseInputModule)this).eventSystem.SetSelectedGameObject(eventHandler, (BaseEventData)(object)pointerEvent);
		}
	}

	private void Update()
	{
		direction.Filter(Device.Direction, Time.deltaTime);
	}

	private void UpdateInputState()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		lastVectorState = thisVectorState;
		thisVectorState = Vector2.zero;
		TwoAxisInputControl twoAxisInputControl = MoveAction ?? direction;
		if (Utility.AbsoluteIsOverThreshold(twoAxisInputControl.X, analogMoveThreshold))
		{
			thisVectorState.x = Mathf.Sign(twoAxisInputControl.X);
		}
		if (Utility.AbsoluteIsOverThreshold(twoAxisInputControl.Y, analogMoveThreshold))
		{
			thisVectorState.y = Mathf.Sign(twoAxisInputControl.Y);
		}
		if (VectorIsReleased)
		{
			nextMoveRepeatTime = 0f;
		}
		if (VectorIsPressed)
		{
			if (lastVectorState == Vector2.zero)
			{
				if (Time.realtimeSinceStartup > lastVectorPressedTime + 0.1f)
				{
					nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatFirstDuration;
				}
				else
				{
					nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatDelayDuration;
				}
			}
			lastVectorPressedTime = Time.realtimeSinceStartup;
		}
		lastSubmitState = thisSubmitState;
		thisSubmitState = ((SubmitAction != null) ? SubmitAction.IsPressed : SubmitButton.IsPressed);
		lastCancelState = thisCancelState;
		thisCancelState = ((CancelAction != null) ? CancelAction.IsPressed : CancelButton.IsPressed);
	}

	private void SetVectorRepeatTimer()
	{
		nextMoveRepeatTime = Mathf.Max(nextMoveRepeatTime, Time.realtimeSinceStartup + moveRepeatDelayDuration);
	}

	protected bool SendUpdateEventToSelectedObject()
	{
		if ((Object)(object)((BaseInputModule)this).eventSystem.currentSelectedGameObject == (Object)null)
		{
			return false;
		}
		BaseEventData baseEventData = ((BaseInputModule)this).GetBaseEventData();
		ExecuteEvents.Execute<IUpdateSelectedHandler>(((BaseInputModule)this).eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
		return ((AbstractEventData)baseEventData).used;
	}

	protected void ProcessMouseEvent()
	{
		ProcessMouseEvent(0);
	}

	protected void ProcessMouseEvent(int id)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		MouseState mousePointerEventData = ((PointerInputModule)this).GetMousePointerEventData(id);
		MouseButtonEventData eventData = mousePointerEventData.GetButtonState((InputButton)0).eventData;
		ProcessMousePress(eventData);
		((PointerInputModule)this).ProcessMove(eventData.buttonData);
		((PointerInputModule)this).ProcessDrag(eventData.buttonData);
		ProcessMousePress(mousePointerEventData.GetButtonState((InputButton)1).eventData);
		((PointerInputModule)this).ProcessDrag(mousePointerEventData.GetButtonState((InputButton)1).eventData.buttonData);
		ProcessMousePress(mousePointerEventData.GetButtonState((InputButton)2).eventData);
		((PointerInputModule)this).ProcessDrag(mousePointerEventData.GetButtonState((InputButton)2).eventData.buttonData);
		Vector2 scrollDelta = eventData.buttonData.scrollDelta;
		if (!Mathf.Approximately(((Vector2)(ref scrollDelta)).sqrMagnitude, 0f))
		{
			RaycastResult pointerCurrentRaycast = eventData.buttonData.pointerCurrentRaycast;
			GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(((RaycastResult)(ref pointerCurrentRaycast)).gameObject);
			ExecuteEvents.ExecuteHierarchy<IScrollHandler>(eventHandler, (BaseEventData)(object)eventData.buttonData, ExecuteEvents.scrollHandler);
		}
	}

	protected void ProcessMousePress(MouseButtonEventData data)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		PointerEventData buttonData = data.buttonData;
		RaycastResult pointerCurrentRaycast = buttonData.pointerCurrentRaycast;
		GameObject gameObject = ((RaycastResult)(ref pointerCurrentRaycast)).gameObject;
		if (data.PressedThisFrame())
		{
			buttonData.eligibleForClick = true;
			buttonData.delta = Vector2.zero;
			buttonData.dragging = false;
			buttonData.useDragThreshold = true;
			buttonData.pressPosition = buttonData.position;
			buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
			((PointerInputModule)this).DeselectIfSelectionChanged(gameObject, (BaseEventData)(object)buttonData);
			GameObject val = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, (BaseEventData)(object)buttonData, ExecuteEvents.pointerDownHandler);
			if ((Object)(object)val == (Object)null)
			{
				val = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			}
			float unscaledTime = Time.unscaledTime;
			if ((Object)(object)val == (Object)(object)buttonData.lastPress)
			{
				float num = unscaledTime - buttonData.clickTime;
				if (num < 0.3f)
				{
					buttonData.clickCount += 1;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.clickTime = unscaledTime;
			}
			else
			{
				buttonData.clickCount = 1;
			}
			buttonData.pointerPress = val;
			buttonData.rawPointerPress = gameObject;
			buttonData.clickTime = unscaledTime;
			buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
			if ((Object)(object)buttonData.pointerDrag != (Object)null)
			{
				ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, (BaseEventData)(object)buttonData, ExecuteEvents.initializePotentialDrag);
			}
		}
		if (data.ReleasedThisFrame())
		{
			ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, (BaseEventData)(object)buttonData, ExecuteEvents.pointerUpHandler);
			GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			if ((Object)(object)buttonData.pointerPress == (Object)(object)eventHandler && buttonData.eligibleForClick)
			{
				ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, (BaseEventData)(object)buttonData, ExecuteEvents.pointerClickHandler);
			}
			else if ((Object)(object)buttonData.pointerDrag != (Object)null && buttonData.dragging)
			{
				ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, (BaseEventData)(object)buttonData, ExecuteEvents.dropHandler);
			}
			buttonData.eligibleForClick = false;
			buttonData.pointerPress = null;
			buttonData.rawPointerPress = null;
			if ((Object)(object)buttonData.pointerDrag != (Object)null && buttonData.dragging)
			{
				ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, (BaseEventData)(object)buttonData, ExecuteEvents.endDragHandler);
			}
			buttonData.dragging = false;
			buttonData.pointerDrag = null;
			if ((Object)(object)gameObject != (Object)(object)buttonData.pointerEnter)
			{
				((BaseInputModule)this).HandlePointerExitAndEnter(buttonData, (GameObject)null);
				((BaseInputModule)this).HandlePointerExitAndEnter(buttonData, gameObject);
			}
		}
	}

	protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		RaycastResult pointerCurrentRaycast = pointerEvent.pointerCurrentRaycast;
		GameObject gameObject = ((RaycastResult)(ref pointerCurrentRaycast)).gameObject;
		if (pressed)
		{
			pointerEvent.eligibleForClick = true;
			pointerEvent.delta = Vector2.zero;
			pointerEvent.dragging = false;
			pointerEvent.useDragThreshold = true;
			pointerEvent.pressPosition = pointerEvent.position;
			pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
			((PointerInputModule)this).DeselectIfSelectionChanged(gameObject, (BaseEventData)(object)pointerEvent);
			if ((Object)(object)pointerEvent.pointerEnter != (Object)(object)gameObject)
			{
				((BaseInputModule)this).HandlePointerExitAndEnter(pointerEvent, gameObject);
				pointerEvent.pointerEnter = gameObject;
			}
			GameObject val = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, (BaseEventData)(object)pointerEvent, ExecuteEvents.pointerDownHandler);
			if ((Object)(object)val == (Object)null)
			{
				val = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			}
			float unscaledTime = Time.unscaledTime;
			if ((Object)(object)val == (Object)(object)pointerEvent.lastPress)
			{
				float num = unscaledTime - pointerEvent.clickTime;
				if (num < 0.3f)
				{
					pointerEvent.clickCount += 1;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.clickTime = unscaledTime;
			}
			else
			{
				pointerEvent.clickCount = 1;
			}
			pointerEvent.pointerPress = val;
			pointerEvent.rawPointerPress = gameObject;
			pointerEvent.clickTime = unscaledTime;
			pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
			if ((Object)(object)pointerEvent.pointerDrag != (Object)null)
			{
				ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, (BaseEventData)(object)pointerEvent, ExecuteEvents.initializePotentialDrag);
			}
		}
		if (released)
		{
			ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, (BaseEventData)(object)pointerEvent, ExecuteEvents.pointerUpHandler);
			GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			if ((Object)(object)pointerEvent.pointerPress == (Object)(object)eventHandler && pointerEvent.eligibleForClick)
			{
				ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, (BaseEventData)(object)pointerEvent, ExecuteEvents.pointerClickHandler);
			}
			else if ((Object)(object)pointerEvent.pointerDrag != (Object)null && pointerEvent.dragging)
			{
				ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, (BaseEventData)(object)pointerEvent, ExecuteEvents.dropHandler);
			}
			pointerEvent.eligibleForClick = false;
			pointerEvent.pointerPress = null;
			pointerEvent.rawPointerPress = null;
			if ((Object)(object)pointerEvent.pointerDrag != (Object)null && pointerEvent.dragging)
			{
				ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, (BaseEventData)(object)pointerEvent, ExecuteEvents.endDragHandler);
			}
			pointerEvent.dragging = false;
			pointerEvent.pointerDrag = null;
			if ((Object)(object)pointerEvent.pointerDrag != (Object)null)
			{
				ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, (BaseEventData)(object)pointerEvent, ExecuteEvents.endDragHandler);
			}
			pointerEvent.pointerDrag = null;
			ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, (BaseEventData)(object)pointerEvent, ExecuteEvents.pointerExitHandler);
			pointerEvent.pointerEnter = null;
		}
	}
}
