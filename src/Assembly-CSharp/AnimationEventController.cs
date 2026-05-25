using Ai;
using TMG.Core;

public class AnimationEventController : TMGMonoBehaviour
{
	public BaseAiController BaseAi { get; private set; }

	public SwollenSearcherAi swollenAI { get; private set; }

	public CH5BendyHandChase BendyHand { get; private set; }

	public void SetReference(SwollenSearcherAi reference)
	{
		swollenAI = reference;
	}

	public void SetReference(BaseAiController reference)
	{
		BaseAi = reference;
	}

	public void SetReference(CH5BendyHandChase reference)
	{
		BendyHand = reference;
	}

	public void Init(BaseAiController baseAi)
	{
		BaseAi = baseAi;
	}

	public void AttackTarget()
	{
		BaseAi.AttackTarget();
	}

	public void ResetBorisMovement()
	{
		GameManager.Instance.CharacterManager.Boris.ResetMovement();
	}

	public void SwollenSearcherHide()
	{
		swollenAI.HideOnComplete();
	}

	public void BendyHandSlap()
	{
		BendyHand.CheckForAndKillPlayer();
	}

	public void BendyHandSlapOnComplete()
	{
		BendyHand.BendyHandSlapOnComplete();
	}
}
