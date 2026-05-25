using System;

namespace TMG.Data;

[Serializable]
public class SaveFileData
{
	public int ID;

	public int CurrentChapter;

	public bool HasDied;

	public float PlayTime;

	public bool IsNewGamePlus;

	public int CompleteCount;

	public int[] Internecions = new int[5];

	public string Internecion = string.Empty;

	public CH1DataVO CH1Data;

	public CH2DataVO CH2Data;

	public CH3DataVO CH3Data;

	public CH4DataVO CH4Data;

	public CH5DataVO CH5Data;

	public SaveFileData(int id)
	{
		ID = id;
	}
}
