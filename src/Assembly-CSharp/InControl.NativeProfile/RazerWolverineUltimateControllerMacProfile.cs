namespace InControl.NativeProfile;

public class RazerWolverineUltimateControllerMacProfile : Xbox360DriverMacProfile
{
	public RazerWolverineUltimateControllerMacProfile()
	{
		base.Name = "Razer Wolverine Ultimate Controller";
		base.Meta = "Razer Wolverine Ultimate Controller on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 5426,
				ProductID = 2580
			}
		};
	}
}
