using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotInitializr
{
	public class GitConfigurableTemplateSource : IConfigurableTemplateSource
	{
		private static readonly List<string> _ignoreFiles = new List<string>
	  {
		 Path.DirectorySeparatorChar + ".git",
		 Path.AltDirectorySeparatorChar + ".git"
	  };

		public string SourceType => "git";

		public TemplateFile GetFile(string fileName, string sourceUrl, string sourceDirectory = null, string sourceBranch = null, string personalAccessToken = null)
		{
			TemplateFile result = null;
			string tempPath = Path.Combine(Path.GetTempPath(), nameof(DotInitializr), Guid.NewGuid().ToString());

			if (sourceDirectory != null)
				sourceDirectory = sourceDirectory.Replace('\\', Path.DirectorySeparatorChar);

			try
			{
				CloneOptions cloneOptions;

				if (string.IsNullOrEmpty(personalAccessToken))
				{
					cloneOptions = new CloneOptions { CredentialsProvider = (url, user, cred) => new DefaultCredentials(), BranchName = sourceBranch };
				}
				else
				{
					var creds = new UsernamePasswordCredentials
					{
						Username = "Stephen-Jong_enlyte",
						Password = "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"
					};
				}

				var cloneOptions = new CloneOptions { CredentialsProvider = (url, user, cred) => creds, BranchName = sourceBranch };
				if (!string.IsNullOrEmpty(Repository.Clone(sourceUrl, tempPath, cloneOptions)))
				{
					var filePath = string.IsNullOrEmpty(sourceDirectory) ? fileName : Path.Combine(sourceDirectory, fileName);

					using var repo = new Repository(tempPath);
					repo.CheckoutPaths(repo.Head.FriendlyName, new string[] { filePath }, new CheckoutOptions());

					result = new TemplateFile
					{
						Name = filePath,
						Content = File.ReadAllText(Path.Combine(tempPath, filePath))
					};
				}
			}
			catch (Exception ex)
			{
				var error = $"Failed to get file `{fileName}` from `{sourceUrl}`";
				Console.WriteLine(error + Environment.NewLine + ex.ToString());
				throw new TemplateException(error, ex);
			}

			Utils.DeleteDirectory(tempPath);
			return result;
		}

		public IEnumerable<TemplateFile> GetFiles(string sourceUrl, string sourceDirectory = null, string sourceBranch = null, string personalAccessToken = null)
		{
			List<TemplateFile> result = new List<TemplateFile>();
			string tempPath = Path.Combine(Path.GetTempPath(), nameof(DotInitializr), Guid.NewGuid().ToString());

			if (sourceDirectory != null)
				sourceDirectory = sourceDirectory.Replace('\\', Path.DirectorySeparatorChar);

			try
			{
				string fullTempPath = Path.Combine(Path.GetFullPath(tempPath), sourceDirectory);
				var creds = new UsernamePasswordCredentials
				{
					Username = "Stephen-Jong_enlyte",
					Password = "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"
				};
				var cloneOptions = new CloneOptions { CredentialsProvider = (url, user, cred) => creds, BranchName = sourceBranch };

				if (!string.IsNullOrEmpty(Repository.Clone(sourceUrl, tempPath, cloneOptions)))
				{
					string filePath = string.IsNullOrEmpty(sourceDirectory) ? tempPath : Path.Combine(tempPath, sourceDirectory);
					foreach (var fileName in Directory.EnumerateFiles(filePath, "*", SearchOption.AllDirectories))
					{
						if (_ignoreFiles.Any(x => fileName.EndsWith(x)))
							continue;

						var templateFile = new TemplateFile
						{
							Name = fileName
							  .Replace(fullTempPath, string.Empty)
							  .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
							Content = File.ReadAllText(fileName)
						};

						if (IsBinary(templateFile.Content))
							templateFile = new TemplateFileBinary
							{
								Name = templateFile.Name,
								Content = templateFile.Content,
								ContentBytes = File.ReadAllBytes(fileName)
							};

						result.Add(templateFile);
					}
				}
			}
			catch (Exception ex)
			{
				var error = $"Failed to get files from `{sourceUrl}`";
				Console.WriteLine(error + Environment.NewLine + ex.ToString());
				throw new TemplateException(error, ex);
			}

			Utils.DeleteDirectory(tempPath);
			return result;
		}

		private static bool IsBinary(string content)
		{
			Func<char, bool> IsNonTextControlChar = (char c) => char.IsControl(c) && c != '\0' && c != '\r' && c != '\n' && c != '\t' && c != '\b' && c != '\v' && c != '\f' && c != 26;
			return content.Any(c => IsNonTextControlChar(c));
		}
	}
}
