using System;

[Serializable]
public class CH2DataVO : ChapterDataVO
{
	public int InternecionValue;

	public ObjectiveSaveDataVO RitualObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO GateObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO MusicDepartmentObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO LostKeysObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO MusicPuzzleObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO SanctuaryObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO InfirmaryObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO SewersObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO SammysOfficeObjective = new ObjectiveSaveDataVO();
}
