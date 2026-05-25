namespace InControl.NativeProfile;

public class HoriRealArcadeProIVMacProfile : Xbox360DriverMacProfile
{
	public HoriRealArcadeProIVMacProfile()
	{
		base.Name = "Hori Real Arcade Pro IV";
		base.Meta = "Hori Real Arcade Pro IV on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3853,
				ProductID = 140
			}
		};
	}
}
