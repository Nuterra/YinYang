using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
