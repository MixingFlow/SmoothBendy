using UnityEngine;

namespace TMPro;

public static class TMP_TextUtilities
{
	private struct LineSegment(Vector3 p1, Vector3 p2)
	{
		public Vector3 Point1 = p1;

		public Vector3 Point2 = p2;
	}

	private static Vector3[] m_rectWorldCorners = (Vector3[])(object)new Vector3[4];

	private const string k_lookupStringL = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[-]^_`abcdefghijklmnopqrstuvwxyz{|}~-";

	private const string k_lookupStringU = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-";

	public static CaretInfo GetCursorInsertionIndex(TMP_Text textComponent, Vector3 position, Camera camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		int num = FindNearestCharacter(textComponent, position, camera, visibleOnly: false);
		RectTransform rectTransform = textComponent.rectTransform;
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		TMP_CharacterInfo tMP_CharacterInfo = textComponent.textInfo.characterInfo[num];
		Vector3 val = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.bottomLeft);
		Vector3 val2 = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.topRight);
		float num2 = (position.x - val.x) / (val2.x - val.x);
		if (num2 < 0.5f)
		{
			return new CaretInfo(num, CaretPosition.Left);
		}
		return new CaretInfo(num, CaretPosition.Right);
	}

	public static int GetCursorIndexFromPosition(TMP_Text textComponent, Vector3 position, Camera camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		int num = FindNearestCharacter(textComponent, position, camera, visibleOnly: false);
		RectTransform rectTransform = textComponent.rectTransform;
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		TMP_CharacterInfo tMP_CharacterInfo = textComponent.textInfo.characterInfo[num];
		Vector3 val = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.bottomLeft);
		Vector3 val2 = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.topRight);
		float num2 = (position.x - val.x) / (val2.x - val.x);
		if (num2 < 0.5f)
		{
			return num;
		}
		return num + 1;
	}

	public static int GetCursorIndexFromPosition(TMP_Text textComponent, Vector3 position, Camera camera, out CaretPosition cursor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		int num = FindNearestCharacter(textComponent, position, camera, visibleOnly: false);
		RectTransform rectTransform = textComponent.rectTransform;
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		TMP_CharacterInfo tMP_CharacterInfo = textComponent.textInfo.characterInfo[num];
		Vector3 val = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.bottomLeft);
		Vector3 val2 = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.topRight);
		float num2 = (position.x - val.x) / (val2.x - val.x);
		if (num2 < 0.5f)
		{
			cursor = CaretPosition.Left;
			return num;
		}
		cursor = CaretPosition.Right;
		return num;
	}

	public static bool IsIntersectingRectTransform(RectTransform rectTransform, Vector3 position, Camera camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		rectTransform.GetWorldCorners(m_rectWorldCorners);
		if (PointIntersectRectangle(position, m_rectWorldCorners[0], m_rectWorldCorners[1], m_rectWorldCorners[2], m_rectWorldCorners[3]))
		{
			return true;
		}
		return false;
	}

	public static int FindIntersectingCharacter(TMP_Text text, Vector3 position, Camera camera, bool visibleOnly)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		RectTransform rectTransform = text.rectTransform;
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		for (int i = 0; i < text.textInfo.characterCount; i++)
		{
			TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[i];
			if ((!visibleOnly || tMP_CharacterInfo.isVisible) && (text.OverflowMode != TextOverflowModes.Page || tMP_CharacterInfo.pageNumber + 1 == text.pageToDisplay))
			{
				Vector3 a = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.bottomLeft);
				Vector3 b = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.topRight.y, 0f));
				Vector3 c = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.topRight);
				Vector3 d = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.bottomLeft.y, 0f));
				if (PointIntersectRectangle(position, a, b, c, d))
				{
					return i;
				}
			}
		}
		return -1;
	}

	public static int FindNearestCharacter(TMP_Text text, Vector3 position, Camera camera, bool visibleOnly)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		RectTransform rectTransform = text.rectTransform;
		float num = float.PositiveInfinity;
		int result = 0;
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		for (int i = 0; i < text.textInfo.characterCount; i++)
		{
			TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[i];
			if ((!visibleOnly || tMP_CharacterInfo.isVisible) && (text.OverflowMode != TextOverflowModes.Page || tMP_CharacterInfo.pageNumber + 1 == text.pageToDisplay))
			{
				Vector3 val = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.bottomLeft);
				Vector3 val2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.topRight.y, 0f));
				Vector3 val3 = ((Transform)rectTransform).TransformPoint(tMP_CharacterInfo.topRight);
				Vector3 val4 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.bottomLeft.y, 0f));
				if (PointIntersectRectangle(position, val, val2, val3, val4))
				{
					return i;
				}
				float num2 = DistanceToLine(val, val2, position);
				float num3 = DistanceToLine(val2, val3, position);
				float num4 = DistanceToLine(val3, val4, position);
				float num5 = DistanceToLine(val4, val, position);
				float num6 = ((!(num2 < num3)) ? num3 : num2);
				num6 = ((!(num6 < num4)) ? num4 : num6);
				num6 = ((!(num6 < num5)) ? num5 : num6);
				if (num > num6)
				{
					num = num6;
					result = i;
				}
			}
		}
		return result;
	}

	public static int FindIntersectingWord(TMP_Text text, Vector3 position, Camera camera)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		RectTransform rectTransform = text.rectTransform;
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		for (int i = 0; i < text.textInfo.wordCount; i++)
		{
			TMP_WordInfo tMP_WordInfo = text.textInfo.wordInfo[i];
			bool flag = false;
			Vector3 a = Vector3.zero;
			Vector3 b = Vector3.zero;
			Vector3 d = Vector3.zero;
			Vector3 c = Vector3.zero;
			float num = float.NegativeInfinity;
			float num2 = float.PositiveInfinity;
			for (int j = 0; j < tMP_WordInfo.characterCount; j++)
			{
				int num3 = tMP_WordInfo.firstCharacterIndex + j;
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num3];
				int lineNumber = tMP_CharacterInfo.lineNumber;
				bool flag2 = ((num3 <= text.maxVisibleCharacters && tMP_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || tMP_CharacterInfo.pageNumber + 1 == text.pageToDisplay)) ? true : false);
				num = Mathf.Max(num, tMP_CharacterInfo.ascender);
				num2 = Mathf.Min(num2, tMP_CharacterInfo.descender);
				if (!flag && flag2)
				{
					flag = true;
					((Vector3)(ref a))._002Ector(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f);
					((Vector3)(ref b))._002Ector(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f);
					if (tMP_WordInfo.characterCount == 1)
					{
						flag = false;
						((Vector3)(ref d))._002Ector(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
						((Vector3)(ref c))._002Ector(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
						a = ((Transform)rectTransform).TransformPoint(new Vector3(a.x, num2, 0f));
						b = ((Transform)rectTransform).TransformPoint(new Vector3(b.x, num, 0f));
						c = ((Transform)rectTransform).TransformPoint(new Vector3(c.x, num, 0f));
						d = ((Transform)rectTransform).TransformPoint(new Vector3(d.x, num2, 0f));
						if (PointIntersectRectangle(position, a, b, c, d))
						{
							return i;
						}
					}
				}
				if (flag && j == tMP_WordInfo.characterCount - 1)
				{
					flag = false;
					((Vector3)(ref d))._002Ector(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
					((Vector3)(ref c))._002Ector(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
					a = ((Transform)rectTransform).TransformPoint(new Vector3(a.x, num2, 0f));
					b = ((Transform)rectTransform).TransformPoint(new Vector3(b.x, num, 0f));
					c = ((Transform)rectTransform).TransformPoint(new Vector3(c.x, num, 0f));
					d = ((Transform)rectTransform).TransformPoint(new Vector3(d.x, num2, 0f));
					if (PointIntersectRectangle(position, a, b, c, d))
					{
						return i;
					}
				}
				else if (flag && lineNumber != text.textInfo.characterInfo[num3 + 1].lineNumber)
				{
					flag = false;
					((Vector3)(ref d))._002Ector(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
					((Vector3)(ref c))._002Ector(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
					a = ((Transform)rectTransform).TransformPoint(new Vector3(a.x, num2, 0f));
					b = ((Transform)rectTransform).TransformPoint(new Vector3(b.x, num, 0f));
					c = ((Transform)rectTransform).TransformPoint(new Vector3(c.x, num, 0f));
					d = ((Transform)rectTransform).TransformPoint(new Vector3(d.x, num2, 0f));
					num = float.NegativeInfinity;
					num2 = float.PositiveInfinity;
					if (PointIntersectRectangle(position, a, b, c, d))
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public static int FindNearestWord(TMP_Text text, Vector3 position, Camera camera)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		RectTransform rectTransform = text.rectTransform;
		float num = float.PositiveInfinity;
		int result = 0;
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		for (int i = 0; i < text.textInfo.wordCount; i++)
		{
			TMP_WordInfo tMP_WordInfo = text.textInfo.wordInfo[i];
			bool flag = false;
			Vector3 val = Vector3.zero;
			Vector3 val2 = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			for (int j = 0; j < tMP_WordInfo.characterCount; j++)
			{
				int num2 = tMP_WordInfo.firstCharacterIndex + j;
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num2];
				int lineNumber = tMP_CharacterInfo.lineNumber;
				bool flag2 = ((num2 <= text.maxVisibleCharacters && tMP_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || tMP_CharacterInfo.pageNumber + 1 == text.pageToDisplay)) ? true : false);
				if (!flag && flag2)
				{
					flag = true;
					val = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
					val2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
					if (tMP_WordInfo.characterCount == 1)
					{
						flag = false;
						zero = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						zero2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, val, val2, zero2, zero))
						{
							return i;
						}
						float num3 = DistanceToLine(val, val2, position);
						float num4 = DistanceToLine(val2, zero2, position);
						float num5 = DistanceToLine(zero2, zero, position);
						float num6 = DistanceToLine(zero, val, position);
						float num7 = ((!(num3 < num4)) ? num4 : num3);
						num7 = ((!(num7 < num5)) ? num5 : num7);
						num7 = ((!(num7 < num6)) ? num6 : num7);
						if (num > num7)
						{
							num = num7;
							result = i;
						}
					}
				}
				if (flag && j == tMP_WordInfo.characterCount - 1)
				{
					flag = false;
					zero = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
					zero2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
					if (PointIntersectRectangle(position, val, val2, zero2, zero))
					{
						return i;
					}
					float num8 = DistanceToLine(val, val2, position);
					float num9 = DistanceToLine(val2, zero2, position);
					float num10 = DistanceToLine(zero2, zero, position);
					float num11 = DistanceToLine(zero, val, position);
					float num12 = ((!(num8 < num9)) ? num9 : num8);
					num12 = ((!(num12 < num10)) ? num10 : num12);
					num12 = ((!(num12 < num11)) ? num11 : num12);
					if (num > num12)
					{
						num = num12;
						result = i;
					}
				}
				else if (flag && lineNumber != text.textInfo.characterInfo[num2 + 1].lineNumber)
				{
					flag = false;
					zero = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
					zero2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
					if (PointIntersectRectangle(position, val, val2, zero2, zero))
					{
						return i;
					}
					float num13 = DistanceToLine(val, val2, position);
					float num14 = DistanceToLine(val2, zero2, position);
					float num15 = DistanceToLine(zero2, zero, position);
					float num16 = DistanceToLine(zero, val, position);
					float num17 = ((!(num13 < num14)) ? num14 : num13);
					num17 = ((!(num17 < num15)) ? num15 : num17);
					num17 = ((!(num17 < num16)) ? num16 : num17);
					if (num > num17)
					{
						num = num17;
						result = i;
					}
				}
			}
		}
		return result;
	}

	public static int FindIntersectingLink(TMP_Text text, Vector3 position, Camera camera)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = text.transform;
		ScreenPointToWorldPointInRectangle(transform, Vector2.op_Implicit(position), camera, out position);
		for (int i = 0; i < text.textInfo.linkCount; i++)
		{
			TMP_LinkInfo tMP_LinkInfo = text.textInfo.linkInfo[i];
			bool flag = false;
			Vector3 a = Vector3.zero;
			Vector3 b = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			for (int j = 0; j < tMP_LinkInfo.linkTextLength; j++)
			{
				int num = tMP_LinkInfo.linkTextfirstCharacterIndex + j;
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num];
				int lineNumber = tMP_CharacterInfo.lineNumber;
				if (text.OverflowMode == TextOverflowModes.Page && tMP_CharacterInfo.pageNumber + 1 != text.pageToDisplay)
				{
					continue;
				}
				if (!flag)
				{
					flag = true;
					a = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
					b = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
					if (tMP_LinkInfo.linkTextLength == 1)
					{
						flag = false;
						zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
				}
				if (flag && j == tMP_LinkInfo.linkTextLength - 1)
				{
					flag = false;
					zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
					zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
					if (PointIntersectRectangle(position, a, b, zero2, zero))
					{
						return i;
					}
				}
				else if (flag && lineNumber != text.textInfo.characterInfo[num + 1].lineNumber)
				{
					flag = false;
					zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
					zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
					if (PointIntersectRectangle(position, a, b, zero2, zero))
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public static int FindNearestLink(TMP_Text text, Vector3 position, Camera camera)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		RectTransform rectTransform = text.rectTransform;
		ScreenPointToWorldPointInRectangle((Transform)(object)rectTransform, Vector2.op_Implicit(position), camera, out position);
		float num = float.PositiveInfinity;
		int result = 0;
		for (int i = 0; i < text.textInfo.linkCount; i++)
		{
			TMP_LinkInfo tMP_LinkInfo = text.textInfo.linkInfo[i];
			bool flag = false;
			Vector3 val = Vector3.zero;
			Vector3 val2 = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			for (int j = 0; j < tMP_LinkInfo.linkTextLength; j++)
			{
				int num2 = tMP_LinkInfo.linkTextfirstCharacterIndex + j;
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num2];
				int lineNumber = tMP_CharacterInfo.lineNumber;
				if (text.OverflowMode == TextOverflowModes.Page && tMP_CharacterInfo.pageNumber + 1 != text.pageToDisplay)
				{
					continue;
				}
				if (!flag)
				{
					flag = true;
					val = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
					val2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
					if (tMP_LinkInfo.linkTextLength == 1)
					{
						flag = false;
						zero = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						zero2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, val, val2, zero2, zero))
						{
							return i;
						}
						float num3 = DistanceToLine(val, val2, position);
						float num4 = DistanceToLine(val2, zero2, position);
						float num5 = DistanceToLine(zero2, zero, position);
						float num6 = DistanceToLine(zero, val, position);
						float num7 = ((!(num3 < num4)) ? num4 : num3);
						num7 = ((!(num7 < num5)) ? num5 : num7);
						num7 = ((!(num7 < num6)) ? num6 : num7);
						if (num > num7)
						{
							num = num7;
							result = i;
						}
					}
				}
				if (flag && j == tMP_LinkInfo.linkTextLength - 1)
				{
					flag = false;
					zero = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
					zero2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
					if (PointIntersectRectangle(position, val, val2, zero2, zero))
					{
						return i;
					}
					float num8 = DistanceToLine(val, val2, position);
					float num9 = DistanceToLine(val2, zero2, position);
					float num10 = DistanceToLine(zero2, zero, position);
					float num11 = DistanceToLine(zero, val, position);
					float num12 = ((!(num8 < num9)) ? num9 : num8);
					num12 = ((!(num12 < num10)) ? num10 : num12);
					num12 = ((!(num12 < num11)) ? num11 : num12);
					if (num > num12)
					{
						num = num12;
						result = i;
					}
				}
				else if (flag && lineNumber != text.textInfo.characterInfo[num2 + 1].lineNumber)
				{
					flag = false;
					zero = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
					zero2 = ((Transform)rectTransform).TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
					if (PointIntersectRectangle(position, val, val2, zero2, zero))
					{
						return i;
					}
					float num13 = DistanceToLine(val, val2, position);
					float num14 = DistanceToLine(val2, zero2, position);
					float num15 = DistanceToLine(zero2, zero, position);
					float num16 = DistanceToLine(zero, val, position);
					float num17 = ((!(num13 < num14)) ? num14 : num13);
					num17 = ((!(num17 < num15)) ? num15 : num17);
					num17 = ((!(num17 < num16)) ? num16 : num17);
					if (num > num17)
					{
						num = num17;
						result = i;
					}
				}
			}
		}
		return result;
	}

	private static bool PointIntersectRectangle(Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = b - a;
		Vector3 val2 = m - a;
		Vector3 val3 = c - b;
		Vector3 val4 = m - b;
		float num = Vector3.Dot(val, val2);
		float num2 = Vector3.Dot(val3, val4);
		return 0f <= num && num <= Vector3.Dot(val, val) && 0f <= num2 && num2 <= Vector3.Dot(val3, val3);
	}

	public static bool ScreenPointToWorldPointInRectangle(Transform transform, Vector2 screenPoint, Camera cam, out Vector3 worldPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		worldPoint = Vector2.op_Implicit(Vector2.zero);
		Ray val = RectTransformUtility.ScreenPointToRay(cam, screenPoint);
		Plane val2 = default(Plane);
		((Plane)(ref val2))._002Ector(transform.rotation * Vector3.back, transform.position);
		float num = default(float);
		if (!((Plane)(ref val2)).Raycast(val, ref num))
		{
			return false;
		}
		worldPoint = ((Ray)(ref val)).GetPoint(num);
		return true;
	}

	private static bool IntersectLinePlane(LineSegment line, Vector3 point, Vector3 normal, out Vector3 intersectingPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		intersectingPoint = Vector3.zero;
		Vector3 val = line.Point2 - line.Point1;
		Vector3 val2 = line.Point1 - point;
		float num = Vector3.Dot(normal, val);
		float num2 = 0f - Vector3.Dot(normal, val2);
		if (Mathf.Abs(num) < Mathf.Epsilon)
		{
			if (num2 == 0f)
			{
				return true;
			}
			return false;
		}
		float num3 = num2 / num;
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		intersectingPoint = line.Point1 + num3 * val;
		return true;
	}

	public static float DistanceToLine(Vector3 a, Vector3 b, Vector3 point)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = b - a;
		Vector3 val2 = a - point;
		float num = Vector3.Dot(val, val2);
		if (num > 0f)
		{
			return Vector3.Dot(val2, val2);
		}
		Vector3 val3 = point - b;
		if (Vector3.Dot(val, val3) > 0f)
		{
			return Vector3.Dot(val3, val3);
		}
		Vector3 val4 = val2 - val * (num / Vector3.Dot(val, val));
		return Vector3.Dot(val4, val4);
	}

	public static char ToLowerFast(char c)
	{
		return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[-]^_`abcdefghijklmnopqrstuvwxyz{|}~-"[c];
	}

	public static char ToUpperFast(char c)
	{
		return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-"[c];
	}

	public static int GetSimpleHashCode(string s)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			num = ((num << 5) + num) ^ s[i];
		}
		return num;
	}

	public static uint GetSimpleHashCodeLowercase(string s)
	{
		uint num = 5381u;
		for (int i = 0; i < s.Length; i++)
		{
			num = ((num << 5) + num) ^ ToLowerFast(s[i]);
		}
		return num;
	}

	public static int HexToInt(char hex)
	{
		return hex switch
		{
			'0' => 0, 
			'1' => 1, 
			'2' => 2, 
			'3' => 3, 
			'4' => 4, 
			'5' => 5, 
			'6' => 6, 
			'7' => 7, 
			'8' => 8, 
			'9' => 9, 
			'A' => 10, 
			'B' => 11, 
			'C' => 12, 
			'D' => 13, 
			'E' => 14, 
			'F' => 15, 
			'a' => 10, 
			'b' => 11, 
			'c' => 12, 
			'd' => 13, 
			'e' => 14, 
			'f' => 15, 
			_ => 15, 
		};
	}

	public static int StringToInt(string s)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			num += HexToInt(s[i]) * (int)Mathf.Pow(16f, (float)(s.Length - 1 - i));
		}
		return num;
	}
}
