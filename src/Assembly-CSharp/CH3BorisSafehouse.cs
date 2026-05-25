using System;
using TMG.Core;
using UnityEngine;

public class CH3BorisSafehouse : TMGMonoBehaviour
{
	private const string CHAIR_GET_TOOLBOX = "Chair_GetToolbox";

	private const string CHAIR_GET_UP = "Chair_GetUp";

	private const string CHAIR_IDLE = "Chair_Idle";

	[Header("Transforms")]
	[SerializeField]
	private Transform m_Head;

	[SerializeField]
	private Vector3 m_HeadOffset;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private Transform m_Toolbox;

	[SerializeField]
	private Transform m_Hand;

	[SerializeField]
	private Transform m_Table;

	[SerializeField]
	private Transform m_GetUp;

	private Transform m_ToolboxParent;

	private Transform m_Target;

	private bool m_IsLooking;

	public event EventHandler OnToolboxPlaced;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ToolboxParent = m_Toolbox.parent;
		m_Animator.Play("Chair_Idle");
		m_Target = GameManager.Instance.Player.transform;
	}

	private void LateUpdate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)m_Target == (Object)null) && !(Vector3.Distance(m_Target.position, base.transform.position) > 10f))
		{
			m_Head.LookAt(m_Target);
			Transform head = m_Head;
			Quaternion val = (head.rotation *= Quaternion.Euler(m_HeadOffset));
			Quaternion val3 = val;
			m_Head.rotation = Quaternion.Lerp(m_Head.rotation, val3, 0.1f * Time.deltaTime);
		}
	}

	public void GetToolbox()
	{
		m_Animator.Play("Chair_GetToolbox");
		AnimationClip val = m_Animator.runtimeAnimatorController.animationClips[1];
		val.AddEvent(AddEvent("GrabToolbox", 2f / 3f));
		val.AddEvent(AddEvent("PlaceToolbox", (float)Math.PI * 113f / 150f));
	}

	private AnimationEvent AddEvent(string functionName, float time)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		AnimationEvent val = new AnimationEvent();
		val.functionName = functionName;
		val.time = time;
		val.objectReferenceParameter = (Object)(object)this;
		return val;
	}

	public void GrabToolbox()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		m_Toolbox.SetParent(m_Hand);
		m_Toolbox.localPosition = Vector3.zero;
		m_Toolbox.localEulerAngles = Vector3.zero;
	}

	public void PlaceToolbox()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		m_Toolbox.SetParent(m_ToolboxParent);
		m_Toolbox.position = m_Table.position;
		m_Toolbox.eulerAngles = m_Table.eulerAngles;
		this.OnToolboxPlaced.Send(this);
	}

	public void GetUp()
	{
		m_Animator.Play("Chair_GetUp");
		AnimationClip val = m_Animator.runtimeAnimatorController.animationClips[2];
		val.AddEvent(AddEvent("EnableBorisPathing", 2f));
	}

	private void EnableBorisPathing()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.transform.position = m_GetUp.position;
	}

	protected override void OnDisposed()
	{
		this.OnToolboxPlaced = null;
		base.OnDisposed();
	}
}
