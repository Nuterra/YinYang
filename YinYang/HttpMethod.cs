using System;
using System.Linq;

namespace YinYang
{
	public sealed class HttpMethod
	{
		public static readonly HttpMethod Get = new HttpMethod("GET");
		public static readonly HttpMethod Post = new HttpMethod("POST");
		public static readonly HttpMethod Put = new HttpMethod("PUT");
		public static readonly HttpMethod Patch = new HttpMeathod("PATCH");
		public static readonly HttpMethod Delete = new HttpMethod("DELETE");

		private string _name;

		public HttpMethod(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (name.Length == 0) throw new ArgumentException("Invalid method name, cannot be empty", nameof(name));
			if (!IsValidHttpMethod(name)) throw new ArgumentException("Invalid method name, can only contain letters", nameof(name));
			_name = name.ToUpper();
		}

		public static HttpMethod Parse(string method)
		{
			return new HttpMethod(method);
		}

		public static bool IsValidHttpMethod(string method)
		{
			return method.All(char.IsLetter);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is HttpMethod)
			{
				HttpMethod method = (HttpMethod)obj;
				return _name.Equals(method._name, StringComparison.OrdinalIgnoreCase);
			}
			if (obj is string)
			{
				string method = (string)obj;
				return _name.Equals(method, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode();
		}

		public override string ToString()
		{
			return _name;
		}
	}
}