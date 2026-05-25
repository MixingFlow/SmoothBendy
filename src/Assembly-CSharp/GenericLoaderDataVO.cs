using TMG.Core;

public class GenericLoaderDataVO : TMGAbstractDisposable
{
	public string SceneName;

	public GenericLoaderDataVO(string sceneName)
	{
		SceneName = sceneName;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
