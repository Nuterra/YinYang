﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace YinYang.Community
{
	[DataContract]
	public class Account
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[DataMember]
		public long SteamID { get; set; }

		[DataMember]
		public string Username { get; set; }

		[DataMember]
		public AccountFlags Flags { get; set; }
	}
}