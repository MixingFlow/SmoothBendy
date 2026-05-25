namespace InControl.NativeProfile;

public class RockCandyPS3ControllerMacProfile : Xbox360DriverMacProfile
{
	public RockCandyPS3ControllerMacProfile()
	{
		base.Name = "Rock Candy PS3 Controller";
		base.Meta = "Rock Candy PS3 Controller on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 286
			}
		};
	}
}
