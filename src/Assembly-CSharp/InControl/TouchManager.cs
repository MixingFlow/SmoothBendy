using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl;

[ExecuteInEditMode]
public class TouchManager : SingletonMonoBehavior<TouchManager>
{
	public enum GizmoShowOption
	{
		Never,
		WhenSelected,
		UnlessPlaying,
		Always
	}

	[Space(10f)]
	public Camera touchCamera;

	public GizmoShowOption controlsShowGizmos = GizmoShowOption.Always;

	[HideInInspector]
	public bool enableControlsOnTouch;

	[SerializeField]
	[HideInInspector]
	private bool _controlsEnabled = true;

	[HideInInspector]
	public int controlsLayer = 5;

	private InputDevice device;

	private Vector3 viewSize;

	private Vector2 screenSize;

	private Vector2 halfScreenSize;

	private float percentToWorld;

	private float halfPercentToWorld;

	private float pixelToWorld;

	private float halfPixelToWorld;

	private TouchControl[] touchControls;

	private TouchPool cachedTouches;

	private List<Touch> activeTouches;

	private ReadOnlyCollection<Touch> readOnlyActiveTouches;

	private Vector2 lastMousePosition;

	private bool isReady;

	private Touch mouseTouch;

	public bool controlsEnabled
	{
		get
		{
			return _controlsEnabled;
		}
		set
		{
			if (_controlsEnabled != value)
			{
				int num = touchControls.Length;
				for (int i = 0; i < num; i++)
				{
					((Behaviour)touchControls[i]).enabled = value;
				}
				_controlsEnabled = value;
			}
		}
	}

	public static ReadOnlyCollection<Touch> Touches => SingletonMonoBehavior<TouchManager>.Instance.readOnlyActiveTouches;

	public static int TouchCount => SingletonMonoBehavior<TouchManager>.Instance.activeTouches.Count;

	public static Camera Camera => SingletonMonoBehavior<TouchManager>.Instance.touchCamera;

	public static InputDevice Device => SingletonMonoBehavior<TouchManager>.Instance.device;

	public static Vector3 ViewSize => SingletonMonoBehavior<TouchManager>.Instance.viewSize;

	public static float PercentToWorld => SingletonMonoBehavior<TouchManager>.Instance.percentToWorld;

	public static float HalfPercentToWorld => SingletonMonoBehavior<TouchManager>.Instance.halfPercentToWorld;

	public static float PixelToWorld => SingletonMonoBehavior<TouchManager>.Instance.pixelToWorld;

	public static float HalfPixelToWorld => SingletonMonoBehavior<TouchManager>.Instance.halfPixelToWorld;

	public static Vector2 ScreenSize => SingletonMonoBehavior<TouchManager>.Instance.screenSize;

	public static Vector2 HalfScreenSize => SingletonMonoBehavior<TouchManager>.Instance.halfScreenSize;

	public static GizmoShowOption ControlsShowGizmos => SingletonMonoBehavior<TouchManager>.Instance.controlsShowGizmos;

	public static bool ControlsEnabled
	{
		get
		{
			return SingletonMonoBehavior<TouchManager>.Instance.controlsEnabled;
		}
		set
		{
			SingletonMonoBehavior<TouchManager>.Instance.controlsEnabled = value;
		}
	}

	public static event Action OnSetup;

	protected TouchManager()
	{
	}

