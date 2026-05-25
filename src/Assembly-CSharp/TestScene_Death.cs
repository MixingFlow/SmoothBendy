using TMG.Core;

public class TestScene_Death : TMGMonoBehaviour
{
	public DeathController DeathController;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		DeathController.Activate();
	}
}
