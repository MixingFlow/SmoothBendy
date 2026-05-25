namespace InControl.NativeProfile;

public class MayflashMagicNSMacProfile : Xbox360DriverMacProfile
{
	public MayflashMagicNSMacProfile()
	{
		base.Name = "Mayflash Magic-NS";
		base.Meta = "Mayflash Magic-NS on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 121,
				ProductID = 6355
			}
		};
	}
}
