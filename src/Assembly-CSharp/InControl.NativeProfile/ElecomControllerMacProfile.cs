namespace InControl.NativeProfile;

public class ElecomControllerMacProfile : Xbox360DriverMacProfile
{
	public ElecomControllerMacProfile()
	{
		base.Name = "Elecom Controller";
		base.Meta = "Elecom Controller on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 1390,
				ProductID = 8196
			}
		};
	}
}
