using System;

[Serializable]
public class CH4DataVO : ChapterDataVO
{
	public bool HasPlunger;

	public int[] InternecionValues = new int[6] { 1, 2, 3, 4, 5, 6 };

	public int[] InternecionBools = new int[6];

	public ObjectiveSaveDataVO AccountingObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO BridgeMachineObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO LostOnesObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO VentObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO MapRoomObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO WarehouseObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO FairGamesObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO ResearchObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO RideStorageObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO MaintenanceObjective = new ObjectiveSaveDataVO();

	public ObjectiveSaveDataVO HauntedHouseObjective = new ObjectiveSaveDataVO();
}
