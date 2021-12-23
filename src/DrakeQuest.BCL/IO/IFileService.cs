using System.IO;

namespace DrakeQuest.IO
{


	public interface IFileService
	{
		/// <summary>Determines whether the specified file exists.</summary>
		/// <returns>true if the caller has the required permissions and <paramref name="path" /> contains the name of an existing file; otherwise, false. This method also returns false if <paramref name="path" /> is null, an invalid path, or a zero-length string. If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the method returns false regardless of the existence of <paramref name="path" />.</returns>
		/// <param name="path">The file to check. </param>
		/// <filterpriority>1</filterpriority>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		/// </PermissionSet>true if path refers to an existing directory; false if the directory does not exist or an error occurs when trying to determine if the specified file exists.</returns>
		bool Exists(string path);

		/// <summary>Creates or overwrites a file in the specified path.</summary>
		/// <returns>A <see cref="T:System.IO.FileStream" /> that provides read/write access to the file specified in <paramref name="path" />.</returns>
		/// <param name="path">The path and name of the file to create. </param>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.-or- <paramref name="path" /> specified a file that is read-only. </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is null. </exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurred while creating the file. </exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="path" /> is in an invalid format. </exception>
		/// <filterpriority>1</filterpriority>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		/// </PermissionSet>
		FileStream Create(string path, int bufferSize);

		/// <summary>Creates or overwrites the specified file, specifying a buffer size and a <see cref="T:System.IO.FileOptions" /> value that describes how to create or overwrite the file.</summary>
		/// <returns>A new file with the specified buffer size.</returns>
		/// <param name="path">The name of the file. </param>
		/// <param name="bufferSize">The number of bytes buffered for reads and writes to the file. </param>
		/// <param name="options">One of the <see cref="T:System.IO.FileOptions" /> values that describes how to create or overwrite the file.</param>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.-or- <paramref name="path" /> specified a file that is read-only. -or-<see cref="F:System.IO.FileOptions.Encrypted" /> is specified for <paramref name="options" /> and file encryption is not supported on the current platform.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is null. </exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive. </exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurred while creating the file. </exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="path" /> is in an invalid format. </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.-or- <paramref name="path" /> specified a file that is read-only. </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.-or- <paramref name="path" /> specified a file that is read-only. </exception>
		FileStream Create(string path, int bufferSize, FileOptions fileOptions);

	}
}
