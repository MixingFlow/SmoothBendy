using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class SaveStationManager : TMGAbstractDisposable
{
	public List<SaveStation> SaveStations { get; private set; }

	public void Initialize()
	{
		ClearSaveStations();
		SaveStation[] array = Object.FindObjectsOfType<SaveStation>();
		for (int i = 0; i < array.Length; i++)
		{
			SaveStations.Add(array[i]);
		}
	}

	public void DisableSaving()
	{
		for (int i = 0; i < SaveStations.Count; i++)
		{
			SaveStations[i].DisableSaving();
		}
	}

	public void EnableSaving()
	{
		for (int i = 0; i < SaveStations.Count; i++)
		{
			SaveStations[i].EnableSaving();
		}
	}

	private void ClearSaveStations()
	{
		if (SaveStations != null)
		{
			SaveStations.Clear();
			SaveStations = null;
		}
		SaveStations = new List<SaveStation>();
	}

	protected override void OnDisposed()
	{
		ClearSaveStations();
		base.OnDisposed();
	}
}
