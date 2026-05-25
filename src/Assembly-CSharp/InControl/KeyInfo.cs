using UnityEngine;

namespace InControl;

public struct KeyInfo
{
	private readonly Key key;

	private readonly string name;

	private readonly string macName;

	private readonly KeyCode[] keyCodes;

	public static readonly KeyInfo[] KeyList;

	public bool IsPressed
	{
		get
		{
			int num = keyCodes.Length;
			for (int i = 0; i < num; i++)
			{
				if (Input.GetKey(keyCodes[i]))
				{
					return true;
				}
			}
			return false;
		}
	}

	public string Name
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			if ((int)Application.platform == 0 || (int)Application.platform == 1)
			{
				return macName;
			}
			return name;
		}
	}

	public Key Key => key;

	private KeyInfo(Key key, string name, params KeyCode[] keyCodes)
	{
		this.key = key;
		this.name = name;
		macName = name;
		this.keyCodes = keyCodes;
	}

	private KeyInfo(Key key, string name, string macName, params KeyCode[] keyCodes)
	{
		this.key = key;
		this.name = name;
		this.macName = macName;
		this.keyCodes = keyCodes;
	}

	static KeyInfo()
	{
		KeyList = new KeyInfo[111]
		{
			new KeyInfo(Key.None, "None", default(KeyCode)),
			new KeyInfo(Key.Shift, "Shift", (KeyCode)304, (KeyCode)303),
			new KeyInfo(Key.Alt, "Alt", "Option", (KeyCode)308, (KeyCode)307),
			new KeyInfo(Key.Command, "Command", (KeyCode)310, (KeyCode)309),
			new KeyInfo(Key.Control, "Control", (KeyCode)306, (KeyCode)305),
			new KeyInfo(Key.LeftShift, "Left Shift", (KeyCode)304),
			new KeyInfo(Key.LeftAlt, "Left Alt", "Left Option", (KeyCode)308),
			new KeyInfo(Key.LeftCommand, "Left Command", (KeyCode)310),
			new KeyInfo(Key.LeftControl, "Left Control", (KeyCode)306),
			new KeyInfo(Key.RightShift, "Right Shift", (KeyCode)303),
			new KeyInfo(Key.RightAlt, "Right Alt", "Right Option", (KeyCode)307),
			new KeyInfo(Key.RightCommand, "Right Command", (KeyCode)309),
			new KeyInfo(Key.RightControl, "Right Control", (KeyCode)305),
			new KeyInfo(Key.Escape, "Escape", (KeyCode)27),
			new KeyInfo(Key.F1, "F1", (KeyCode)282),
			new KeyInfo(Key.F2, "F2", (KeyCode)283),
			new KeyInfo(Key.F3, "F3", (KeyCode)284),
			new KeyInfo(Key.F4, "F4", (KeyCode)285),
			new KeyInfo(Key.F5, "F5", (KeyCode)286),
			new KeyInfo(Key.F6, "F6", (KeyCode)287),
			new KeyInfo(Key.F7, "F7", (KeyCode)288),
			new KeyInfo(Key.F8, "F8", (KeyCode)289),
			new KeyInfo(Key.F9, "F9", (KeyCode)290),
			new KeyInfo(Key.F10, "F10", (KeyCode)291),
			new KeyInfo(Key.F11, "F11", (KeyCode)292),
			new KeyInfo(Key.F12, "F12", (KeyCode)293),
			new KeyInfo(Key.Key0, "Num 0", (KeyCode)48),
			new KeyInfo(Key.Key1, "Num 1", (KeyCode)49),
			new KeyInfo(Key.Key2, "Num 2", (KeyCode)50),
			new KeyInfo(Key.Key3, "Num 3", (KeyCode)51),
			new KeyInfo(Key.Key4, "Num 4", (KeyCode)52),
			new KeyInfo(Key.Key5, "Num 5", (KeyCode)53),
			new KeyInfo(Key.Key6, "Num 6", (KeyCode)54),
			new KeyInfo(Key.Key7, "Num 7", (KeyCode)55),
			new KeyInfo(Key.Key8, "Num 8", (KeyCode)56),
			new KeyInfo(Key.Key9, "Num 9", (KeyCode)57),
			new KeyInfo(Key.A, "A", (KeyCode)97),
			new KeyInfo(Key.B, "B", (KeyCode)98),
			new KeyInfo(Key.C, "C", (KeyCode)99),
			new KeyInfo(Key.D, "D", (KeyCode)100),
			new KeyInfo(Key.E, "E", (KeyCode)101),
			new KeyInfo(Key.F, "F", (KeyCode)102),
			new KeyInfo(Key.G, "G", (KeyCode)103),
			new KeyInfo(Key.H, "H", (KeyCode)104),
			new KeyInfo(Key.I, "I", (KeyCode)105),
			new KeyInfo(Key.J, "J", (KeyCode)106),
			new KeyInfo(Key.K, "K", (KeyCode)107),
			new KeyInfo(Key.L, "L", (KeyCode)108),
			new KeyInfo(Key.M, "M", (KeyCode)109),
			new KeyInfo(Key.N, "N", (KeyCode)110),
			new KeyInfo(Key.O, "O", (KeyCode)111),
			new KeyInfo(Key.P, "P", (KeyCode)112),
			new KeyInfo(Key.Q, "Q", (KeyCode)113),
			new KeyInfo(Key.R, "R", (KeyCode)114),
			new KeyInfo(Key.S, "S", (KeyCode)115),
			new KeyInfo(Key.T, "T", (KeyCode)116),
			new KeyInfo(Key.U, "U", (KeyCode)117),
			new KeyInfo(Key.V, "V", (KeyCode)118),
			new KeyInfo(Key.W, "W", (KeyCode)119),
			new KeyInfo(Key.X, "X", (KeyCode)120),
			new KeyInfo(Key.Y, "Y", (KeyCode)121),
			new KeyInfo(Key.Z, "Z", (KeyCode)122),
			new KeyInfo(Key.Backquote, "Backquote", (KeyCode)96),
			new KeyInfo(Key.Minus, "Minus", (KeyCode)45),
			new KeyInfo(Key.Equals, "Equals", (KeyCode)61),
			new KeyInfo(Key.Backspace, "Backspace", "Delete", (KeyCode)8),
			new KeyInfo(Key.Tab, "Tab", (KeyCode)9),
			new KeyInfo(Key.LeftBracket, "Left Bracket", (KeyCode)91),
			new KeyInfo(Key.RightBracket, "Right Bracket", (KeyCode)93),
			new KeyInfo(Key.Backslash, "Backslash", (KeyCode)92),
			new KeyInfo(Key.Semicolon, "Semicolon", (KeyCode)59),
			new KeyInfo(Key.Quote, "Quote", (KeyCode)39),
			new KeyInfo(Key.Return, "Return", (KeyCode)13),
			new KeyInfo(Key.Comma, "Comma", (KeyCode)44),
			new KeyInfo(Key.Period, "Period", (KeyCode)46),
			new KeyInfo(Key.Slash, "Slash", (KeyCode)47),
			new KeyInfo(Key.Space, "Space", (KeyCode)32),
			new KeyInfo(Key.Insert, "Insert", (KeyCode)277),
			new KeyInfo(Key.Delete, "Delete", "Forward Delete", (KeyCode)127),
			new KeyInfo(Key.Home, "Home", (KeyCode)278),
			new KeyInfo(Key.End, "End", (KeyCode)279),
			new KeyInfo(Key.PageUp, "PageUp", (KeyCode)280),
			new KeyInfo(Key.PageDown, "PageDown", (KeyCode)281),
			new KeyInfo(Key.LeftArrow, "Left Arrow", (KeyCode)276),
			new KeyInfo(Key.RightArrow, "Right Arrow", (KeyCode)275),
			new KeyInfo(Key.UpArrow, "Up Arrow", (KeyCode)273),
			new KeyInfo(Key.DownArrow, "Down Arrow", (KeyCode)274),
			new KeyInfo(Key.Pad0, "Pad 0", (KeyCode)256),
			new KeyInfo(Key.Pad1, "Pad 1", (KeyCode)257),
			new KeyInfo(Key.Pad2, "Pad 2", (KeyCode)258),
			new KeyInfo(Key.Pad3, "Pad 3", (KeyCode)259),
			new KeyInfo(Key.Pad4, "Pad 4", (KeyCode)260),
			new KeyInfo(Key.Pad5, "Pad 5", (KeyCode)261),
			new KeyInfo(Key.Pad6, "Pad 6", (KeyCode)262),
			new KeyInfo(Key.Pad7, "Pad 7", (KeyCode)263),
			new KeyInfo(Key.Pad8, "Pad 8", (KeyCode)264),
			new KeyInfo(Key.Pad9, "Pad 9", (KeyCode)265),
			new KeyInfo(Key.Numlock, "Numlock", (KeyCode)300),
			new KeyInfo(Key.PadDivide, "Pad Divide", (KeyCode)267),
			new KeyInfo(Key.PadMultiply, "Pad Multiply", (KeyCode)268),
			new KeyInfo(Key.PadMinus, "Pad Minus", (KeyCode)269),
			new KeyInfo(Key.PadPlus, "Pad Plus", (KeyCode)270),
			new KeyInfo(Key.PadEnter, "Pad Enter", (KeyCode)271),
			new KeyInfo(Key.PadPeriod, "Pad Period", (KeyCode)266),
			new KeyInfo(Key.Clear, "Clear", (KeyCode)12),
			new KeyInfo(Key.PadEquals, "Pad Equals", (KeyCode)272),
			new KeyInfo(Key.F13, "F13", (KeyCode)294),
			new KeyInfo(Key.F14, "F14", (KeyCode)295),
			new KeyInfo(Key.F15, "F15", (KeyCode)296),
			new KeyInfo(Key.AltGr, "Alt Graphic", (KeyCode)313),
			new KeyInfo(Key.CapsLock, "Caps Lock", (KeyCode)301)
		};
	}
}
