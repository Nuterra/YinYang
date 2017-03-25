using System;
using System.ComponentModel.DataAnnotations;

namespace YinYang.Community
{
	public sealed class Account
	{
		[Key]
		public int ID { get; set; }

		public string Username { get; set; }

		public long SteamID { get; set; }

		public AccountFlags Flags { get; set; }
	}
}