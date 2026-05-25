namespace InControl.NativeProfile;

public class HoriRealArcadeProVHayabusaMacProfile : Xbox360DriverMacProfile
{
	public HoriRealArcadeProVHayabusaMacProfile()
	{
		base.Name = "Hori Real Arcade Pro V Hayabusa";
		base.Meta = "Hori Real Arcade Pro V Hayabusa on Mac";
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
