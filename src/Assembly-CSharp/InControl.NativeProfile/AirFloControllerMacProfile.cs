namespace InControl.NativeProfile;

public class AirFloControllerMacProfile : Xbox360DriverMacProfile
{
	public AirFloControllerMacProfile()
	{
		base.Name = "Air Flo Controller";
		base.Meta = "Air Flo Controller on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 9414,
				ProductID = 21251
			}
		};
	}
}
