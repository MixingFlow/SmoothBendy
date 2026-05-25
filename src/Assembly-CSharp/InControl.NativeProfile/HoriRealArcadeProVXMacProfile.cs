namespace InControl.NativeProfile;

public class HoriRealArcadeProVXMacProfile : Xbox360DriverMacProfile
{
	public HoriRealArcadeProVXMacProfile()
	{
		base.Name = "Hori Real Arcade Pro VX";
		base.Meta = "Hori Real Arcade Pro VX on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3853,
				ProductID = 27
			}
		};
	}
}
