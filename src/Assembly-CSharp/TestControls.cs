using S13Audio;
using UnityEngine;

[RequireComponent(typeof(S13AudioSource))]
public class TestControls : MonoBehaviour
{
	public Vector2 controlsPosition;

	private S13AudioSource _testComponent;

	private Rect _controlsRect;

	private Rect _buttonRect;

	private void Awake()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		_testComponent = ((Component)this).GetComponent<S13AudioSource>();
		_controlsRect = new Rect(controlsPosition.x, controlsPosition.y, 200f, 300f);
		_buttonRect = new Rect(0f, 20f, 180f, 24f);
	}

	private void OnGUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		GUI.BeginGroup(_controlsRect, "Test Controls");
		((Rect)(ref _buttonRect)).y = 20f;
		if (GUI.Button(_buttonRect, "Play"))
		{
			_testComponent.Play();
		}
		ref Rect buttonRect = ref _buttonRect;
		((Rect)(ref buttonRect)).y = ((Rect)(ref buttonRect)).y + ((Rect)(ref _buttonRect)).height;
		if (GUI.Button(_buttonRect, "Stop"))
		{
			_testComponent.Stop();
		}
		GUI.EndGroup();
	}
}
