namespace InControl.NativeProfile;

public class ThrustMasterFerrari458SpiderRacingWheelMacProfile : Xbox360DriverMacProfile
{
	public ThrustMasterFerrari458SpiderRacingWheelMacProfile()
	{
		base.Name = "ThrustMaster Ferrari 458 Spider Racing Wheel";
		base.Meta = "ThrustMaster Ferrari 458 Spider Racing Wheel on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 1103,
				ProductID = 46705
			}
		};
	}
}
