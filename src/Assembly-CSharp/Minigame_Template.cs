public class Minigame_Template : WinnableMiniGameBaseController
{
	public override void Init()
	{
		base.Init();
	}

	public override void HandleGameStartupSequence()
	{
		base.HandleGameStartupSequence();
		HandleOnPrepHeldObject();
	}

	public override void BeginGameLoops()
	{
		base.BeginGameLoops();
	}

	public override void Update()
	{
		base.Update();
		if (base.CurrentState != MiniGameState.ACTIVE)
		{
		}
	}
}
