namespace InControl.NativeProfile;

public class MadCatzMC2RacingWheelMacProfile : Xbox360DriverMacProfile
{
	public MadCatzMC2RacingWheelMacProfile()
	{
		base.Name = "MadCatz MC2 Racing Wheel";
		base.Meta = "MadCatz MC2 Racing Wheel on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 7085,
				ProductID = 61472
			}
		};
	}
}
