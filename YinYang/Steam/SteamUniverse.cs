using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YinYang.Steam
{
	//Source: https://developer.valvesoftware.com/wiki/SteamID
	public enum SteamUniverse
	{
		Invalid = 0,
		Public = 1,
		Beta = 2,
		Internal = 3,
		Dev = 4,
		RC = 5,
	}
}
