using TMG.Core;
using UnityEngine;

public class SeasonalInitializer : TMGMonoBehaviour
{
	public GameObject Seasonal_InkDemonsEve;

	private SeasonalController m_Seasonal;

	public override void Init()
	{
		SeasonalType seasonalType = SeasonalType.None;
		if (SeasonalCheck.IsSeasonal(out seasonalType))
		{
			CheckSeason(seasonalType);
		}
		if ((Object)(object)m_Seasonal != (Object)null)
		{
			m_Seasonal.Initialize();
		}
		Dispose();
	}

	private void CheckSeason(SeasonalType seasonalType)
	{
		if (seasonalType == SeasonalType.InkDemonsEve)
		{
			InitializeInkDemonsEve();
		}
	}

	private void InitializeInkDemonsEve()
	{
		if ((Object)(object)Seasonal_InkDemonsEve != (Object)null)
		{
			m_Seasonal = Object.Instantiate<GameObject>(Seasonal_InkDemonsEve).GetComponent<SeasonalController>();
		}
	}
}
