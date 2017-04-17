using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YinYang.Api
{
	public static class MultipartFormParserasdf
	{
		private static readonly Regex ContentTypeFormDataRegex = new Regex(
			@"^(multipart/(mixed|form\-data));\s*boundary=([\w-]+)$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly Regex HeaderStartRegex = new Regex(
			"^(?'key'[\\w-]+):\\s*(?'value'[^;]+)(;\\s*(?'leftover'.*))?$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly Regex FormDataRegex = new Regex(
			"(((name=\"(?'name'[^\"]*)\")|(filename=\"(?'filename'[^\"]*)\"));?\\s*)*",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly Regex OldRegex = new Regex(
			"((?'name'[\\w-/+]+)(=\"(?'value'[^\"]*)\")?;?)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static async Task<Dictionary<string, object>> ParseMultipartForm(string contentType, Stream body)
		{
			var barrierMatch = ContentTypeFormDataRegex.Match(contentType);
			string mainBarrier = barrierMatch.Groups[3].Value;
			string barrier = "--" + mainBarrier;
			string endBarrier = "--" + mainBarrier + "--";
			Dictionary<string, object> result = new Dictionary<string, object>();

			byte[] info = await ReadUntil(body, Encoding.UTF8.GetBytes(barrier + "\r\n"), false);

			if (info.Length > 0)
			{
				throw new Exception("Body didnt start with barrier");
			}

			barrier = "\r\n" + barrier;
			byte[] barrierBytes = Encoding.UTF8.GetBytes(barrier);

			while (true)
			{
				HeaderInfo headers = await ReadHeaders(body);
				object value = await ReadValue(body, headers, barrierBytes);
				string nextline = await ReadLine(body, Encoding.UTF8);
				if (nextline.Equals("--"))
				{
					break;
				}
				else if (!string.IsNullOrEmpty(nextline))
				{
					throw new Exception("unexpexted barrier");
				}
				//Store value
				result.Add(headers.PartName, value);
			}

			return result;
		}

		private static async Task<object> ReadValue(Stream body, HeaderInfo headers, byte[] barrierBytes)
		{
			if (headers.IsFile)
			{
				return await ReadFile(body, headers, barrierBytes);
			}
			else if (headers.IsFileList)
			{
				return await ReadFileList(body, headers, barrierBytes);
			}
			else
			{
				return await ReadString(body, headers, barrierBytes);
			}
		}

		private static async Task<FileUpload> ReadFile(Stream body, HeaderInfo headers, byte[] barrierBytes)
		{
			byte[] bytes = await ReadUntil(body, barrierBytes, false);
			return new FileUpload(headers.FileName, bytes);
		}

		private static Task<object> ReadFileList(Stream body, HeaderInfo headers, byte[] barrierBytes)
		{
			throw new NotImplementedException();
		}

		private static async Task<object> ReadString(Stream body, HeaderInfo headers, byte[] barrierBytes)
		{
			var bytes = await ReadUntil(body, barrierBytes, false);
			return Encoding.UTF8.GetString(bytes);
		}

		private static async Task<HeaderInfo> ReadHeaders(Stream body)
		{
			HeaderInfo result = new HeaderInfo();
			while (true)
			{
				//Content-Disposition: form-data; name="NAME"
				//Content-Disposition: form-data; name="NAME"; filename="FILENAME"
				//Content-Disposition: form-data; ***
				//split by ;
				//Key: Value
				//all other: a=b, trim spaces
				string line = await ReadLine(body, Encoding.UTF8);
				if (string.IsNullOrEmpty(line))
				{
					break;
				}
				//Read header
				var match = HeaderStartRegex.Match(line);
				if (!match.Success)
				{
					throw new Exception("not reading a header!");
				}

				string key = match.Groups["key"].Value;
				string value = match.Groups["value"].Value;
				string leftover = match.Groups["leftover"].Value;

				switch (key.ToLower())
				{
					case "content-disposition":
						{
							switch (value.ToLower())
							{
								case "form-data":
									Match metaMatch = FormDataRegex.Match(leftover);
									if (!metaMatch.Success)
									{
										throw new Exception("header is missing part info");
									}
									result.PartName = metaMatch.Groups["name"].Value;
									result.FileName = metaMatch.Groups["filename"].Value;
									break;

								case "file":
									break;

							}
						}
						break;

					case "content-type":
						result.ContentType = value;
						break;
				}
			}
			return result;
		}

		private static async Task<byte[]> ReadUntil(Stream input, byte[] boundary, bool outputTarget)
		{
			List<byte> userdata = new List<byte>();
			int headstart = 0;
			while (true)
			{
				byte[] next = new byte[1];
				int read = await input.ReadAsync(next, 0, 1);
				if (read == 0)
				{
					continue;
				}

				if (boundary[headstart] == next[0])
				{
					headstart++;
					if (headstart >= boundary.Length)
					{
						if (outputTarget)
						{
							userdata.AddRange(boundary);
						}
						break;
					}
				}
				else
				{
					userdata.AddRange(boundary.Take(headstart));
					userdata.AddRange(next);
					headstart = 0;
				}
			}
			return userdata.ToArray();
		}

		private static async Task<string> ReadLine(Stream input, Encoding coding)
		{
			byte[] line = await ReadUntil(input, coding.GetBytes("\r\n"), false);
			return coding.GetString(line);
		}
	}

	internal sealed class HeaderInfo
	{
		public string PartName { get; set; }

		public bool IsFile
		{
			get { return !string.IsNullOrEmpty(FileName); }
		}

		public bool IsFileList { get; set; }
		public string ListBoundary { get; set; }
		public string FileName { get; set; }
		public string ContentType { get; set; }
	}
}