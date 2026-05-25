namespace InControl.NativeProfile;

public class HoriFightingCommanderMacProfile : Xbox360DriverMacProfile
{
	public HoriFightingCommanderMacProfile()
	{
		base.Name = "Hori Fighting Commander";
		base.Meta = "Hori Fighting Commander on Mac";
		Matchers = new NativeInputDeviceMatcher[2]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3853,
				ProductID = 197
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 9414,
				ProductID = 21776
			}
		};
	}
}
