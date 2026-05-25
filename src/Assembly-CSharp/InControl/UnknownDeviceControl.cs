using System;
using System.IO;

namespace InControl;

public struct UnknownDeviceControl(InputControlType control, InputRangeType sourceRange) : IEquatable<UnknownDeviceControl>
{
	public static readonly UnknownDeviceControl None = new UnknownDeviceControl(InputControlType.None, InputRangeType.None);

	public InputControlType Control = control;

	public InputRangeType SourceRange = sourceRange;

	public bool IsButton = Utility.TargetIsButton(control);

	public bool IsAnalog = !IsButton;

	public int Index => (int)(Control - ((!IsButton) ? 400 : 500));

	internal float GetValue(InputDevice device)
	{
		if (device == null)
		{
			return 0f;
		}
		float value = device.GetControl(Control).Value;
		return InputRange.Remap(value, SourceRange, InputRangeType.ZeroToOne);
	}

	public static bool operator ==(UnknownDeviceControl a, UnknownDeviceControl b)
	{
		if (object.ReferenceEquals(null, a))
		{
			return object.ReferenceEquals(null, b);
		}
		return a.Equals(b);
	}

	public static bool operator !=(UnknownDeviceControl a, UnknownDeviceControl b)
	{
		return !(a == b);
	}

	public bool Equals(UnknownDeviceControl other)
	{
		return Control == other.Control && SourceRange == other.SourceRange;
	}

	public override bool Equals(object other)
	{
		return Equals((UnknownDeviceControl)other);
	}

	public override int GetHashCode()
	{
		return Control.GetHashCode() ^ SourceRange.GetHashCode();
	}

	public static implicit operator bool(UnknownDeviceControl control)
	{
		return control.Control != InputControlType.None;
	}

	public override string ToString()
	{
		return $"UnknownDeviceControl( {Control.ToString()}, {SourceRange.ToString()} )";
	}

	internal void Save(BinaryWriter writer)
	{
		writer.Write((int)Control);
		writer.Write((int)SourceRange);
	}

	internal void Load(BinaryReader reader)
	{
		Control = (InputControlType)reader.ReadInt32();
		SourceRange = (InputRangeType)reader.ReadInt32();
		IsButton = Utility.TargetIsButton(Control);
		IsAnalog = !IsButton;
	}
}
