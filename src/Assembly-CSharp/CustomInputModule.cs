using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInputModule : StandaloneInputModule
{
	private Vector2 m_cursorPos;

	private readonly MouseState m_MouseState = new MouseState();

	protected override MouseState GetMousePointerEventData(int id = 0)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		PointerEventData val = default(PointerEventData);
		bool pointerData = ((PointerInputModule)this).GetPointerData(-1, ref val, true);
		((AbstractEventData)val).Reset();
		if (pointerData)
		{
			val.position = Vector2.op_Implicit(Input.mousePosition);
		}
		Vector2 val2 = Vector2.op_Implicit(Input.mousePosition);
		val.delta = val2 - val.position;
		val.position = val2;
		val.scrollDelta = Input.mouseScrollDelta;
		val.button = (InputButton)0;
		((BaseInputModule)this).eventSystem.RaycastAll(val, ((BaseInputModule)this).m_RaycastResultCache);
		RaycastResult pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(((BaseInputModule)this).m_RaycastResultCache);
		val.pointerCurrentRaycast = pointerCurrentRaycast;
		((BaseInputModule)this).m_RaycastResultCache.Clear();
		PointerEventData val3 = default(PointerEventData);
		((PointerInputModule)this).GetPointerData(-2, ref val3, true);
		((PointerInputModule)this).CopyFromTo(val, val3);
		val3.button = (InputButton)1;
		PointerEventData val4 = default(PointerEventData);
		((PointerInputModule)this).GetPointerData(-3, ref val4, true);
		((PointerInputModule)this).CopyFromTo(val, val4);
		val4.button = (InputButton)2;
		m_MouseState.SetButtonState((InputButton)0, ((PointerInputModule)this).StateForMouseButton(0), val);
		m_MouseState.SetButtonState((InputButton)1, ((PointerInputModule)this).StateForMouseButton(1), val3);
		m_MouseState.SetButtonState((InputButton)2, ((PointerInputModule)this).StateForMouseButton(2), val4);
		return m_MouseState;
	}

	private void GetSelectState(PointerEventData data, InputButton inputButton, bool wasPressed, bool wasReleased)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		FramePressState val = (FramePressState)3;
		if (wasPressed)
		{
			val = (FramePressState)0;
		}
		else if (wasReleased)
		{
			val = (FramePressState)1;
		}
		m_MouseState.SetButtonState(inputButton, val, data);
	}
}
