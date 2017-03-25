using System;
using System.Collections.Generic;
using YinYang.Community;
using YinYang.Steam;

namespace YinYang.Session
{
	public sealed class HttpSession
	{
		public DateTime Created { get; }
		public DateTime Expires { get; private set; }
		public bool IsNew { get; private set; }
		public SteamID SteamID { get; set; } = new SteamID();
		public Account UserAccount { get; set; }

		public HttpSession(TimeSpan timeValid)
		{
			IsNew = true;
			Created = DateTime.Now;
			Expires = Created;
			ExtendLifetime(timeValid);
		}

		public void MarkUsed()
		{
			IsNew = false;
		}

		public bool IsValid()
		{
			return (Created <= DateTime.Now) && (DateTime.Now <= Expires);
		}

		public void ExtendLifetime(TimeSpan duration)
		{
			if (duration.Ticks < 0)
			{
				throw new ArgumentException("Cannot extend by negative time", nameof(duration));
			}

			Expires += duration;
		}

	}
}