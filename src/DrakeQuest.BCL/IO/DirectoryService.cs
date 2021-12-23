using System.IO;

namespace DrakeQuest.IO
{
	internal class DirectoryService : IDirectoryService
	{
		public bool Exists(string path)
		{
			return Directory.Exists(path);
		}

		public string[] GetFiles(string path, string searchPattern)
		{
			return Directory.GetFiles(path, searchPattern);
		}

		public DirectoryInfo CreateDirectory(string path)
		{
			return Directory.CreateDirectory(path);
		}
	}
}
