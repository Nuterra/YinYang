using System;
using YinYang.Steam;

namespace YinYang.Session
{
	public sealed class HttpSession
	{
		public DateTime Created { get; }
		public DateTime Expires { get; private set; }
		public bool IsNew { get; private set; }
		public SteamID SteamID { get; set; }

		public HttpSession(TimeSpan timeValid)
		{
			IsNew = true;
			Created = DateTime.Now;
			Expires = Created;
			ExtendLifetime(timeValid);
			//SteamID = new SteamID(76561198023393043);
#warning REMOVE FROM PRODUCTION
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