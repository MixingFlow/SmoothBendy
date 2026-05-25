using TMG.Core;

public class CreditsDataVO : TMGAbstractDisposable
{
	public bool IsChapterTwo;

	public CreditsDataVO(bool isChapterTwo)
	{
		IsChapterTwo = isChapterTwo;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
