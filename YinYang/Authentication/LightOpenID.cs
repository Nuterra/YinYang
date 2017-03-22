using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace YinYang.Authentication
{
	//Source: https://github.com/SmItH197/SteamAuthentication/blob/master/steamauth/openid.php
	public sealed class LightOpenID
	{
		private string _host;
		private string _server;
		private string _setupUrl;
		private NameValueCollection _queryParams;

		/// <summary>
		/// The URI the current server is hosted on.
		/// </summary>
		public Uri Realm { get; set; }

		/// <summary>
		/// The localpath the provider is instructed to send the client to.
		/// </summary>
		public Uri ReturnUrl { get; set; }// TODO: Ensure it is inside the realm

		public string Mode { get; } // TODO: Get this from the _GET / _POST info

		public LightOpenID()
		{

		}

		public LightOpenID(Uri clientReturnUri)
		{
			_queryParams = HttpUtility.ParseQueryString(clientReturnUri.Query);

			Mode = _queryParams.GetValues("openid.mode")?.Single();
			_setupUrl = _queryParams.GetValues("openid.user_setup_url")?.Single();
		}

		public async Task<string> GetAuthUrl(string identity)
		{
			if (_server == null)
			{
				_server = await Discover(identity);
			}
			return FormatAuthUrl_v2(false).ToString();
		}

		private Uri FormatAuthUrl_v2(bool immediate)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();
			arguments.Add("openid.ns", "http://specs.openid.net/auth/2.0");
			arguments.Add("openid.mode", immediate ? "checkid_immediate" : "checkid_setup");
			arguments.Add("openid.return_to", ReturnUrl.AbsoluteUri);
			arguments.Add("openid.realm", Realm.AbsoluteUri);

			string thing = "http://specs.openid.net/auth/2.0/identifier_select";
			arguments.Add("openid.identity", thing);
			arguments.Add("openid.claimed_id", thing);


			UriBuilder builder = new UriBuilder(_server);

			builder.Query += GetUriString("&", arguments);

			return builder.Uri;
		}
		private string GetUriString(string argSeperator, Dictionary<string, string> items)
		{
			return String.Join(argSeperator,
							   items.Select(kvp =>
							   {
								   var key = Uri.EscapeDataString(kvp.Key);
								   var value = Uri.EscapeDataString(kvp.Value.ToString());
								   return $"{key}={value}";
							   }));
		}

		private async Task<string> Discover(string url)
		{
			HttpClient client = new HttpClient();
			string response = await client.GetStringAsync(url);
			var document = new XmlDocument();
			document.LoadXml(response);
			string xpath = "//*[local-name() = 'Service']";
			var node = document.SelectSingleNode(xpath);
			return node["URI"].InnerText;
		}
		public string ClaimedID { get; private set; }
		public async Task<bool> Validate()
		{
			if (_setupUrl != null)
			{
				return false;
			}
			if (Mode != "id_res")
			{
				return false;
			}
			// isset($this->data['openid_claimed_id']) ?$this->data['openid_claimed_id']:$this->data['openid_identity'];
			ClaimedID = _queryParams.GetValues("openid.claimed_id")?.Single() ?? _queryParams.GetValues("openid.identity")?.Single();

			//Build the request to validate with the openid server
			var args = new Dictionary<string, string>();
			args.Add("openid.assoc_handle", _queryParams.GetValues("openid.assoc_handle")?.Single());
			args.Add("openid.signed", _queryParams.GetValues("openid.signed")?.Single());
			args.Add("openid.sig", _queryParams.GetValues("openid.sig")?.Single());

			if (_queryParams.GetValues("openid.ns") != null)
			{
				//OpenID 2.0 server, find endpoint using discovery
				args.Add("openid.ns", "http://specs.openid.net/auth/2.0");
			}

			if (!ReturnUrl.Equals(_queryParams.GetValues("openid.return_to").Single()))
			{
				return false;
			}

			string server = await Discover(ClaimedID);

			var items = _queryParams.GetValues("openid.signed")[0];

			var claims = items.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string claim in claims)
			{
				string key = $"openid.{claim}";
				string value = _queryParams.GetValues(key).Single();
				if (!args.ContainsKey(key))
				{
					args.Add(key, value);
				}
			}

			args.Add("openid.mode", "check_authentication");

			var content = new FormUrlEncodedContent(args);
			HttpClient client = new HttpClient();

			var response = await client.PostAsync(server, content);
			var text = await response.Content.ReadAsStringAsync();

			return text.Contains("is_valid:true");
		}
	}
}
