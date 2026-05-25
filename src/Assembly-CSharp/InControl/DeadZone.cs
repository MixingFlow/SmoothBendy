using System;
using UnityEngine;

namespace InControl;

public static class DeadZone
{
	public static Vector2 SeparateNotNormalized(float x, float y, float lowerDeadZone, float upperDeadZone)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		float num = upperDeadZone - lowerDeadZone;
		float num2 = ((x < 0f) ? ((x > 0f - lowerDeadZone) ? 0f : ((!(x < 0f - upperDeadZone)) ? ((x + lowerDeadZone) / num) : (-1f))) : ((x < lowerDeadZone) ? 0f : ((!(x > upperDeadZone)) ? ((x - lowerDeadZone) / num) : 1f)));
		float num3 = ((y < 0f) ? ((y > 0f - lowerDeadZone) ? 0f : ((!(y < 0f - upperDeadZone)) ? ((y + lowerDeadZone) / num) : (-1f))) : ((y < lowerDeadZone) ? 0f : ((!(y > upperDeadZone)) ? ((y - lowerDeadZone) / num) : 1f)));
		return new Vector2(num2, num3);
	}

	public static Vector2 Separate(float x, float y, float lowerDeadZone, float upperDeadZone)
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		float num = upperDeadZone - lowerDeadZone;
		float num2 = ((x < 0f) ? ((x > 0f - lowerDeadZone) ? 0f : ((!(x < 0f - upperDeadZone)) ? ((x + lowerDeadZone) / num) : (-1f))) : ((x < lowerDeadZone) ? 0f : ((!(x > upperDeadZone)) ? ((x - lowerDeadZone) / num) : 1f)));
		float num3 = ((y < 0f) ? ((y > 0f - lowerDeadZone) ? 0f : ((!(y < 0f - upperDeadZone)) ? ((y + lowerDeadZone) / num) : (-1f))) : ((y < lowerDeadZone) ? 0f : ((!(y > upperDeadZone)) ? ((y - lowerDeadZone) / num) : 1f)));
		float num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
		if (num4 < 1E-05f)
		{
			return Vector2.zero;
		}
		return new Vector2(num2 / num4, num3 / num4);
	}

	public static Vector2 Circular(float x, float y, float lowerDeadZone, float upperDeadZone)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)Math.Sqrt(x * x + y * y);
		if (num < lowerDeadZone || num < 1E-05f)
		{
			return Vector2.zero;
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(x / num, y / num);
		if (num > upperDeadZone)
		{
			return val;
		}
		return val * ((num - lowerDeadZone) / (upperDeadZone - lowerDeadZone));
	}
}
