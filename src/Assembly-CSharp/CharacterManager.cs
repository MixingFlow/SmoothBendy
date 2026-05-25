using Ai;
using TMG.Core;

public class CharacterManager : TMGAbstractDisposable
{
	public BendyAi Bendy;

	public BorisAi Boris;

	public PlayerController Player;

	public BruteBorisAi BruteBoris;

	protected override void OnDisposed()
	{
		Bendy = null;
		Boris = null;
		Player = null;
		BruteBoris = null;
		base.OnDisposed();
	}
}
