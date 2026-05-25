using System.Collections.Generic;
using UnityEngine;

namespace InControl;

public class TestInputManager : MonoBehaviour
{
	public Font font;

	private readonly GUIStyle style = new GUIStyle();

	private readonly List<LogMessage> logMessages = new List<LogMessage>();

	private bool isPaused;

	private void OnEnable()
	{
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 0;
		isPaused = false;
		Time.timeScale = 1f;
		Logger.OnLogMessage += delegate(LogMessage logMessage)
		{
			logMessages.Add(logMessage);
		};
		InputManager.OnDeviceAttached += delegate(InputDevice inputDevice)
		{
			Debug.Log((object)("Attached: " + inputDevice.Name));
		};
		InputManager.OnDeviceDetached += delegate(InputDevice inputDevice)
		{
			Debug.Log((object)("Detached: " + inputDevice.Name));
		};
		InputManager.OnActiveDeviceChanged += delegate(InputDevice inputDevice)
		{
			Debug.Log((object)("Active device changed to: " + inputDevice.Name));
		};
		InputManager.OnUpdate += HandleInputUpdate;
	}

	private void HandleInputUpdate(ulong updateTick, float deltaTime)
	{
		CheckForPauseButton();
		int count = InputManager.Devices.Count;
		for (int i = 0; i < count; i++)
		{
			InputDevice inputDevice = InputManager.Devices[i];
			inputDevice.Vibrate(inputDevice.LeftTrigger, inputDevice.RightTrigger);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown((KeyCode)114))
		{
			Utility.LoadScene("TestInputManager");
		}
		if (Input.GetKeyDown((KeyCode)101))
		{
			InputManager.Enabled = !InputManager.Enabled;
		}
	}

	private void CheckForPauseButton()
	{
		if (Input.GetKeyDown((KeyCode)112) || InputManager.CommandWasPressed)
		{
			Time.timeScale = ((!isPaused) ? 0f : 1f);
			isPaused = !isPaused;
		}
	}

