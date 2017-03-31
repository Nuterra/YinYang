using System;

namespace YinYang.Steam
{
	[Flags]
	public enum SteamInstance
	{
		All = 0,
		Desktop = 1,
		Console = 2,
		Web = 4,
	}
}