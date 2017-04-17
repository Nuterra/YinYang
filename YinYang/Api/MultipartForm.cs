using System;
using System.Collections.Generic;
using System.Linq;

namespace YinYang.Api
{
	public class FormValue
	{
		public string StringValue => (this as FormString)?.Value;
		public IEnumerable<FileUpload> FilesValue => (this as FormFiles)?.Files;
	}

	public sealed class FormString : FormValue
	{
		public string Value { get; }
		public FormString(string value)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			Value = value;
		}
	}

	public sealed class FormFiles : FormValue
	{
		public IEnumerable<FileUpload> Files { get; }

		public FormFiles(IEnumerable<FileUpload> files)
		{
			if (files == null) throw new ArgumentNullException(nameof(files));
			Files = files.ToArray();
		}
	}

	public sealed class FileUpload
	{
		public string FileName { get; }
		public byte[] Contents { get; }

		public FileUpload(string fileName, byte[] contents)
		{
			FileName = fileName;
			Contents = contents;
		}
	}
}