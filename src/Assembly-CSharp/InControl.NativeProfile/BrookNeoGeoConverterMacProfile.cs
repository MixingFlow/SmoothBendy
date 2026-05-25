namespace InControl.NativeProfile;

public class BrookNeoGeoConverterMacProfile : Xbox360DriverMacProfile
{
	public BrookNeoGeoConverterMacProfile()
	{
		base.Name = "Brook NeoGeo Converter";
		base.Meta = "Brook NeoGeo Converter on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3090,
				ProductID = 2036
			}
		};
	}
}
