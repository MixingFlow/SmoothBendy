using System;

[Serializable]
public class PoolInstance
{
	public int id;

	public string title;

	public int[] limits;

	public PoolInstance(string Title, PoolInstance[] CurrentInstances)
	{
		id = UniqueID(CurrentInstances);
		title = Title;
		limits = new int[15];
		for (int i = 0; i < limits.Length; i++)
		{
			limits[i] = 250 + i * 50;
		}
	}

	private int UniqueID(PoolInstance[] CurrentInstances)
	{
		int num = 0;
		bool flag = false;
		if (CurrentInstances != null)
		{
			while (!flag)
			{
				num++;
				flag = true;
				for (int i = 0; i < CurrentInstances.Length; i++)
				{
					if (CurrentInstances[i] != null && num == CurrentInstances[i].id)
					{
						flag = false;
					}
				}
			}
		}
		return num;
	}
}
