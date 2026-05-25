using I2.Loc;
using TMG.Core;
using UnityEngine;

public class DialogueDataVO : TMGAbstractDisposable
{
	public string Dialogue;

	public AudioClip DialogueClip;

	public SubtitleDataVO Subtitles;

	public static DialogueDataVO Create(string dialogue, string subtitles, bool isTrimmed = false)
	{
		string Translation = subtitles;
		if (LocalizationManager.TryGetTranslation(subtitles, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			subtitles = Translation;
		}
		SubtitleDataVO subtitles2 = SubtitleDataVO.Create(subtitles, 0f, isTrimmed);
		DialogueDataVO dialogueDataVO = new DialogueDataVO();
		dialogueDataVO.Dialogue = dialogue;
		dialogueDataVO.Subtitles = subtitles2;
		return dialogueDataVO;
	}

	public static DialogueDataVO Create(AudioClip dialogue, string subtitles, bool isTrimmed = false)
	{
		string Translation = subtitles;
		if (LocalizationManager.TryGetTranslation(subtitles, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			subtitles = Translation;
		}
		SubtitleDataVO subtitles2 = SubtitleDataVO.Create(subtitles, 0f, isTrimmed);
		DialogueDataVO dialogueDataVO = new DialogueDataVO();
		dialogueDataVO.DialogueClip = dialogue;
		dialogueDataVO.Subtitles = subtitles2;
		return dialogueDataVO;
	}

	protected override void OnDisposed()
	{
		Subtitles.Dispose();
		Subtitles = null;
		base.OnDisposed();
	}
}
