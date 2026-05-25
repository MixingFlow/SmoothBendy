using TMG.Core;
using UnityEngine;

public class CollectableDataVO : TMGAbstractDisposable
{
	public AudioClip AudioClip;

	public string AudioClipString;

	public SpriteLookupDataVO SpriteLookup;

	public CollectableDataVO(string audioClip, string lookup, string sprite)
	{
		AudioClipString = audioClip;
		SpriteLookup = SpriteLookupDataVO.Create(lookup, sprite);
	}

	public CollectableDataVO(AudioClip audioClip, string lookup, string sprite)
	{
		AudioClip = audioClip;
		SpriteLookup = SpriteLookupDataVO.Create(lookup, sprite);
	}

	public static CollectableDataVO Create(string audioClip, string lookup, string sprite)
	{
		return new CollectableDataVO(audioClip, lookup, sprite);
	}

	public static CollectableDataVO Create(AudioClip audioClip, string lookup, string sprite)
	{
		return new CollectableDataVO(audioClip, lookup, sprite);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
