using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public struct S13AnimationAudioContainer
{
	public string id;

	[Tooltip("Name of the animation clip")]
	public string animationClipName;

	public ContainerFunction functionType;

	[Tooltip("The Game Object which contains the audiosource to play")]
	public GameObject audioSourceObject;

	[Tooltip("Which frame(s) of the animation clip to trigger the event on.")]
	[AudioSlider("Frame", 0, 10000)]
	public int[] frame;
}
