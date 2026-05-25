namespace InControl.NativeProfile;

public class LogitechThunderpadMacProfile : Xbox360DriverMacProfile
{
	public LogitechThunderpadMacProfile()
	{
		base.Name = "Logitech Thunderpad";
		base.Meta = "Logitech Thunderpad on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 1133,
				ProductID = 51848
			}
		};
	}
}
