public interface ISavable
{
	GameObjectDataVO Save();

	void Load(GameObjectDataVO dataVO);
}
