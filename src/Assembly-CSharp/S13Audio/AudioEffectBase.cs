using UnityEngine;

namespace S13Audio;

public abstract class AudioEffectBase : MonoBehaviour
{
	[AudioSlider("Cutoff Frequency (Hz)", 10f, 22000f)]
	public float cutoffFrequency = 5000f;

	[AudioSlider("Resonance", 1f, 10f)]
	public float resonance = 1f;

	public abstract void Attach<T>(T unityFilter);

	public abstract void UpdateParameters();
}
