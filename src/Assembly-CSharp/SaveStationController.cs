using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveStationController : BaseController
{
	[SerializeField]
	private Chapters m_Chapter;

	[SerializeField]
	private List<SaveStation> m_SaveStations;

	public override void Init()
	{
		base.Init();
		SaveStation[] array = Resources.FindObjectsOfTypeAll<SaveStation>();
		foreach (SaveStation saveStation in array)
		{
			if (!m_SaveStations.Contains(saveStation) && saveStation.gameObject.activeInHierarchy)
			{
				m_SaveStations.Add(saveStation);
			}
		}
		for (int j = 0; j < m_SaveStations.Count; j++)
		{
			m_SaveStations[j].OnSaving += HandleSaveStationOnSaving;
		}
	}

	private void HandleSaveStationOnSaving(object sender, EventArgs e)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		ChapterDataVO chapterDataVO = null;
		if (m_Chapter == Chapters.ONE)
		{
			chapterDataVO = GameManager.Instance.GameData.CurrentSaveFile.CH1Data;
		}
		else if (m_Chapter == Chapters.TWO)
		{
			chapterDataVO = GameManager.Instance.GameData.CurrentSaveFile.CH2Data;
		}
		else if (m_Chapter == Chapters.THREE)
		{
			chapterDataVO = GameManager.Instance.GameData.CurrentSaveFile.CH3Data;
		}
		else if (m_Chapter == Chapters.FOUR)
		{
			chapterDataVO = GameManager.Instance.GameData.CurrentSaveFile.CH4Data;
		}
		else if (m_Chapter == Chapters.FIVE)
		{
			chapterDataVO = GameManager.Instance.GameData.CurrentSaveFile.CH5Data;
		}
		chapterDataVO.PlayerPosition = new Vector3DataVO(GameManager.Instance.Player.transform.position);
		chapterDataVO.PlayerRotation = new Vector3DataVO(GameManager.Instance.Player.transform.eulerAngles);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
