using System;

namespace YinYang.Community
{
	[Flags]
	public enum AccountFlags
	{
		None = 0x00,
		Verified = 0x01,
		Banned = 0x02,
		Admin = 0x04,
	}
}