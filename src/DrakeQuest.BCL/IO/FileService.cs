using System.IO;

namespace DrakeQuest.IO
{
	internal class FileService : IFileService
	{
		public bool Exists(string path)
		{
			return File.Exists(path);
		}

		public FileStream Create(string path, int bufferSize)
		{
			return File.Create(path, bufferSize);
		}

		public FileStream Create(string path, int bufferSize, FileOptions fileOptions)
		{
			return File.Create(path, bufferSize, fileOptions);
		}
	}
}
