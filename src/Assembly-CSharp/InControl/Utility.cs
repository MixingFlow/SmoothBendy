using System;
using System.IO;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InControl;

public static class Utility
{
	public const float Epsilon = 1E-07f;

	private static Vector2[] circleVertexList = (Vector2[])(object)new Vector2[25]
	{
		new Vector2(0f, 1f),
		new Vector2(0.2588f, 0.9659f),
		new Vector2(0.5f, 0.866f),
		new Vector2(0.7071f, 0.7071f),
		new Vector2(0.866f, 0.5f),
		new Vector2(0.9659f, 0.2588f),
		new Vector2(1f, 0f),
		new Vector2(0.9659f, -0.2588f),
		new Vector2(0.866f, -0.5f),
		new Vector2(0.7071f, -0.7071f),
		new Vector2(0.5f, -0.866f),
		new Vector2(0.2588f, -0.9659f),
		new Vector2(0f, -1f),
		new Vector2(-0.2588f, -0.9659f),
		new Vector2(-0.5f, -0.866f),
		new Vector2(-0.7071f, -0.7071f),
		new Vector2(-0.866f, -0.5f),
		new Vector2(-0.9659f, -0.2588f),
		new Vector2(-1f, -0f),
		new Vector2(-0.9659f, 0.2588f),
		new Vector2(-0.866f, 0.5f),
		new Vector2(-0.7071f, 0.7071f),
		new Vector2(-0.5f, 0.866f),
		new Vector2(-0.2588f, 0.9659f),
		new Vector2(0f, 1f)
	};

	internal static bool Is32Bit => IntPtr.Size == 4;

	internal static bool Is64Bit => IntPtr.Size == 8;

