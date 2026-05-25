namespace InControl.NativeProfile;

public class HyperkinX91MacProfile : Xbox360DriverMacProfile
{
	public HyperkinX91MacProfile()
	{
		base.Name = "Hyperkin X91";
		base.Meta = "Hyperkin X91 on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 11812,
				ProductID = 5768
			}
		};
	}
}
