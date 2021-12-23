using System.IO;

namespace DrakeQuest.IO
{


	public interface IDirectoryService
	{
		/// <summary>
		/// Determines whether the given path refers to an existing directory on disk.
		/// </summary>
		/// <param name="path">The path to test.</param>
		/// <returns>true if path refers to an existing directory; false if the directory does not exist or an error occurs when trying to determine if the specified file exists.</returns>
		bool Exists(string path);

		//
		// Summary:
		//     Returns the names of files (including their paths) that match the specified search
		//     pattern in the specified directory.
		//
		// Parameters:
		//   path:
		//     The relative or absolute path to the directory to search. This string is not
		//     case-sensitive.
		//
		//   searchPattern:
		//     The search string to match against the names of files in path. This parameter
		//     can contain a combination of valid literal path and wildcard (* and ?) characters
		//     (see Remarks), but doesn't support regular expressions.
		//
		// Returns:
		//     An array of the full names (including paths) for the files in the specified directory
		//     that match the specified search pattern, or an empty array if no files are found.
		//
		// Exceptions:
		//   T:System.IO.IOException:
		//     path is a file name.-or-A network error has occurred.
		//
		//   T:System.UnauthorizedAccessException:
		//     The caller does not have the required permission.
		//
		//   T:System.ArgumentException:
		//     path is a zero-length string, contains only white space, or contains one or more
		//     invalid characters. You can query for invalid characters by using System.IO.Path.GetInvalidPathChars.-or-
		//     searchPattern doesn't contain a valid pattern.
		//
		//   T:System.ArgumentNullException:
		//     path or searchPattern is null.
		//
		//   T:System.IO.PathTooLongException:
		//     The specified path, file name, or both exceed the system-defined maximum length.
		//     For example, on Windows-based platforms, paths must be less than 248 characters
		//     and file names must be less than 260 characters.
		//
		//   T:System.IO.DirectoryNotFoundException:
		//     The specified path is not found or is invalid (for example, it is on an unmapped
		//     drive).
		string[] GetFiles(string path, string searchPattern);

		/// <summary>Creates all directories and subdirectories in the specified path.</summary>
		/// <returns>A <see cref="T:System.IO.DirectoryInfo" /> as specified by <paramref name="path" />.</returns>
		/// <param name="path">The directory path to create. </param>
		/// <exception cref="T:System.IO.IOException">The directory specified by <paramref name="path" /> is a file .-or-The network name is not known.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.-or-<paramref name="path" /> is prefixed with, or contains only a colon character (:).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is null. </exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="path" /> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
		/// <filterpriority>1</filterpriority>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		/// </PermissionSet>
		DirectoryInfo CreateDirectory(string path);

	}
}
