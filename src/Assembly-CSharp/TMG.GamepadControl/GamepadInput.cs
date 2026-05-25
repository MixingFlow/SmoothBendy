using InControl;

namespace TMG.GamepadControl;

public static class GamepadInput
{
	public static bool GetButton(InputControlType inputControlType)
	{
		InputControl control = GetControl(inputControlType);
		bool result = false;
		if (control != null)
		{
			result = control.IsPressed;
		}
		return result;
	}

	public static bool GetButtonDown(InputControlType inputControlType)
	{
		InputControl control = GetControl(inputControlType);
		bool result = false;
		if (control != null)
		{
			result = control.WasPressed;
		}
		return result;
	}

	public static bool GetButtonUp(InputControlType inputControlType)
	{
		InputControl control = GetControl(inputControlType);
		bool result = false;
		if (control != null)
		{
			result = control.WasReleased;
		}
		return result;
	}

	public static float GetAxis(InputControlType inputControlType)
	{
		InputControl control = GetControl(inputControlType);
		float result = 0f;
		if (control != null)
		{
			result = control.Value;
		}
		return result;
	}

	public static float GetAxisRaw(InputControlType inputControlType)
	{
		InputControl control = GetControl(inputControlType);
		float result = 0f;
		if (control != null)
		{
			result = control.RawValue;
		}
		return result;
	}

	private static InputControl GetControl(InputControlType inputControlType)
	{
		return InputManager.ActiveDevice?.GetControl(inputControlType);
	}
}