	private void OnEnable()
	{
		InControlManager component = ((Component)this).GetComponent<InControlManager>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)"Touch Manager component can only be added to the InControl Manager object.");
			Object.DestroyImmediate((Object)(object)this);
		}
		else if (!base.EnforceSingleton)
		{
			touchControls = ((Component)this).GetComponentsInChildren<TouchControl>(true);
			if (Application.isPlaying)
			{
				InputManager.OnSetup += Setup;
				InputManager.OnUpdateDevices += UpdateDevice;
				InputManager.OnCommitDevices += CommitDevice;
			}
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			InputManager.OnSetup -= Setup;
			InputManager.OnUpdateDevices -= UpdateDevice;
			InputManager.OnCommitDevices -= CommitDevice;
		}
		Reset();
	}

	private void Setup()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateScreenSize(GetCurrentScreenSize());
		CreateDevice();
		CreateTouches();
		if (TouchManager.OnSetup != null)
		{
			TouchManager.OnSetup();
			TouchManager.OnSetup = null;
		}
	}

	private void Reset()
	{
		device = null;
		mouseTouch = null;
		cachedTouches = null;
		activeTouches = null;
		readOnlyActiveTouches = null;
		touchControls = null;
		TouchManager.OnSetup = null;
	}

	private IEnumerator UpdateScreenSizeAtEndOfFrame()
	{
		yield return (object)new WaitForEndOfFrame();
		UpdateScreenSize(GetCurrentScreenSize());
		yield return null;
	}

	private void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector2 currentScreenSize = GetCurrentScreenSize();
		if (!isReady)
		{
			((MonoBehaviour)this).StartCoroutine(UpdateScreenSizeAtEndOfFrame());
			UpdateScreenSize(currentScreenSize);
			isReady = true;
			return;
		}
		if (screenSize != currentScreenSize)
		{
			UpdateScreenSize(currentScreenSize);
		}
		if (TouchManager.OnSetup != null)
		{
			TouchManager.OnSetup();
			TouchManager.OnSetup = null;
		}
	}

	private void CreateDevice()
	{
		device = new TouchInputDevice();
		device.AddControl(InputControlType.LeftStickLeft, "LeftStickLeft");
		device.AddControl(InputControlType.LeftStickRight, "LeftStickRight");
		device.AddControl(InputControlType.LeftStickUp, "LeftStickUp");
		device.AddControl(InputControlType.LeftStickDown, "LeftStickDown");
		device.AddControl(InputControlType.RightStickLeft, "RightStickLeft");
		device.AddControl(InputControlType.RightStickRight, "RightStickRight");
		device.AddControl(InputControlType.RightStickUp, "RightStickUp");
		device.AddControl(InputControlType.RightStickDown, "RightStickDown");
		device.AddControl(InputControlType.DPadUp, "DPadUp");
		device.AddControl(InputControlType.DPadDown, "DPadDown");
		device.AddControl(InputControlType.DPadLeft, "DPadLeft");
		device.AddControl(InputControlType.DPadRight, "DPadRight");
		device.AddControl(InputControlType.LeftTrigger, "LeftTrigger");
		device.AddControl(InputControlType.RightTrigger, "RightTrigger");
		device.AddControl(InputControlType.LeftBumper, "LeftBumper");
		device.AddControl(InputControlType.RightBumper, "RightBumper");
		for (InputControlType inputControlType = InputControlType.Action1; inputControlType <= InputControlType.Action4; inputControlType++)
		{
			device.AddControl(inputControlType, inputControlType.ToString());
		}
		device.AddControl(InputControlType.Menu, "Menu");
		for (InputControlType inputControlType2 = InputControlType.Button0; inputControlType2 <= InputControlType.Button19; inputControlType2++)
		{
			device.AddControl(inputControlType2, inputControlType2.ToString());
		}
		InputManager.AttachDevice(device);
	}

	private void UpdateDevice(ulong updateTick, float deltaTime)
	{
		UpdateTouches(updateTick, deltaTime);
		SubmitControlStates(updateTick, deltaTime);
	}

	private void CommitDevice(ulong updateTick, float deltaTime)
	{
		CommitControlStates(updateTick, deltaTime);
	}

	private void SubmitControlStates(ulong updateTick, float deltaTime)
	{
		int num = touchControls.Length;
		for (int i = 0; i < num; i++)
		{
			TouchControl touchControl = touchControls[i];
			if (((Behaviour)touchControl).enabled && ((Component)touchControl).gameObject.activeInHierarchy)
			{
				touchControl.SubmitControlState(updateTick, deltaTime);
			}
		}
	}

	private void CommitControlStates(ulong updateTick, float deltaTime)
	{
		int num = touchControls.Length;
		for (int i = 0; i < num; i++)
		{
			TouchControl touchControl = touchControls[i];
			if (((Behaviour)touchControl).enabled && ((Component)touchControl).gameObject.activeInHierarchy)
			{
				touchControl.CommitControlState(updateTick, deltaTime);
			}
		}
	}

	private void UpdateScreenSize(Vector2 currentScreenSize)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		touchCamera.rect = new Rect(0f, 0f, 0.99f, 1f);
		touchCamera.rect = new Rect(0f, 0f, 1f, 1f);
		screenSize = currentScreenSize;
		halfScreenSize = screenSize / 2f;
		viewSize = ConvertViewToWorldPoint(Vector2.one) * 0.02f;
		percentToWorld = Mathf.Min(viewSize.x, viewSize.y);
		halfPercentToWorld = percentToWorld / 2f;
		if ((Object)(object)touchCamera != (Object)null)
		{
			halfPixelToWorld = touchCamera.orthographicSize / screenSize.y;
			pixelToWorld = halfPixelToWorld * 2f;
		}
		if (touchControls != null)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				touchControls[i].ConfigureControl();
			}
		}
	}

	private void CreateTouches()
	{
		cachedTouches = new TouchPool();
		mouseTouch = new Touch();
		mouseTouch.fingerId = Touch.FingerID_Mouse;
		activeTouches = new List<Touch>(32);
		readOnlyActiveTouches = new ReadOnlyCollection<Touch>(activeTouches);
	}

	private void UpdateTouches(ulong updateTick, float deltaTime)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Invalid comparison between Unknown and I4
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		activeTouches.Clear();
		cachedTouches.FreeEndedTouches();
		if (mouseTouch.SetWithMouseData(updateTick, deltaTime))
		{
			activeTouches.Add(mouseTouch);
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			Touch touch2 = cachedTouches.FindOrCreateTouch(((Touch)(ref touch)).fingerId);
			touch2.SetWithTouchData(touch, updateTick, deltaTime);
			activeTouches.Add(touch2);
		}
		int count = cachedTouches.Touches.Count;
		for (int j = 0; j < count; j++)
		{
			Touch touch3 = cachedTouches.Touches[j];
			if ((int)touch3.phase != 3 && touch3.updateTick != updateTick)
			{
				touch3.phase = (TouchPhase)3;
				activeTouches.Add(touch3);
			}
		}
		InvokeTouchEvents();
	}

	private void SendTouchBegan(Touch touch)
	{
		int num = touchControls.Length;
		for (int i = 0; i < num; i++)
		{
			TouchControl touchControl = touchControls[i];
			if (((Behaviour)touchControl).enabled && ((Component)touchControl).gameObject.activeInHierarchy)
			{
				touchControl.TouchBegan(touch);
			}
		}
	}

	private void SendTouchMoved(Touch touch)
	{
		int num = touchControls.Length;
		for (int i = 0; i < num; i++)
		{
			TouchControl touchControl = touchControls[i];
			if (((Behaviour)touchControl).enabled && ((Component)touchControl).gameObject.activeInHierarchy)
			{
				touchControl.TouchMoved(touch);
			}
		}
	}

	private void SendTouchEnded(Touch touch)
	{
		int num = touchControls.Length;
		for (int i = 0; i < num; i++)
		{
			TouchControl touchControl = touchControls[i];
			if (((Behaviour)touchControl).enabled && ((Component)touchControl).gameObject.activeInHierarchy)
			{
				touchControl.TouchEnded(touch);
			}
		}
	}

	private void InvokeTouchEvents()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected I4, but got Unknown
		int count = activeTouches.Count;
		if (enableControlsOnTouch && count > 0 && !controlsEnabled)
		{
			Device.RequestActivation();
			controlsEnabled = true;
		}
		for (int i = 0; i < count; i++)
		{
			Touch touch = activeTouches[i];
			TouchPhase phase = touch.phase;
			switch ((int)phase)
			{
			case 0:
				SendTouchBegan(touch);
				break;
			case 1:
				SendTouchMoved(touch);
				break;
			case 3:
				SendTouchEnded(touch);
				break;
			case 4:
				SendTouchEnded(touch);
				break;
			}
		}
	}

	private bool TouchCameraIsValid()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)touchCamera == (Object)null)
		{
			return false;
		}
		if (Utility.IsZero(touchCamera.orthographicSize))
		{
			return false;
		}
		Rect rect = touchCamera.rect;
		if (Utility.IsZero(((Rect)(ref rect)).width))
		{
			Rect rect2 = touchCamera.rect;
			if (Utility.IsZero(((Rect)(ref rect2)).height))
			{
				return false;
			}
		}
		Rect pixelRect = touchCamera.pixelRect;
		if (Utility.IsZero(((Rect)(ref pixelRect)).width))
		{
			Rect pixelRect2 = touchCamera.pixelRect;
			if (Utility.IsZero(((Rect)(ref pixelRect2)).height))
			{
				return false;
			}
		}
		return true;
	}

	private Vector3 ConvertScreenToWorldPoint(Vector2 point)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (TouchCameraIsValid())
		{
			return touchCamera.ScreenToWorldPoint(new Vector3(point.x, point.y, 0f - ((Component)touchCamera).transform.position.z));
		}
		return Vector3.zero;
	}

	private Vector3 ConvertViewToWorldPoint(Vector2 point)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (TouchCameraIsValid())
		{
			return touchCamera.ViewportToWorldPoint(new Vector3(point.x, point.y, 0f - ((Component)touchCamera).transform.position.z));
		}
		return Vector3.zero;
	}

	private Vector3 ConvertScreenToViewPoint(Vector2 point)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (TouchCameraIsValid())
		{
			return touchCamera.ScreenToViewportPoint(new Vector3(point.x, point.y, 0f - ((Component)touchCamera).transform.position.z));
		}
		return Vector3.zero;
	}

	private Vector2 GetCurrentScreenSize()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (TouchCameraIsValid())
		{
			return new Vector2((float)touchCamera.pixelWidth, (float)touchCamera.pixelHeight);
		}
		return new Vector2((float)Screen.width, (float)Screen.height);
	}

	public static Touch GetTouch(int touchIndex)
	{
		return SingletonMonoBehavior<TouchManager>.Instance.activeTouches[touchIndex];
	}

	public static Touch GetTouchByFingerId(int fingerId)
	{
		return SingletonMonoBehavior<TouchManager>.Instance.cachedTouches.FindTouch(fingerId);
	}

	public static Vector3 ScreenToWorldPoint(Vector2 point)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return SingletonMonoBehavior<TouchManager>.Instance.ConvertScreenToWorldPoint(point);
	}

	public static Vector3 ViewToWorldPoint(Vector2 point)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return SingletonMonoBehavior<TouchManager>.Instance.ConvertViewToWorldPoint(point);
	}

	public static Vector3 ScreenToViewPoint(Vector2 point)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return SingletonMonoBehavior<TouchManager>.Instance.ConvertScreenToViewPoint(point);
	}

	public static float ConvertToWorld(float value, TouchUnitType unitType)
	{
		return value * ((unitType != TouchUnitType.Pixels) ? PercentToWorld : PixelToWorld);
	}

	public static Rect PercentToWorldRect(Rect rect)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		return new Rect((((Rect)(ref rect)).xMin - 50f) * ViewSize.x, (((Rect)(ref rect)).yMin - 50f) * ViewSize.y, ((Rect)(ref rect)).width * ViewSize.x, ((Rect)(ref rect)).height * ViewSize.y);
	}

	public static Rect PixelToWorldRect(Rect rect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(Mathf.Round(((Rect)(ref rect)).xMin - HalfScreenSize.x) * PixelToWorld, Mathf.Round(((Rect)(ref rect)).yMin - HalfScreenSize.y) * PixelToWorld, Mathf.Round(((Rect)(ref rect)).width) * PixelToWorld, Mathf.Round(((Rect)(ref rect)).height) * PixelToWorld);
	}

	public static Rect ConvertToWorld(Rect rect, TouchUnitType unitType)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return (unitType != TouchUnitType.Pixels) ? PercentToWorldRect(rect) : PixelToWorldRect(rect);
	}

	public static implicit operator bool(TouchManager instance)
	{
		return (Object)(object)instance != (Object)null;
	}
}
