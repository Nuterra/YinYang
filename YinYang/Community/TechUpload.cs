using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace YinYang.Community
{
	[DataContract]
	public class TechUpload
	{
		[Key]
		[DataMember]
		public int ID { get; set; }

		[Required]
		public long OwnerID { get; set; }

		[ForeignKey("OwnerID")]
		public virtual Account Owner { get; set; }

		[DataMember]
		public string Title { get; set; }
	}
}