using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InControl;

public struct VersionInfo(int major, int minor, int patch, int build) : IComparable<VersionInfo>
{
	public int Major = major;

	public int Minor = minor;

	public int Patch = patch;

	public int Build = build;

	public static VersionInfo Min => new VersionInfo(int.MinValue, int.MinValue, int.MinValue, int.MinValue);

	public static VersionInfo Max => new VersionInfo(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);

	public VersionInfo Next => new VersionInfo(Major, Minor, Patch, Build + 1);

	public static VersionInfo InControlVersion()
	{
		return new VersionInfo
		{
			Major = 1,
			Minor = 7,
			Patch = 1,
			Build = 9323
		};
	}

	public static VersionInfo UnityVersion()
	{
		Match match = Regex.Match(Application.unityVersion, "^(\\d+)\\.(\\d+)\\.(\\d+)");
		return new VersionInfo
		{
			Major = Convert.ToInt32(match.Groups[1].Value),
			Minor = Convert.ToInt32(match.Groups[2].Value),
			Patch = Convert.ToInt32(match.Groups[3].Value),
			Build = 0
		};
	}

	public int CompareTo(VersionInfo other)
	{
		if (Major < other.Major)
		{
			return -1;
		}
		if (Major > other.Major)
		{
			return 1;
		}
		if (Minor < other.Minor)
		{
			return -1;
		}
		if (Minor > other.Minor)
		{
			return 1;
		}
		if (Patch < other.Patch)
		{
			return -1;
		}
		if (Patch > other.Patch)
		{
			return 1;
		}
		if (Build < other.Build)
		{
			return -1;
		}
		if (Build > other.Build)
		{
			return 1;
		}
		return 0;
	}

	public static bool operator ==(VersionInfo a, VersionInfo b)
	{
		return a.CompareTo(b) == 0;
	}

	public static bool operator !=(VersionInfo a, VersionInfo b)
	{
		return a.CompareTo(b) != 0;
	}

	public static bool operator <=(VersionInfo a, VersionInfo b)
	{
		return a.CompareTo(b) <= 0;
	}

	public static bool operator >=(VersionInfo a, VersionInfo b)
	{
		return a.CompareTo(b) >= 0;
	}

	public static bool operator <(VersionInfo a, VersionInfo b)
	{
		return a.CompareTo(b) < 0;
	}

	public static bool operator >(VersionInfo a, VersionInfo b)
	{
		return a.CompareTo(b) > 0;
	}

	public override bool Equals(object other)
	{
		if (other is VersionInfo)
		{
			return this == (VersionInfo)other;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Major.GetHashCode() ^ Minor.GetHashCode() ^ Patch.GetHashCode() ^ Build.GetHashCode();
	}

	public override string ToString()
	{
		if (Build == 0)
		{
			return $"{Major}.{Minor}.{Patch}";
		}
		return $"{Major}.{Minor}.{Patch} build {Build}";
	}

	public string ToShortString()
	{
		if (Build == 0)
		{
			return $"{Major}.{Minor}.{Patch}";
		}
		return $"{Major}.{Minor}.{Patch}b{Build}";
	}
}
