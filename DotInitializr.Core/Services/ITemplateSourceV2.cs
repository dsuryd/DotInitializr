using System.Collections.Generic;

namespace DotInitializr
{
	public interface ITemplateSourceV2
	{
		/// <summary>
		/// Identifies the source repository type, e.g. "git".
		/// </summary>
		string SourceType { get; }

		/// <summary>
		/// Returns a file from a project template's source repository.<br/><br/>
		/// Provide a <paramref name="personalAccessToken"/> (PAT) and <paramref name="userName"/> for authentication or do not provide one for default (anonymous) authentication.
		/// </summary>
		/// <param name="fileName">File name to search.</param>
		/// <param name="sourceUrl">Source repository URL.</param>
		/// <param name="sourceDirectory">Directory in the source repository.</param>
		/// <param name="sourceBranch">Source repository branch.</param>
		/// <param name="personalAccessToken">personal access token for the repository provider</param>
		/// <returns>The template file.</returns>
		TemplateFile GetFile(string fileName, string sourceUrl, string sourceDirectory = null, string sourceBranch = null, string userName = null, string personalAccessToken = null);

		/// <summary>
		/// Returns all files from a project template's source repository.<br/><br/>
		/// Provide a <paramref name="personalAccessToken"/> (PAT) and <paramref name="userName"/> for authentication or do not provide one for default (anonymous) authentication.
		/// </summary>
		/// <param name="sourceUrl">Source repository URL.</param>
		/// <param name="sourceDirectory">Directory in the source repository.</param>
		/// <param name="sourceBranch">Source repository branch.</param>
		/// <param name="personalAccessToken">personal access token for the repository provider</param>
		/// <returns>All template files.</returns>
		IEnumerable<TemplateFile> GetFiles(string sourceUrl, string sourceDirectory = null, string sourceBranch = null, string userName = null, string personalAccessToken = null);
	}
}
