using System;

[Flags]
public enum MeshFlags
{
	None = 0,
	Generator = 1,
	Count = 2,
	Color = 4,
	Speed = 8,
	Surface = 0x10,
	Seed = 0x1C,
	All = 0x1F
}
