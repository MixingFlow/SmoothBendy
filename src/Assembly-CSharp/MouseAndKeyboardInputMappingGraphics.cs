using System;
using UnityEngine;

[CreateAssetMenu]
public class MouseAndKeyboardInputMappingGraphics : ScriptableObject
{
	[Serializable]
	public class InputToGfxMapping
	{
		public KeyCode keyCode;

		public Sprite gfx;
	}

	[SerializeField]
	private InputToGfxMapping[] mappings;

	public Sprite GetSpriteForInput(KeyCode keyCode)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		InputToGfxMapping[] array = mappings;
		foreach (InputToGfxMapping inputToGfxMapping in array)
		{
			if (keyCode == inputToGfxMapping.keyCode)
			{
				return inputToGfxMapping.gfx;
			}
		}
		return null;
	}
}
