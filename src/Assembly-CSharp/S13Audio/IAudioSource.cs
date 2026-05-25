using UnityEngine;

namespace S13Audio;

public interface IAudioSource
{
	float Length { get; }

	int AudioSourceCount { get; }

	bool IsPlaying { get; }

	void Play();

	void Play(float duration);

	void PlayDelayed(float delayTime);

	void Stop(bool ignoreFade = false);

	void StopDelayed(float delayTime, bool ignoreFade = false);

	void Pause(bool ignoreFade = false);

	void Resume();

	void SetMuting(bool muting);

	void UpdateParameters();

	AudioSource UnitySource(int index);
}
