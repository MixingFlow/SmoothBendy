using UnityEngine;

public class ExampleWheelController : MonoBehaviour
{
	private static class Uniforms
	{
		internal static readonly int _MotionAmount = Shader.PropertyToID("_MotionAmount");
	}

	public float acceleration;

	public Renderer motionVectorRenderer;

	private Rigidbody m_Rigidbody;

	private void Start()
	{
		m_Rigidbody = ((Component)this).GetComponent<Rigidbody>();
		m_Rigidbody.maxAngularVelocity = 100f;
	}

	private void Update()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKey((KeyCode)273))
		{
			m_Rigidbody.AddRelativeTorque(new Vector3(-1f * acceleration, 0f, 0f), (ForceMode)5);
		}
		else if (Input.GetKey((KeyCode)274))
		{
			m_Rigidbody.AddRelativeTorque(new Vector3(1f * acceleration, 0f, 0f), (ForceMode)5);
		}
		float num = (0f - m_Rigidbody.angularVelocity.x) / 100f;
		if (Object.op_Implicit((Object)(object)motionVectorRenderer))
		{
			motionVectorRenderer.material.SetFloat(Uniforms._MotionAmount, Mathf.Clamp(num, -0.25f, 0.25f));
		}
	}
}
