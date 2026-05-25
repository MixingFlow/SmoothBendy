using System.Collections.Generic;
using I2.Loc;
using TMG.Core;
using UnityEngine;

public class ObjectiveDataVO : TMGAbstractDisposable
{
	public LocalizedString Header;

	public LocalizedString Objective;

	public LocalizedString Tip;

	public float Delay;

	public float PlayInDelay;

	public bool IsCurrentObjective;

	public int ItemCounter;

	public Sprite Item;

	public List<Sprite> Items;

	public List<bool> Collected;

	public static ObjectiveDataVO Create(string header, string objective, string tip, float delay = 0f, bool isCurrentObjective = false, float playInDelay = 0f)
	{
		ObjectiveDataVO objectiveDataVO = new ObjectiveDataVO();
		objectiveDataVO.Header = header;
		objectiveDataVO.Objective = objective;
		objectiveDataVO.Tip = tip;
		objectiveDataVO.Delay = delay;
		objectiveDataVO.PlayInDelay = playInDelay;
		objectiveDataVO.IsCurrentObjective = isCurrentObjective;
		return objectiveDataVO;
	}

	public void AddItems(List<Sprite> items)
	{
		Items = items;
		Collected = new List<bool>();
		for (int i = 0; i < Items.Count; i++)
		{
			Collected.Add(item: false);
		}
	}

	public void AddItemCounter(Sprite item, int itemCounter)
	{
		Item = item;
		ItemCounter = itemCounter;
	}
}
