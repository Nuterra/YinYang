using System;
using System.Data.Entity;

namespace YinYang.Community
{
	public sealed class CommunityContext : DbContext
	{
		public const string ConnectionStringName = "YinYang." + nameof(CommunityContext);

		public DbSet<Account> Accounts { get; set; }
		public DbSet<TechUpload> Techs { get; set; }

		public CommunityContext() : base("name=" + ConnectionStringName)
		{
			Database.Log = s => System.Diagnostics.Debug.Write(s);
		}
	}
}