using UnityEngine;

namespace S13Audio;

public class S13Range
{
	private int _start;

	private int _end;

	public int Start
	{
		get
		{
			return _start;
		}
		set
		{
			Set(value, End);
		}
	}

	public int End
	{
		get
		{
			return _end;
		}
		set
		{
			Set(Start, value);
		}
	}

	public int Length => End - Start + 1;

	public S13Range(int start, int end)
	{
		Set(start, end);
	}

	public override string ToString()
	{
		return $"[Range: {Start} to {End}]";
	}

	public void Set(int start, int end)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (end - start < 0)
		{
			throw new UnityException("Invalid range.");
		}
		_start = start;
		_end = end;
	}

	public bool InRange(int value)
	{
		return Start <= value && value <= End;
	}
}
