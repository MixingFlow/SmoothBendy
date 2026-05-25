using System;

namespace TMG.Data;

[Serializable]
public class GameData
{
	private const int SAVE_FILE_COUNT = 3;

	public SaveFileData[] SaveFiles = new SaveFileData[3];

	public SaveFileData CurrentSaveFile;

	public SaveFileData NoSaveFile;

	public AchievementSaveData CH1AchievementData = new AchievementSaveData();

	public AchievementSaveData CH2AchievementData = new AchievementSaveData();

	public AchievementSaveData CH3AchievementData = new AchievementSaveData();

	public AchievementSaveData CH4AchievementData = new AchievementSaveData();

	public AchievementSaveData CH5AchievementData = new AchievementSaveData();

	public int ContinueIndex;

	public bool HasChapter02;

	public bool HasChapter03;

	public bool HasChapter04;

	public bool HasChapter05;
}
