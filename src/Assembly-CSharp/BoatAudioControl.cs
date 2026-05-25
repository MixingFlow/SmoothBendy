using System.Collections;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class BoatAudioControl : TMGMonoBehaviour
{
	[SerializeField]
	private S13Switch m_boatSwitch;

	[Space]
	[Header("EngineOn")]
	public float engineStartPlayDelay = 0.3f;

	public float engineIdlePlayDelay = 4.1f;

	[Space]
	[Header("EngineOff")]
	public float engineStopPlayDelay = 0.3f;

	public float engineWorkStopDelay = 0.6f;

	public float engineIdleStopDelay = 0.6f;

	[Space]
	[Header("ThrottleOn")]
	public float engineWorkOnPlayDelay = 0.3f;

	public float engineWorkLoopPlayDelay = 1.9f;

	public float engineIdleLoopStopDelay = 0.6f;

	[Space]
	[Header("ThrottleOff")]
	public float engineWorkOffPlayDelay = 0.3f;

	public float engineIdleLoopPlayDelay = 1.4f;

	public float engineWorkLoopStopDelay = 0.6f;

	private IEnumerator m_delayCoroutine;

	public override void Init()
	{
		if ((Object)(object)m_boatSwitch == (Object)null)
		{
			m_boatSwitch = ((Component)this).GetComponent<S13Switch>();
		}
	}

	public override void InitOnComplete()
	{
		if ((Object)(object)m_boatSwitch == (Object)null)
		{
			Debug.LogError((object)"S13Switch not found by BoatAudioControl");
		}
	}

	public void EngineOn()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		m_boatSwitch.Stop("engine_idle_loop");
		m_boatSwitch.Stop("engine_stop");
		m_boatSwitch.Play("control_start");
		PlayAudioDelayed("engine_start", engineStartPlayDelay);
		PlayAudioDelayed("engine_idle_loop", engineIdlePlayDelay);
	}

	public void EngineOff()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		m_boatSwitch.Stop("engine_start");
		m_boatSwitch.Play("control_stop");
		PlayAudioDelayed("engine_stop", engineStopPlayDelay);
		StopAudioDelayed("engine_work_loop", engineWorkStopDelay);
		StopAudioDelayed("engine_idle_loop", engineIdleStopDelay);
	}

	public void ThrottleOn()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		m_boatSwitch.Stop("engine_work_off");
		m_boatSwitch.Play("control_forward");
		PlayAudioDelayed("engine_work_on", engineWorkOnPlayDelay);
		StopAudioDelayed("engine_idle_loop", engineIdleLoopStopDelay);
		PlayAudioDelayed("engine_work_loop", engineWorkLoopPlayDelay);
	}

	public void ThrottleOff()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		m_boatSwitch.Play("control_back");
		PlayAudioDelayed("engine_work_off", engineWorkOffPlayDelay);
		StopAudioDelayed("engine_work_loop", engineWorkLoopStopDelay);
		PlayAudioDelayed("engine_idle_loop", engineIdleLoopPlayDelay);
	}

	private void PlayAudioDelayed(string soundId, float delayTime)
	{
		m_delayCoroutine = S13AudioUtil.WaitForDuration(delayTime, ignoreTimeScale: true, delegate
		{
			m_boatSwitch.Play(soundId);
		});
		((MonoBehaviour)this).StartCoroutine(m_delayCoroutine);
	}

	private void StopAudioDelayed(string soundId, float delayTime)
	{
		m_delayCoroutine = S13AudioUtil.WaitForDuration(delayTime, ignoreTimeScale: true, delegate
		{
			m_boatSwitch.Stop(soundId);
		});
		((MonoBehaviour)this).StartCoroutine(m_delayCoroutine);
	}

	protected override void OnDisposed()
	{
		m_boatSwitch = null;
		m_delayCoroutine = null;
	}
}
