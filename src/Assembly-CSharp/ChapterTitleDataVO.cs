using I2.Loc;
using TMG.Core;

public class ChapterTitleDataVO : TMGAbstractDisposable
{
	public string Chapter;

	public string Title;

	public bool ShowBlocker;

	public static ChapterTitleDataVO Create(string chapter, string title, bool showBlocker = true)
	{
		string Translation = chapter;
		if (LocalizationManager.TryGetTranslation(chapter, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			chapter = Translation;
		}
		Translation = title;
		if (LocalizationManager.TryGetTranslation(title, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			title = Translation;
		}
		ChapterTitleDataVO chapterTitleDataVO = new ChapterTitleDataVO();
		chapterTitleDataVO.Chapter = chapter;
		chapterTitleDataVO.Title = title;
		chapterTitleDataVO.ShowBlocker = showBlocker;
		return chapterTitleDataVO;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
