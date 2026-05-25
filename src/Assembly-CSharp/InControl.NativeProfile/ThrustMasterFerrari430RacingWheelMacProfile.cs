namespace InControl.NativeProfile;

public class ThrustMasterFerrari430RacingWheelMacProfile : Xbox360DriverMacProfile
{
	public ThrustMasterFerrari430RacingWheelMacProfile()
	{
		base.Name = "ThrustMaster Ferrari 430 Racing Wheel";
		base.Meta = "ThrustMaster Ferrari 430 Racing Wheel on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 1103,
				ProductID = 46683
			}
		};
	}
}
