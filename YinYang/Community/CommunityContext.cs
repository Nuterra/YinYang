using System;
using System.Data.Entity;

namespace YinYang.Community
{
	public sealed class CommunityContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }

		public CommunityContext(string connectionString) : base(connectionString)
		{
		}
	}
}