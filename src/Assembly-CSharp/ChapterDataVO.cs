using System;

[Serializable]
public class ChapterDataVO
{
	public Vector3DataVO PlayerPosition;

	public Vector3DataVO PlayerRotation;

	public bool HasDied;

	public bool IsChapterComplete;

	public bool HasSaveData;
}
