namespace InControl.NativeProfile;

public class AtPlayControllerMacProfile : Xbox360DriverMacProfile
{
	public AtPlayControllerMacProfile()
	{
		base.Name = "At Play Controller";
		base.Meta = "At Play Controller on Mac";
		Matchers = new NativeInputDeviceMatcher[2]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 9414,
				ProductID = 64250
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 9414,
				ProductID = 64251
			}
		};
	}
}
