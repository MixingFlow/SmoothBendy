using TMG.Core;

public class AttentionDataVO : TMGAbstractDisposable
{
	public string Header;

	public string Message;

	public AttentionDataVO(string header, string message)
	{
		Header = header;
		Message = message;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
