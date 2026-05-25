namespace InControl.NativeProfile;

public class PDPXboxOneControllerMacProfile : XboxOneDriverMacProfile
{
	public PDPXboxOneControllerMacProfile()
	{
		base.Name = "PDP Xbox One Controller";
		base.Meta = "PDP Xbox One Controller on Mac";
		Matchers = new NativeInputDeviceMatcher[12]
		{
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 676
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 314
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 354
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 9414,
				ProductID = 22042
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 353
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 355
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 683
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 352
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 347
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 685
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 679
			},
			new NativeInputDeviceMatcher
			{
				VendorID = 3695,
				ProductID = 678
			}
		};
	}
}
