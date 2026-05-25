using UnityEngine;

namespace InControl;

public class UnityKeyCodeAxisSource : InputControlSource
{
	public KeyCode NegativeKeyCode;

	public KeyCode PositiveKeyCode;

	public UnityKeyCodeAxisSource()
	{
	}

	public UnityKeyCodeAxisSource(KeyCode negativeKeyCode, KeyCode positiveKeyCode)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		NegativeKeyCode = negativeKeyCode;
		PositiveKeyCode = positiveKeyCode;
	}

	public float GetValue(InputDevice inputDevice)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (Input.GetKey(NegativeKeyCode))
		{
			num--;
		}
		if (Input.GetKey(PositiveKeyCode))
		{
			num++;
		}
		return num;
	}

	public bool GetState(InputDevice inputDevice)
	{
		return Utility.IsNotZero(GetValue(inputDevice));
	}
}
