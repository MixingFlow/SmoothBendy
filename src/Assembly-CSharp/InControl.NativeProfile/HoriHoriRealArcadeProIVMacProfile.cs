namespace InControl.NativeProfile;

public class HoriHoriRealArcadeProIVMacProfile : Xbox360DriverMacProfile
{
	public HoriHoriRealArcadeProIVMacProfile()
	{
		base.Name = "Hori Hori Real Arcade Pro IV";
		base.Meta = "Hori Hori Real Arcade Pro IV on Mac";
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
