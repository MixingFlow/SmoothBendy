using InControl;
using TMG.GamepadControl;
using UnityEngine;

namespace TMG.Controls;

public class PlayerInput : BasePlayerInput
{
	public static bool Any()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.Action1) || GamepadInput.GetButtonDown(InputControlType.Action2) || GamepadInput.GetButtonDown(InputControlType.Action3) || GamepadInput.GetButtonDown(InputControlType.Action4), Input.anyKeyDown);
	}

	public static bool Attack()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.RightTrigger), Input.GetMouseButtonDown(0));
	}

	public static bool SeeingTool()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.LeftTrigger), Input.GetMouseButtonDown(1));
	}

	public static bool AttackHold()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButton(InputControlType.RightTrigger), Input.GetMouseButton(0));
	}

	public static bool Run()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButton(InputControlType.LeftStickButton) || GamepadInput.GetButton(InputControlType.LeftBumper), Input.GetKey((KeyCode)304));
	}

	public static bool RunDown()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.LeftStickButton) || GamepadInput.GetButtonDown(InputControlType.LeftBumper), Input.GetKeyDown((KeyCode)304));
	}

	public static bool Jump()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.Action1), Input.GetKeyDown((KeyCode)32));
	}

	public static bool ExpoInvert()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.Action4), Input.GetKeyDown((KeyCode)104));
	}

	public static bool Pause()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.Start) || GamepadInput.GetButtonDown(InputControlType.Menu), Input.GetKeyDown((KeyCode)27));
	}

	public static float MoveX()
	{
		return BasePlayerInput.GetInputFloat(GamepadInput.GetAxis(InputControlType.LeftStickX), Input.GetAxis("Horizontal"));
	}

	public static float MoveY()
	{
		return BasePlayerInput.GetInputFloat(GamepadInput.GetAxis(InputControlType.LeftStickY), Input.GetAxis("Vertical"));
	}

	public static float LookX(float TSpeed = 0f)
	{
		return BasePlayerInput.GetInputFloat(GamepadInput.GetAxis(InputControlType.RightStickX) * (1.25f + TSpeed), Input.GetAxis("Mouse X"));
	}

	public static float LookY(float TSpeed = 0f)
	{
		float num = ((!GameManager.Instance.PlayerSettings.Inverted) ? 1 : (-1));
		return BasePlayerInput.GetInputFloat(GamepadInput.GetAxis(InputControlType.RightStickY) * (0.75f + TSpeed), Input.GetAxis("Mouse Y")) * num;
	}

	public static bool BackOnPressed()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.Action2), Input.GetKeyDown((KeyCode)113));
	}

	public static bool InteractOnPressed()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonDown(InputControlType.Action3), Input.GetKeyDown((KeyCode)101));
	}

	public static bool InteractOnReleased()
	{
		return BasePlayerInput.GetInputBool(GamepadInput.GetButtonUp(InputControlType.Action3), Input.GetKeyUp((KeyCode)101));
	}
}
