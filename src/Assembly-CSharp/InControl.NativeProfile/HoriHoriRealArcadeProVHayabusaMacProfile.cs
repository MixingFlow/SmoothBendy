namespace InControl.NativeProfile;

public class HoriHoriRealArcadeProVHayabusaMacProfile : Xbox360DriverMacProfile
{
	public HoriHoriRealArcadeProVHayabusaMacProfile()
	{
		base.Name = "Hori Hori Real Arcade Pro V Hayabusa";
		base.Meta = "Hori Hori Real Arcade Pro V Hayabusa on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3853,
				ProductID = 216
			}
		};
	}
}
