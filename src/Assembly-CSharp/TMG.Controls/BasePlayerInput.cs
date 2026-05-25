using InControl;
using TMG.GamepadControl;
using UnityEngine;

namespace TMG.Controls;

public class BasePlayerInput
{
	public static bool VirtualMouseLeftOnPressed()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action1), Input.GetMouseButtonDown(0));
	}

	public static bool VirtualMouseLeftOnReleased()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action1), Input.GetMouseButtonUp(0));
	}

	public static bool VirtualMouseRightOnPressed()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action2), Input.GetMouseButtonDown(1));
	}

	public static bool VirtualMouseRightOnReleased()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action2), Input.GetMouseButtonUp(1));
	}

	public static bool VirtualMouseMiddleOnPressed()
	{
		return GetInput(GamepadInput.GetButtonDown(InputControlType.Action3), Input.GetMouseButtonDown(2));
	}

	public static bool VirtualMouseMiddleOnReleased()
	{
		return GetInput(GamepadInput.GetButtonUp(InputControlType.Action3), Input.GetMouseButtonUp(2));
	}

	protected static bool GetInputBool(bool inputGamepad, bool inputKeyboard)
	{
		if (inputGamepad)
		{
			return inputGamepad;
		}
		if (inputKeyboard)
		{
			return inputKeyboard;
		}
		return false;
	}

	protected static float GetInputFloat(float inputGamepad, float inputKeyboard)
	{
		if (Mathf.Abs(inputGamepad) > float.Epsilon)
		{
			return inputGamepad;
		}
		if (Mathf.Abs(inputKeyboard) > float.Epsilon)
		{
			return inputKeyboard;
		}
		return 0f;
	}

	protected static T GetInput<T>(T inputGamepad, T inputKeyboard)
	{
		T result = default(T);
		if (!inputGamepad.Equals(default(T)))
		{
			result = inputGamepad;
		}
		else if (!inputKeyboard.Equals(default(T)))
		{
			result = inputKeyboard;
		}
		return result;
	}
}