	public static void DrawCircleGizmo(Vector2 center, float radius)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = circleVertexList[0] * radius + center;
		int num = circleVertexList.Length;
		for (int i = 1; i < num; i++)
		{
			Gizmos.DrawLine(Vector2.op_Implicit(val), Vector2.op_Implicit(val = circleVertexList[i] * radius + center));
		}
	}

	public static void DrawCircleGizmo(Vector2 center, float radius, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = color;
		DrawCircleGizmo(center, radius);
	}

	public static void DrawOvalGizmo(Vector2 center, Vector2 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = size / 2f;
		Vector2 val2 = Vector2.Scale(circleVertexList[0], val) + center;
		int num = circleVertexList.Length;
		for (int i = 1; i < num; i++)
		{
			Gizmos.DrawLine(Vector2.op_Implicit(val2), Vector2.op_Implicit(val2 = Vector2.Scale(circleVertexList[i], val) + center));
		}
	}

	public static void DrawOvalGizmo(Vector2 center, Vector2 size, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = color;
		DrawOvalGizmo(center, size);
	}

	public static void DrawRectGizmo(Rect rect)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMin);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMin);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax);
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMax);
		Gizmos.DrawLine(val, val2);
		Gizmos.DrawLine(val2, val3);
		Gizmos.DrawLine(val3, val4);
		Gizmos.DrawLine(val4, val);
	}

	public static void DrawRectGizmo(Rect rect, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = color;
		DrawRectGizmo(rect);
	}

	public static void DrawRectGizmo(Vector2 center, Vector2 size)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		float num = size.x / 2f;
		float num2 = size.y / 2f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(center.x - num, center.y - num2);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(center.x + num, center.y - num2);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(center.x + num, center.y + num2);
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector(center.x - num, center.y + num2);
		Gizmos.DrawLine(val, val2);
		Gizmos.DrawLine(val2, val3);
		Gizmos.DrawLine(val3, val4);
		Gizmos.DrawLine(val4, val);
	}

	public static void DrawRectGizmo(Vector2 center, Vector2 size, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = color;
		DrawRectGizmo(center, size);
	}

	public static bool GameObjectIsCulledOnCurrentCamera(GameObject gameObject)
	{
		return (Camera.current.cullingMask & (1 << gameObject.layer)) == 0;
	}

	public static Color MoveColorTowards(Color color0, Color color1, float maxDelta)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.MoveTowards(color0.r, color1.r, maxDelta);
		float num2 = Mathf.MoveTowards(color0.g, color1.g, maxDelta);
		float num3 = Mathf.MoveTowards(color0.b, color1.b, maxDelta);
		float num4 = Mathf.MoveTowards(color0.a, color1.a, maxDelta);
		return new Color(num, num2, num3, num4);
	}

	public static float ApplyDeadZone(float value, float lowerDeadZone, float upperDeadZone)
	{
		float num = upperDeadZone - lowerDeadZone;
		if (value < 0f)
		{
			if (value > 0f - lowerDeadZone)
			{
				return 0f;
			}
			if (value < 0f - upperDeadZone)
			{
				return -1f;
			}
			return (value + lowerDeadZone) / num;
		}
		if (value < lowerDeadZone)
		{
			return 0f;
		}
		if (value > upperDeadZone)
		{
			return 1f;
		}
		return (value - lowerDeadZone) / num;
	}

	public static float ApplySmoothing(float thisValue, float lastValue, float deltaTime, float sensitivity)
	{
		if (Approximately(sensitivity, 1f))
		{
			return thisValue;
		}
		float num = deltaTime * sensitivity * 100f;
		if (IsNotZero(thisValue) && Mathf.Sign(lastValue) != Mathf.Sign(thisValue))
		{
			lastValue = 0f;
		}
		return Mathf.MoveTowards(lastValue, thisValue, num);
	}

	public static float ApplySnapping(float value, float threshold)
	{
		if (value < 0f - threshold)
		{
			return -1f;
		}
		if (value > threshold)
		{
			return 1f;
		}
		return 0f;
	}

	internal static bool TargetIsButton(InputControlType target)
	{
		return (target >= InputControlType.Action1 && target <= InputControlType.Action12) || (target >= InputControlType.Button0 && target <= InputControlType.Button19);
	}

	internal static bool TargetIsStandard(InputControlType target)
	{
		return (target >= InputControlType.LeftStickUp && target <= InputControlType.Action12) || (target >= InputControlType.Command && target <= InputControlType.DPadY);
	}

	internal static bool TargetIsAlias(InputControlType target)
	{
		return target >= InputControlType.Command && target <= InputControlType.DPadY;
	}

	public static string ReadFromFile(string path)
	{
		StreamReader streamReader = new StreamReader(path);
		string result = streamReader.ReadToEnd();
		streamReader.Close();
		return result;
	}

	public static void WriteToFile(string path, string data)
	{
		StreamWriter streamWriter = new StreamWriter(path);
		streamWriter.Write(data);
		streamWriter.Flush();
		streamWriter.Close();
	}

	public static float Abs(float value)
	{
		return (!(value < 0f)) ? value : (0f - value);
	}

	public static bool Approximately(float v1, float v2)
	{
		float num = v1 - v2;
		return num >= -1E-07f && num <= 1E-07f;
	}

	public static bool Approximately(Vector2 v1, Vector2 v2)
	{
		return Approximately(v1.x, v2.x) && Approximately(v1.y, v2.y);
	}

	public static bool IsNotZero(float value)
	{
		return value < -1E-07f || value > 1E-07f;
	}

	public static bool IsZero(float value)
	{
		return value >= -1E-07f && value <= 1E-07f;
	}

	public static bool AbsoluteIsOverThreshold(float value, float threshold)
	{
		return value < 0f - threshold || value > threshold;
	}

	public static float NormalizeAngle(float angle)
	{
		while (angle < 0f)
		{
			angle += 360f;
		}
		while (angle > 360f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public static float VectorToAngle(Vector2 vector)
	{
		if (IsZero(vector.x) && IsZero(vector.y))
		{
			return 0f;
		}
		return NormalizeAngle(Mathf.Atan2(vector.x, vector.y) * 57.29578f);
	}

	public static float Min(float v0, float v1)
	{
		return (!(v0 >= v1)) ? v0 : v1;
	}

	public static float Max(float v0, float v1)
	{
		return (!(v0 <= v1)) ? v0 : v1;
	}

	public static float Min(float v0, float v1, float v2, float v3)
	{
		float num = ((!(v0 >= v1)) ? v0 : v1);
		float num2 = ((!(v2 >= v3)) ? v2 : v3);
		return (!(num >= num2)) ? num : num2;
	}

	public static float Max(float v0, float v1, float v2, float v3)
	{
		float num = ((!(v0 <= v1)) ? v0 : v1);
		float num2 = ((!(v2 <= v3)) ? v2 : v3);
		return (!(num <= num2)) ? num : num2;
	}

	internal static float ValueFromSides(float negativeSide, float positiveSide)
	{
		float num = Abs(negativeSide);
		float num2 = Abs(positiveSide);
		if (Approximately(num, num2))
		{
			return 0f;
		}
		return (!(num > num2)) ? num2 : (0f - num);
	}

	internal static float ValueFromSides(float negativeSide, float positiveSide, bool invertSides)
	{
		if (invertSides)
		{
			return ValueFromSides(positiveSide, negativeSide);
		}
		return ValueFromSides(negativeSide, positiveSide);
	}

	public static void ArrayResize<T>(ref T[] array, int capacity)
	{
		if (array == null || capacity > array.Length)
		{
			Array.Resize(ref array, NextPowerOfTwo(capacity));
		}
	}

	public static void ArrayExpand<T>(ref T[] array, int capacity)
	{
		if (array == null || capacity > array.Length)
		{
			array = new T[NextPowerOfTwo(capacity)];
		}
	}

	public static int NextPowerOfTwo(int value)
	{
		if (value > 0)
		{
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value++;
			return value;
		}
		return 0;
	}

	public static string HKLM_GetString(string path, string key)
	{
		try
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path);
			if (registryKey == null)
			{
				return string.Empty;
			}
			return (string)registryKey.GetValue(key);
		}
		catch
		{
			return null;
		}
	}

	public static string GetWindowsVersion()
	{
		string text = HKLM_GetString("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName");
		if (text != null)
		{
			string text2 = HKLM_GetString("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CSDVersion");
			string text3 = ((!Is32Bit) ? "64Bit" : "32Bit");
			int systemBuildNumber = GetSystemBuildNumber();
			return text + ((text2 == null) ? string.Empty : (" " + text2)) + " " + text3 + " Build " + systemBuildNumber;
		}
		return SystemInfo.operatingSystem;
	}

	public static int GetSystemBuildNumber()
	{
		return Environment.OSVersion.Version.Build;
	}

	internal static void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	internal static string PluginFileExtension()
	{
		return ".dll";
	}
}
