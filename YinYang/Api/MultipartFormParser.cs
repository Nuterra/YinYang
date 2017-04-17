using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YinYang.Api
{
	public static class MultipartFormParser
	{
		/*
		 * --AaB03x
   Content-Disposition: form-data; name="submit-name"

   Larry
   --AaB03x
   Content-Disposition: form-data; name="files"
   Content-Type: multipart/mixed; boundary=BbC04y

   --BbC04y
   Content-Disposition: file; filename="file1.txt"
   Content-Type: text/plain

   ... contents of file1.txt ...
   --BbC04y
   Content-Disposition: file; filename="file2.gif"
   Content-Type: image/gif
   Content-Transfer-Encoding: binary

   ...contents of file2.gif...
   --BbC04y--
   --AaB03x--
   */

		private static readonly Regex ContentTypeFormDataRegex = new Regex(
			@"^(multipart/(mixed|form\-data));\s*boundary=([\w-]+)$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly Regex HeaderStartRegex = new Regex(
			"^([\\w-]+):\\s*([^;]+);\\s*(.*)$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly Regex HeaderRegex = new Regex(
			"((?'name'[\\w-/+]+)(=\"(?'value'[^\"]*)\")?;?)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static async Task<Dictionary<string, FormValue>> ParseMultipartForm(string contentType, Stream body)
		{
			var barrierMatch = ContentTypeFormDataRegex.Match(contentType);
			string mainBarrier = barrierMatch.Groups[3].Value;
			string barrier = "--" + mainBarrier;
			string endBarrier = barrier + "--";
			Dictionary<string, FormValue> result = new Dictionary<string, FormValue>();
			using (StreamReader reader = new StreamReader(body))
			{
				while (!reader.EndOfStream)
				{
					var entry = await ParseMultipartProperty(barrier, reader);
					if (entry == null)
					{
						break;
					}
					FormValue existing;
					if (!result.TryGetValue(entry.Item1, out existing))
					{
						result.Add(entry.Item1, entry.Item2);
					}
					else
					{
						if ((existing is FormFiles) && entry.Item2 is FormFiles)
						{
							FormFiles existingFiles = (FormFiles)existing;
							FormFiles newFiles = (FormFiles)entry.Item2;
							result[entry.Item1] = new FormFiles(existingFiles.Files.Concat(newFiles.Files));
						}
					}
				}
			}
			return result;
		}

		private static async Task<Tuple<string, FormValue>> ParseMultipartProperty(string barrier, StreamReader reader)
		{
			Dictionary<string, KeyValuePair<string, Dictionary<string, string>>> headers = new Dictionary<string, KeyValuePair<string, Dictionary<string, string>>>();
			string dispositionType = "";
			string contentType = "";
			string dispositionName = "";
			string filename = "";
			while (!reader.EndOfStream)
			{
				string line = await reader.ReadLineAsync();

				if (string.IsNullOrEmpty(line))
				{
					if (string.IsNullOrEmpty(filename))
					{
						FormString variable = await ParseMultipartPropertyValue(barrier, reader);
						return new Tuple<string, FormValue>(dispositionName, variable);
					}
					else
					{
						byte[] variable = await ParseMultipartPropertyFile(barrier, reader);
						FormFiles files = new FormFiles(new FileUpload[] { new FileUpload(filename, variable) });
						return new Tuple<string, FormValue>(dispositionName, files);
					}
				}
				else
				{
					Match header = HeaderStartRegex.Match(line);
					if (header.Success)
					{
						string headerName = header.Groups[1].Value;
						string value = header.Groups[2].Value;

						string remainder = header.Groups[3].Value;
						Dictionary<string, string> args = new Dictionary<string, string>();
						foreach (Match match in HeaderRegex.Matches(remainder))
						{
							var argKey = match.Groups["name"].Value;
							var argValue = match.Groups["value"].Value;
							args.Add(argKey, argValue);
						}
						if (headerName.Equals("content-disposition", StringComparison.OrdinalIgnoreCase))
						{
							dispositionType = value;
							args.TryGetValue("name", out dispositionName);
							args.TryGetValue("filename", out filename);
						}
						else if (headerName.Equals("content-type", StringComparison.OrdinalIgnoreCase))
						{
							contentType = value;
						}
						headers.Add(headerName, new KeyValuePair<string, Dictionary<string, string>>(value, args));
					}
				}
			}
			return null;
		}

		private static async Task<FormString> ParseMultipartPropertyValue(string barrier, StreamReader reader)
		{
			string content = "";
			while (!reader.EndOfStream)
			{
				string line = await reader.ReadLineAsync();
				if (line.StartsWith(barrier))
				{
					break;
				}
				content += line;
			}
			return new FormString(content);
		}

		private static async Task<byte[]> ParseMultipartPropertyFile(string barrier, StreamReader reader)
		{
			return await ReadUntil(reader, barrier, outputTarget: false);
		}

		private static async Task<byte[]> ReadUntil(StreamReader input, string target, bool outputTarget)
		{
			List<byte> userdata = new List<byte>();
			int headstart = 0;
			while (true)
			{
				char[] next = new char[1];
				int read = await input.ReadAsync(next, 0, 1);
				if (read == 0)
				{
					continue;
				}

				if (target[headstart] == next[0])
				{
					headstart++;
					if (headstart >= target.Length)
					{
						if (outputTarget)
						{
							userdata.AddRange(input.CurrentEncoding.GetBytes(target.ToCharArray()));
						}
						break;
					}
				}
				else
				{
					userdata.AddRange(input.CurrentEncoding.GetBytes(target.ToCharArray(), 0, headstart));
					userdata.AddRange(input.CurrentEncoding.GetBytes(next));
				}
			}
			return userdata.ToArray();
		}
	}
}