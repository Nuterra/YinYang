using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace YinYang.Community
{
	public class Tech
	{
		[Key]
		public int ID { get; set; }

		[Required]
		[JsonConverter(typeof(LongToStringConverter))]
		public long OwnerID { get; set; }

		[ForeignKey("OwnerID")]
		public virtual Account Owner { get; set; }

		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public string TechData { get; set; }

		[InverseProperty("SubscribedTechs")]
		public virtual ICollection<Account> Subscribers { get; set; } = new HashSet<Account>();

		public bool Featured { get; set; }

		public DateTime CreationTime { get; set; } = DateTime.Now;
	}
}