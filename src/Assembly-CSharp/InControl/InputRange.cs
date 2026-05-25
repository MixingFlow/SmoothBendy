using UnityEngine;

namespace InControl;

public struct InputRange
{
	public static readonly InputRange None;

	public static readonly InputRange MinusOneToOne;

	public static readonly InputRange OneToMinusOne;

	public static readonly InputRange ZeroToOne;

	public static readonly InputRange ZeroToMinusOne;

	public static readonly InputRange OneToZero;

	public static readonly InputRange MinusOneToZero;

	public static readonly InputRange ZeroToNegativeInfinity;

	public static readonly InputRange ZeroToPositiveInfinity;

	public static readonly InputRange Everything;

	private static readonly InputRange[] TypeToRange;

	public readonly float Value0;

	public readonly float Value1;

	public readonly InputRangeType Type;

	private InputRange(float value0, float value1, InputRangeType type)
	{
		Value0 = value0;
		Value1 = value1;
		Type = type;
	}

	public InputRange(InputRangeType type)
	{
		Value0 = TypeToRange[(int)type].Value0;
		Value1 = TypeToRange[(int)type].Value1;
		Type = type;
	}

	public bool Includes(float value)
	{
		return !Excludes(value);
	}

	public bool Excludes(float value)
	{
		if (Type == InputRangeType.None)
		{
			return true;
		}
		return value < Mathf.Min(Value0, Value1) || value > Mathf.Max(Value0, Value1);
	}

	public static float Remap(float value, InputRange sourceRange, InputRange targetRange)
	{
		if (sourceRange.Excludes(value))
		{
			return 0f;
		}
		float num = Mathf.InverseLerp(sourceRange.Value0, sourceRange.Value1, value);
		return Mathf.Lerp(targetRange.Value0, targetRange.Value1, num);
	}

	internal static float Remap(float value, InputRangeType sourceRangeType, InputRangeType targetRangeType)
	{
		InputRange sourceRange = TypeToRange[(int)sourceRangeType];
		InputRange targetRange = TypeToRange[(int)targetRangeType];
		return Remap(value, sourceRange, targetRange);
	}

	static InputRange()
	{
		None = new InputRange(0f, 0f, InputRangeType.None);
		MinusOneToOne = new InputRange(-1f, 1f, InputRangeType.MinusOneToOne);
		OneToMinusOne = new InputRange(1f, -1f, InputRangeType.OneToMinusOne);
		ZeroToOne = new InputRange(0f, 1f, InputRangeType.ZeroToOne);
		ZeroToMinusOne = new InputRange(0f, -1f, InputRangeType.ZeroToMinusOne);
		OneToZero = new InputRange(1f, 0f, InputRangeType.OneToZero);
		MinusOneToZero = new InputRange(-1f, 0f, InputRangeType.MinusOneToZero);
		ZeroToNegativeInfinity = new InputRange(0f, float.NegativeInfinity, InputRangeType.ZeroToNegativeInfinity);
		ZeroToPositiveInfinity = new InputRange(0f, float.PositiveInfinity, InputRangeType.ZeroToPositiveInfinity);
		Everything = new InputRange(float.NegativeInfinity, float.PositiveInfinity, InputRangeType.Everything);
		TypeToRange = new InputRange[10] { None, MinusOneToOne, OneToMinusOne, ZeroToOne, ZeroToMinusOne, OneToZero, MinusOneToZero, ZeroToNegativeInfinity, ZeroToPositiveInfinity, Everything };
	}
}
