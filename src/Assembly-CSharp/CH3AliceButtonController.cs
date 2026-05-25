using System;
using TMG.Core;
using UnityEngine;

public class CH3AliceButtonController : TMGMonoBehaviour
{
	[SerializeField]
	private Animator m_AnimationController;

	public event EventHandler OnPressed;

	public void Activate()
	{
		m_AnimationController.SetTrigger("Activate");
		AnimationClip val = null;
		for (int i = 0; i < m_AnimationController.runtimeAnimatorController.animationClips.Length; i++)
		{
			if (((Object)m_AnimationController.runtimeAnimatorController.animationClips[i]).name.Contains("monologue"))
			{
				val = m_AnimationController.runtimeAnimatorController.animationClips[i];
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			val.AddEvent(AddEvent("PressButton", 97.13333f));
		}
	}

	public void PressButton()
	{
		this.OnPressed.Send(this);
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

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
