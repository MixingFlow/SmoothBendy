using I2.Loc;
using TMG.Core;

public class TutorialDataVO : TMGAbstractDisposable
{
	public string Action;

	public string ActionRaw;

	public TutorialDataVO(string actionLbl)
	{
		ActionRaw = actionLbl;
		string Translation = string.Empty;
		if (LocalizationManager.TryGetTranslation(actionLbl, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			actionLbl = Translation;
		}
		Action = actionLbl;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
