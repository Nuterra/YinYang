using System;

namespace YinYang.Steam
{
	//Source: https://github.com/DoctorMcKay/php-steamid/blob/master/src/SteamID.php
	public sealed class SteamID
	{
		public SteamUniverse Universe { get; }
		public SteamAccountType Type { get; }
		public SteamInstance Instance { get; }
		public uint AccountID { get; }

		public SteamID()
		{
		}

		public SteamID(string legacySteamID)
		{
			// Steam2 ID
			if (legacySteamID == null) throw new ArgumentNullException(nameof(legacySteamID));
			const string prefix = "STEAM_";

			if (!legacySteamID.StartsWith(prefix))
			{
				throw new ArgumentException($"SteamID missing prefix '{prefix}'", nameof(legacySteamID));
			}

			Universe = (SteamUniverse)int.Parse(legacySteamID.Substring(prefix.Length, 1));
			if (Universe == SteamUniverse.Invalid)
			{
				Universe = SteamUniverse.Public;
			}

			Type = SteamAccountType.Individual;
			Instance = SteamInstance.Desktop;
			string accountIdLowBitSegment = legacySteamID.Substring(prefix.Length + 2, 1);
			string accountIdHighBitsSegment = legacySteamID.Substring(prefix.Length + 4);
			long accountIDLowBit = accountIdLowBitSegment == "1" ? 1 : 0;
			long accountIDHighBits = long.Parse(accountIdHighBitsSegment);
			AccountID = (uint)(accountIDLowBit + (accountIDHighBits * 2));
		}

		public SteamID(long steamID64)
		{
			Universe = (SteamUniverse)(steamID64 >> 56);
			Type = (SteamAccountType)((steamID64 >> 52) & 0xF);
			Instance = (SteamInstance)((steamID64 >> 32) & 0xFFFFF);
			AccountID = (uint)(steamID64 & 0xFFFFFFFF);
		}

		internal SteamID(SteamUniverse universe, SteamAccountType accountType, SteamInstance instance, uint accountID)
		{
			Universe = universe;
			Type = accountType;
			Instance = instance;
			AccountID = accountID;
		}

		public string ToSteamID2()
		{
			var accountIdLowBit = AccountID & 1;
			var accountIdHighBits = (AccountID >> 1) & 0x7FFFFFF;
			return $"STEAM_{(int)Universe}:{accountIdLowBit}:{accountIdHighBits}";
		}

		public long ToSteamID64()
		{
			long universe = (long)Universe;
			long type = (long)Type;
			long instance = (long)Instance;
			return (universe << 56) | (type << 52) | (instance << 32) | AccountID;
		}

		public override bool Equals(object obj)
		{
			SteamID otherID = obj as SteamID;
			if (otherID == null) return false;
			return (
				AccountID == otherID.AccountID &&
				Instance == otherID.Instance &&
				Type == otherID.Type &&
				Universe == otherID.Universe
				);
		}

		public bool IsValid()
		{
			switch (Universe)
			{
				case SteamUniverse.Invalid:
				case SteamUniverse.Dev:
					return false;
			}
			switch (Type)
			{
				case SteamAccountType.Invalid:
				case SteamAccountType.AnonymousUser:
					return false;

				case SteamAccountType.Individual:
					return AccountID != 0 && Instance <= SteamInstance.Web;

				case SteamAccountType.Clan:
					return AccountID != 0 && Instance == SteamInstance.All;

				case SteamAccountType.GameServer:
					return AccountID != 0;

				default:
					return true;
			}
		}

		public override string ToString()
		{
			return ToSteamID64().ToString();
		}
	}
}