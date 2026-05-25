using TMG.Core;

public class SpriteLookupDataVO : TMGAbstractDisposable
{
	public string Lookup;

	public string Sprite;

	public SpriteLookupDataVO(string lookup, string sprite)
	{
		Lookup = lookup;
		Sprite = sprite;
	}

	public static SpriteLookupDataVO Create(string lookup, string sprite)
	{
		return new SpriteLookupDataVO(lookup, sprite);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
