using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace YinYang.Community
{
	[DataContract]
	public class TechUpload
	{
		[Key]
		[DataMember]
		public int ID { get; set; }

		[Required]
		[DataMember]
		[JsonConverter(typeof(LongToStringConverter))]
		public long OwnerID { get; set; }

		[ForeignKey("OwnerID")]
		public virtual Account Owner { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string ImageUrl { get; set; }

		[DataMember]
		public string TechData { get; set; }

		public virtual ICollection<Account> Subscribers { get; set; }

		public TechUpload()
		{
			Subscribers = new HashSet<Account>();
		}
	}
}