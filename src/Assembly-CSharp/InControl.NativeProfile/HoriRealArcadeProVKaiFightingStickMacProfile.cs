namespace InControl.NativeProfile;

public class HoriRealArcadeProVKaiFightingStickMacProfile : Xbox360DriverMacProfile
{
	public HoriRealArcadeProVKaiFightingStickMacProfile()
	{
		base.Name = "Hori Real Arcade Pro V Kai Fighting Stick";
		base.Meta = "Hori Real Arcade Pro V Kai Fighting Stick on Mac";
		Matchers = new NativeInputDeviceMatcher[2]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 9414,
				ProductID = 21774
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3853,
				ProductID = 120
			}
		};
	}
}
