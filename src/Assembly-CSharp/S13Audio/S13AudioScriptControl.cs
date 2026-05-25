using UnityEngine;

namespace S13Audio;

public class S13AudioScriptControl : S13AudioSource
{
	public S13AudioSource startScript;

	public S13AudioSource[] loopScripts;

	public S13AudioSource endScript;

	[AudioSlider("Pitch min (semitones)", -12f, 12f)]
	public float pitchMin;

	[AudioSlider("Pitch max (semitones)", -12f, 12f)]
	public float pitchMax;

	private int _randomLoop;

	public override float Length => -1f;

	private void Awake()
	{
		_audioSources = (AudioSource[])(object)new AudioSource[0];
	}

	public override void Play()
	{
		startScript.UnitySource(0).pitch = S13AudioUtil.SemitoneToPitch(Random.Range(pitchMin, pitchMax));
		startScript.Play();
		_randomLoop = Random.Range(0, loopScripts.Length);
		S13AudioSource s13AudioSource = loopScripts[_randomLoop];
		s13AudioSource.UnitySource(0).pitch = S13AudioUtil.SemitoneToPitch(Random.Range(pitchMin, pitchMax));
		s13AudioSource.Play();
	}

	public override void Stop(bool ignoreFade)
	{
		loopScripts[_randomLoop].Stop(ignoreFade);
		endScript.UnitySource(0).pitch = S13AudioUtil.SemitoneToPitch(Random.Range(pitchMin, pitchMax));
		endScript.audioEndedHandler = delegate
		{
			if (audioEndedHandler != null)
			{
				audioEndedHandler(this);
			}
		};
		endScript.Play();
	}

	public override void Pause(bool ignoreFade)
	{
	}

	public override void Resume()
	{
	}
}
