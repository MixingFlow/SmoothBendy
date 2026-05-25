namespace InControl;

[AutoDiscover]
public class NvidiaShieldRemoteAndroidProfile : UnityInputDeviceProfile
{
	public NvidiaShieldRemoteAndroidProfile()
	{
		base.Name = "NVIDIA Shield Remote";
		base.Meta = "NVIDIA Shield Remote on Android";
		base.DeviceClass = InputDeviceClass.Remote;
		base.DeviceStyle = InputDeviceStyle.NVIDIAShield;
		base.IncludePlatforms = new string[1] { "Android" };
		JoystickNames = new string[1] { "SHIELD Remote" };
		JoystickRegex = new string[1] { "SHIELD Remote" };
		base.ButtonMappings = new InputControlMapping[1]
		{
			new InputControlMapping
			{
				Handle = "A",
				Target = InputControlType.Action1,
				Source = UnityInputDeviceProfile.Button0
			}
		};
		base.AnalogMappings = new InputControlMapping[4]
		{
			UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog4),
			UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog4),
			UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog5),
			UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5)
		};
	}
}