	private void SetColor(Color color)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		style.normal.textColor = color;
	}

	private void OnGUI()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.FloorToInt((float)(Screen.width / Mathf.Max(1, InputManager.Devices.Count)));
		int num2 = 10;
		int num3 = 10;
		GUI.skin.font = font;
		SetColor(Color.white);
		string text = "Devices:";
		text = text + " (Platform: " + InputManager.Platform + ")";
		text = text + " " + InputManager.ActiveDevice.Direction.Vector;
		if (isPaused)
		{
			SetColor(Color.red);
			text = "+++ PAUSED +++";
		}
		GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text, style);
		SetColor(Color.white);
		foreach (InputDevice device in InputManager.Devices)
		{
			Color val = (Color)((!device.IsActive) ? Color.white : new Color(0.9f, 0.7f, 0.2f));
			bool flag = InputManager.ActiveDevice == device;
			if (flag)
			{
				((Color)(ref val))._002Ector(1f, 0.9f, 0f);
			}
			num3 = 35;
			if (device.IsUnknown)
			{
				SetColor(Color.red);
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), "Unknown Device", style);
			}
			else
			{
				SetColor(val);
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), device.Name, style);
			}
			num3 += 15;
			SetColor(val);
			if (device.IsUnknown)
			{
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), device.Meta, style);
				num3 += 15;
			}
			GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), "Style: " + device.DeviceStyle, style);
			num3 += 15;
			GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), "GUID: " + device.GUID, style);
			num3 += 15;
			GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), "SortOrder: " + device.SortOrder, style);
			num3 += 15;
			GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), "LastInputTick: " + device.LastInputTick, style);
			num3 += 15;
			if (device is NativeInputDevice nativeInputDevice)
			{
				string text2 = $"VID = 0x{nativeInputDevice.Info.vendorID:x}, PID = 0x{nativeInputDevice.Info.productID:x}, VER = 0x{nativeInputDevice.Info.versionNumber:x}";
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text2, style);
				num3 += 15;
			}
			num3 += 15;
			foreach (InputControl control in device.Controls)
			{
				if (control != null && !Utility.TargetIsAlias(control.Target))
				{
					string arg = ((!device.IsKnown) ? control.Handle : $"{control.Target} ({control.Handle})");
					SetColor((!control.State) ? val : Color.green);
					string text3 = string.Format("{0} {1}", arg, (!control.State) ? string.Empty : ("= " + control.Value));
					GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text3, style);
					num3 += 15;
				}
			}
			num3 += 15;
			val = (Color)((!flag) ? Color.white : new Color(0.85f, 0.65f, 0.12f));
			if (device.IsKnown)
			{
				InputControl command = device.Command;
				SetColor((!command.State) ? val : Color.green);
				string text4 = string.Format("{0} {1}", "Command", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
				command = device.LeftStickX;
				SetColor((!command.State) ? val : Color.green);
				text4 = string.Format("{0} {1}", "Left Stick X", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
				command = device.LeftStickY;
				SetColor((!command.State) ? val : Color.green);
				text4 = string.Format("{0} {1}", "Left Stick Y", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
				SetColor((!device.LeftStick.State) ? val : Color.green);
				text4 = string.Format("{0} {1}", "Left Stick A", (!device.LeftStick.State) ? string.Empty : ("= " + device.LeftStick.Angle));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
				command = device.RightStickX;
				SetColor((!command.State) ? val : Color.green);
				text4 = string.Format("{0} {1}", "Right Stick X", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
				command = device.RightStickY;
				SetColor((!command.State) ? val : Color.green);
				text4 = string.Format("{0} {1}", "Right Stick Y", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
				SetColor((!device.RightStick.State) ? val : Color.green);
				text4 = string.Format("{0} {1}", "Right Stick A", (!device.RightStick.State) ? string.Empty : ("= " + device.RightStick.Angle));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
				command = device.DPadX;
				SetColor((!command.State) ? val : Color.green);
				text4 = string.Format("{0} {1}", "DPad X", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
				command = device.DPadY;
				SetColor((!command.State) ? val : Color.green);
				text4 = string.Format("{0} {1}", "DPad Y", (!command.State) ? string.Empty : ("= " + command.Value));
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text4, style);
				num3 += 15;
			}
			SetColor(Color.cyan);
			InputControl anyButton = device.AnyButton;
			if ((bool)anyButton)
			{
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), "AnyButton = " + anyButton.Handle, style);
			}
			num2 += num;
		}
		Color[] array = (Color[])(object)new Color[3]
		{
			Color.gray,
			Color.yellow,
			Color.white
		};
		SetColor(Color.white);
		num2 = 10;
		num3 = Screen.height - 25;
		for (int num4 = logMessages.Count - 1; num4 >= 0; num4--)
		{
			LogMessage logMessage = logMessages[num4];
			if (logMessage.type != LogMessageType.Info)
			{
				SetColor(array[(int)logMessage.type]);
				string[] array2 = logMessage.text.Split('\n');
				foreach (string text5 in array2)
				{
					GUI.Label(new Rect((float)num2, (float)num3, (float)Screen.width, (float)(num3 + 10)), text5, style);
					num3 -= 15;
				}
			}
		}
	}

	private void DrawUnityInputDebugger()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		int num = 300;
		int num2 = Screen.width / 2;
		int num3 = 10;
		int num4 = 20;
		SetColor(Color.white);
		string[] joystickNames = Input.GetJoystickNames();
		int num5 = joystickNames.Length;
		for (int i = 0; i < num5; i++)
		{
			string text = joystickNames[i];
			int num6 = i + 1;
			GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), "Joystick " + num6 + ": \"" + text + "\"", style);
			num3 += num4;
			string text2 = "Buttons: ";
			for (int j = 0; j < 20; j++)
			{
				string text3 = "joystick " + num6 + " button " + j;
				if (Input.GetKey(text3))
				{
					string text4 = text2;
					text2 = text4 + "B" + j + "  ";
				}
			}
			GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text2, style);
			num3 += num4;
			string text5 = "Analogs: ";
			for (int k = 0; k < 20; k++)
			{
				string text6 = "joystick " + num6 + " analog " + k;
				float axisRaw = Input.GetAxisRaw(text6);
				if (Utility.AbsoluteIsOverThreshold(axisRaw, 0.2f))
				{
					string text4 = text5;
					text5 = text4 + "A" + k + ": " + axisRaw.ToString("0.00") + "  ";
				}
			}
			GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text5, style);
			num3 += num4;
			num3 += 25;
		}
	}

	private void OnDrawGizmos()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		InputDevice activeDevice = InputManager.ActiveDevice;
		Vector2 vector = activeDevice.Direction.Vector;
		Gizmos.color = Color.blue;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(-3f, -1f);
		Vector2 val2 = val + vector * 2f;
		Gizmos.DrawSphere(Vector2.op_Implicit(val), 0.1f);
		Gizmos.DrawLine(Vector2.op_Implicit(val), Vector2.op_Implicit(val2));
		Gizmos.DrawSphere(Vector2.op_Implicit(val2), 1f);
		Gizmos.color = Color.red;
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(3f, -1f);
		Vector2 val4 = val3 + activeDevice.RightStick.Vector * 2f;
		Gizmos.DrawSphere(Vector2.op_Implicit(val3), 0.1f);
		Gizmos.DrawLine(Vector2.op_Implicit(val3), Vector2.op_Implicit(val4));
		Gizmos.DrawSphere(Vector2.op_Implicit(val4), 1f);
	}
}
