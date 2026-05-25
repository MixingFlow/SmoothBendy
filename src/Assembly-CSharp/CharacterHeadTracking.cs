using TMG.Core;
using UnityEngine;

public class CharacterHeadTracking : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Head;

	[SerializeField]
	private Transform m_HeadLookForward;

	[SerializeField]
	private Vector3 m_HeadOffset = new Vector3(0f, -90f, -90f);

	[SerializeField]
	private float m_VisionDistance = 5f;

	[SerializeField]
	private float m_VisionCone = 70f;

	private Transform m_Target;

	private Quaternion m_LastLookRotation;

	public override void Init()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_LastLookRotation = m_Head.rotation;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Target = GameManager.Instance.GameCamera.transform;
	}

	private void LateUpdate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)m_Target == (Object)null))
		{
			Vector3 position = base.transform.position;
			position.y = m_Target.position.y;
			float num = Vector3.Angle(m_HeadLookForward.forward, m_Target.position - position);
			if (Vector3.Distance(m_Target.position, position) < m_VisionDistance && num < m_VisionCone)
			{
				Vector3 val = m_Target.position - m_Head.position;
				Quaternion val2 = Quaternion.LookRotation(val) * Quaternion.Euler(m_HeadOffset);
				m_Head.rotation = Quaternion.Slerp(m_LastLookRotation, val2, 2f * Time.deltaTime);
			}
			else
			{
				Quaternion val3 = Quaternion.LookRotation(m_HeadLookForward.forward) * Quaternion.Euler(m_HeadOffset);
				m_Head.rotation = Quaternion.Slerp(m_LastLookRotation, val3, 2f * Time.deltaTime);
			}
			m_LastLookRotation = m_Head.rotation;
		}
	}
}
