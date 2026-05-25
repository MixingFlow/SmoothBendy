using System;
using System.Collections.Generic;

[Serializable]
public class CH3DataVO : ChapterDataVO
{
	public int InternecionValue;

	public int LiftFloor;

	public List<int> SqueakyToys = new List<int>();

	public int Toy;

	public bool ChoseDevilsPath;

	public bool HasTommyGun;

	public bool HasBorisBone;

	public bool IsSpeakeasyComplete;

	public bool IsHenryUnlocked;

	public bool UsedAxe;

	public bool IsProjectionistKilled;

	public ObjectiveSaveDataVO AccountingRoom = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO SafehouseObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO DarkHallwayObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO HeavenlyToysObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO AliceRevealObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO DecisionObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO BorisJumpscareObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO PosterPiperObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO EnterLiftObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO AliceLairObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO AliceTasksObjective = new ObjectiveSaveDataVO();

	public ObjectiveIDDataVO GearTask = new ObjectiveIDDataVO();

	public ObjectiveIDDataVO ThickInkTask = new ObjectiveIDDataVO();

	public ObjectiveIDDataVO PowerCoreTask = new ObjectiveIDDataVO();

	public ObjectiveIDDataVO CutoutTask = new ObjectiveIDDataVO();

	public ObjectiveSaveDataVO ButcherGangTask = new ObjectiveSaveDataVO();

	public ObjectiveIDDataVO HeartTask = new ObjectiveIDDataVO();

	public CH3DataVO()
	{
		SetObject(ref GearTask, 0, 1, 2);
		SetObject(ref ThickInkTask, 0, 1, 2);
		SetObject(ref PowerCoreTask, 0, 1, 2);
		SetObject(ref CutoutTask, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
		SetObject(ref HeartTask, 0, 1, 2, 3, 4);
	}

	private void SetObject(ref ObjectiveIDDataVO data, params int[] ids)
	{
		List<ObjectDataVO> list = new List<ObjectDataVO>();
		for (int i = 0; i < ids.Length; i++)
		{
			list.Add(new ObjectDataVO
			{
				ID = i,
				IsComplete = false
			});
		}
		data.Object = list.ToArray();
	}
}
