using TMG.Core;
using UnityEngine;

public class CharacterHeadController : TMGMonoBehaviour
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_Head;

	[Header("Options")]
	[SerializeField]
	private bool m_OnAwake = true;

	[Header("Clamp Options")]
	[SerializeField]
	private bool m_EnableClamp;

	[SerializeField]
	private Vector2 m_MinClamp;

	[SerializeField]
	private Vector2 m_MaxClamp;

	private bool m_CanLook;

	private Transform m_Target;

	public override void Init()
	{
		base.Init();
		m_CanLook = m_OnAwake;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Target = GameManager.Instance.Player.transform;
	}

	public void Activate()
	{
		m_CanLook = true;
	}

	private void LateUpdate()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)m_Target == (Object)null) && m_CanLook)
		{
			m_Head.LookAt(m_Target);
			Vector3 localEulerAngles = m_Head.localEulerAngles;
			localEulerAngles.z = 0f;
			if (m_EnableClamp)
			{
				localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, m_MinClamp.x, m_MaxClamp.x);
				localEulerAngles.y = Mathf.Clamp(localEulerAngles.y, m_MinClamp.y, m_MaxClamp.y);
			}
			m_Head.localEulerAngles = localEulerAngles;
		}
	}

	protected override void OnDisposed()
	{
		m_Target = null;
		base.OnDisposed();
	}
}
