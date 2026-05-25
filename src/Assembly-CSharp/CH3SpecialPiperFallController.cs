using TMG.Core;
using UnityEngine;

public class CH3SpecialPiperFallController : TMGMonoBehaviour
{
	[SerializeField]
	private Animator m_AnimationController;

	private AudioClip m_PiperFall;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_PiperFall = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_piperfalloutofposter");
		Activate();
	}

	public void Activate()
	{
		AnimationClip val = null;
		for (int i = 0; i < m_AnimationController.runtimeAnimatorController.animationClips.Length; i++)
		{
			if (((Object)m_AnimationController.runtimeAnimatorController.animationClips[i]).name.Contains("jumpscare"))
			{
				val = m_AnimationController.runtimeAnimatorController.animationClips[i];
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			val.AddEvent(AddEvent("Fall", 2.8333333f));
		}
	}

	public void Fall()
	{
		GameManager.Instance.AudioManager.Play(m_PiperFall);
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
		m_PiperFall = null;
		base.OnDisposed();
	}
}
