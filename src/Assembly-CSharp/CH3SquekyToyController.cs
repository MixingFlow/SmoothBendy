using System;
using System.Collections.Generic;
using UnityEngine;

public class CH3SquekyToyController : BaseController
{
	[SerializeField]
	private List<SqueakToy> m_SqueakyToys;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_SqueakyToys.Count; i++)
		{
			SqueakToy squeakToy = m_SqueakyToys[i];
			squeakToy.OnInteracted += HandleSqueakyToyOnInteracted;
		}
	}

	private void HandleSqueakyToyOnInteracted(object sender, EventArgs e)
	{
		SqueakToy squeakToy = (SqueakToy)sender;
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.SqueakyToys.Contains(squeakToy.GetID()))
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.SqueakyToys.Add(squeakToy.GetID());
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.SqueakyToys.Count >= 25)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.INNER_CHILD);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
