using System;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class PoolBall : TMGMonoBehaviour
{
	private Rigidbody m_RigidBody;

	private Interactable m_Interactable;

	private S13ObjectSimple m_AudioSwitch;

	public override void Init()
	{
		base.Init();
		m_RigidBody = ((Component)this).GetComponent<Rigidbody>();
		m_Interactable = ((Component)this).GetComponent<Interactable>();
		m_Interactable.OnInteracted += HandleBallInteracted;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AudioSwitch = ((Component)this).GetComponentInChildren<S13ObjectSimple>();
	}

	public void HandleBallInteracted(object sender, EventArgs e)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		m_RigidBody.AddForce(GameManager.Instance.Player.transform.forward * 1200f);
		m_RigidBody.AddRelativeTorque(GameManager.Instance.Player.transform.forward);
		m_AudioSwitch.Play();
	}

	private void OnCollisionEnter(Collision collision)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (collision.gameObject.tag == "Pocket")
		{
			base.gameObject.SetActive(false);
		}
		else if (Object.op_Implicit((Object)(object)collision.gameObject.GetComponent<PoolBall>()))
		{
			Vector3 relativeVelocity = collision.relativeVelocity;
			if (((Vector3)(ref relativeVelocity)).magnitude > 15f)
			{
				m_AudioSwitch.Play();
			}
		}
	}
}
