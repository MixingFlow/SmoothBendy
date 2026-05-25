namespace InControl.NativeProfile;

public class MadCatzControllerMacProfile : Xbox360DriverMacProfile
{
	public MadCatzControllerMacProfile()
	{
		base.Name = "Mad Catz Controller";
		base.Meta = "Mad Catz Controller on Mac";
		Matchers = new NativeInputDeviceMatcher[4]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 1848,
				ProductID = 18198
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 7085,
				ProductID = 63746
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 7085,
				ProductID = 61642
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 1848,
				ProductID = 672
			}
		};
	}
}
