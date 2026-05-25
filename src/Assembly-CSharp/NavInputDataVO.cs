using TMG.Core;

public class NavInputDataVO : TMGAbstractDisposable
{
	public string Input { get; private set; }

	public string Description { get; private set; }

	public static NavInputDataVO Create(string input, string description)
	{
		NavInputDataVO navInputDataVO = new NavInputDataVO();
		navInputDataVO.Input = input;
		navInputDataVO.Description = description;
		return navInputDataVO;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
