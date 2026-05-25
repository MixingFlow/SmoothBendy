using System;
using InControl;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ControllerInputMappingGraphics : ScriptableObject
{
	[Serializable]
	public class InputToGFX
	{
		public string name;

		public InputControlType inputControlType;

		public Sprite playstationGFX;

		public Sprite xboxGFX;

		public Sprite switchGFX;
	}

	[FormerlySerializedAs("InputToGfxMappings")]
	public InputToGFX[] inputToGfxMappings;

	public Sprite GetSpriteForInputAndPlatform(InputControlType inputControlType, RuntimePlatform platform)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		InputToGFX[] array = inputToGfxMappings;
		foreach (InputToGFX inputToGFX in array)
		{
			if (inputToGFX.inputControlType == inputControlType)
			{
				if ((int)platform == 25)
				{
					return inputToGFX.playstationGFX;
				}
				if ((int)platform == 27)
				{
					return inputToGFX.xboxGFX;
				}
				if ((int)platform == 32)
				{
					return inputToGFX.switchGFX;
				}
				return inputToGFX.xboxGFX;
			}
		}
		return null;
	}
}
