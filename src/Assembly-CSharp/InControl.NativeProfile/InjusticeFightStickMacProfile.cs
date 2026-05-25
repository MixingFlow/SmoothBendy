namespace InControl.NativeProfile;

public class InjusticeFightStickMacProfile : Xbox360DriverMacProfile
{
	public InjusticeFightStickMacProfile()
	{
		base.Name = "Injustice Fight Stick";
		base.Meta = "Injustice Fight Stick on Mac";
		Matchers = new NativeInputDeviceMatcher[1]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 293
			}
		};
	}
}
