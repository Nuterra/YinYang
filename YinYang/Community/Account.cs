using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YinYang.Community
{
	[DataContract]
	public class Account
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[DataMember]
		[JsonConverter(typeof(LongToStringConverter))]
		public long SteamID { get; set; }

		[DataMember]
		public string Username { get; set; }

		[DataMember]
		public AccountFlags Flags { get; set; }

		[InverseProperty("Subscribers")]
		public virtual ICollection<TechUpload> SubscribedTechs { get; set; }

		[InverseProperty("Owner")]
		public virtual ICollection<TechUpload> UploadedTechs { get; set; }

		public Account()
		{
			SubscribedTechs = new HashSet<TechUpload>();
			UploadedTechs = new HashSet<TechUpload>();
		}
	}

	public class LongToStringConverter : JsonConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JToken jt = JToken.ReadFrom(reader);
			return jt.Value<long>();
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(long).Equals(objectType);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value.ToString());
		}
	}
}