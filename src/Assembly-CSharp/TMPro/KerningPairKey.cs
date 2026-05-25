namespace TMPro;

public struct KerningPairKey(int ascii_left, int ascii_right)
{
	public int ascii_Left = ascii_left;

	public int ascii_Right = ascii_right;

	public int key = (ascii_right << 16) + ascii_left;
}
