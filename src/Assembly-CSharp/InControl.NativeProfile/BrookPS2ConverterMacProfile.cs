namespace InControl.NativeProfile;

public class BrookPS2ConverterMacProfile : Xbox360DriverMacProfile
{
	public BrookPS2ConverterMacProfile()
	{
		base.Name = "Brook PS2 Converter";
		base.Meta = "Brook PS2 Converter on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3090,
				ProductID = 2289
			}
		};
	}
}
