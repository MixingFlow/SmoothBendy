using TMG.Core;

public class SubtitleDataVO : TMGAbstractDisposable
{
	public string Subtitles;

	public float Duration;

	public bool IsTrimmed;

	public static SubtitleDataVO Create(string subtitles, float duration, bool isTrimmed = false)
	{
		SubtitleDataVO subtitleDataVO = new SubtitleDataVO();
		subtitleDataVO.Subtitles = subtitles;
		subtitleDataVO.Duration = duration;
		subtitleDataVO.IsTrimmed = isTrimmed;
		return subtitleDataVO;
	}
}
