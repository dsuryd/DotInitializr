namespace DotInitializr
{
	public class PersonalAccessTokenAuthenticationOptions
	{
		internal static readonly string SECTION = "DotInitializr_PATOptions";
		public string Username { get; set; } = null;
		public string PersonalAccessToken { get; set; } = null;
	}
}