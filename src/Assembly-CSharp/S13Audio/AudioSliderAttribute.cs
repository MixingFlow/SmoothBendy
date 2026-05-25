using UnityEngine;

namespace S13Audio;

public class AudioSliderAttribute : PropertyAttribute
{
	public readonly string label;

	public readonly float minFloatValue;

	public readonly float maxFloatValue;

	public readonly int minIntValue;

	public readonly int maxIntValue;

	public readonly PropertyType propertyType;

	public AudioSliderAttribute(string label, float minValue, float maxValue)
	{
		this.label = label;
		minFloatValue = minValue;
		maxFloatValue = maxValue;
		propertyType = PropertyType.Float;
	}

	public AudioSliderAttribute(string label, int minValue, int maxValue)
	{
		this.label = label;
		minIntValue = minValue;
		maxIntValue = maxValue;
		propertyType = PropertyType.Int;
	}
}
