using InControl;
using UnityEngine;

public static class InputUtil
{
	public static bool GetInputY(out float axis)
	{
		bool hasMouse = false;
		return GetInputY(out axis, out hasMouse);
	}

	public static bool GetInputY(out float axis, out bool hasMouse)
	{
		bool result = false;
		hasMouse = false;
		if (GameManager.Instance.HasController)
		{
			float axis2 = Input.GetAxis("Mouse Y");
			float value = InputManager.ActiveDevice.LeftStickY.Value;
			float value2 = InputManager.ActiveDevice.DPadY.Value;
			if (axis2 != 0f)
			{
				Cursor.lockState = (CursorLockMode)0;
				Cursor.visible = true;
				hasMouse = true;
				result = false;
				axis = 0f;
			}
			else if (value != 0f)
			{
				Cursor.lockState = (CursorLockMode)1;
				Cursor.visible = false;
				hasMouse = false;
				result = true;
				axis = value;
			}
			else if (value2 != 0f)
			{
				Cursor.lockState = (CursorLockMode)1;
				Cursor.visible = false;
				hasMouse = false;
				result = true;
				axis = value2;
			}
			else
			{
				axis = 0f;
			}
		}
		else
		{
			float axis3 = Input.GetAxis("Mouse Y");
			if (axis3 != 0f)
			{
				Cursor.lockState = (CursorLockMode)0;
				Cursor.visible = true;
				hasMouse = true;
				result = false;
			}
			axis = 0f;
		}
		return result;
	}

	public static bool GetInputX(out float axis)
	{
		bool hasMouse = false;
		return GetInputX(out axis, out hasMouse);
	}

	public static bool GetInputX(out float axis, out bool hasMouse)
	{
		bool result = false;
		hasMouse = false;
		if (GameManager.Instance.HasController)
		{
			float axis2 = Input.GetAxis("Mouse X");
			float value = InputManager.ActiveDevice.LeftStickX.Value;
			float value2 = InputManager.ActiveDevice.DPadX.Value;
			if (axis2 != 0f)
			{
				Cursor.lockState = (CursorLockMode)0;
				Cursor.visible = true;
				hasMouse = true;
				result = false;
				axis = 0f;
			}
			else if (value != 0f)
			{
				Cursor.lockState = (CursorLockMode)1;
				Cursor.visible = false;
				hasMouse = false;
				result = true;
				axis = value;
			}
			else if (value2 != 0f)
			{
				Cursor.lockState = (CursorLockMode)1;
				Cursor.visible = false;
				hasMouse = false;
				result = true;
				axis = value2;
			}
			else
			{
				axis = 0f;
			}
		}
		else
		{
			float axis3 = Input.GetAxis("Mouse X");
			if (axis3 != 0f)
			{
				Cursor.lockState = (CursorLockMode)0;
				Cursor.visible = true;
				hasMouse = true;
				result = false;
			}
			axis = 0f;
		}
		return result;
	}
}
