using Discord.Sdk;

namespace Scheduled;

internal class DiscordManager
{
	public const ulong CLIENT_ID = 1358163596479303941;
	
	private readonly Client DiscordClient;
	
	private AuthorizationCodeVerifier? Verifier;
	private string? VerifierCode;

	internal DiscordManager()
	{
		DiscordClient = new Client();
		
		DiscordClient.AddLogCallback(OnDiscordLog, LoggingSeverity.Error);
		DiscordClient.SetStatusChangedCallback(OnDiscordStatusChanged);
	}
	
	private void OnDiscordStatusChanged(Client.Status status, Client.Error error, int errorCode)
	{
		Plugin.Logger.LogInfo($"Discord SDK status changed: {status}");
		if (error != Client.Error.None) { Plugin.Logger.LogError("Discord SDK error: " + error); }

		if (status != Client.Status.Ready) return;
		Plugin.Logger.LogInfo($"Friend Count: {DiscordClient.GetRelationships().Length}");
	}

	private void StartOAuthFlow()
	{
		Verifier = DiscordClient.CreateAuthorizationCodeVerifier();
		VerifierCode = Verifier.Verifier();
		
		var args = new AuthorizationArgs();
		args.SetClientId(CLIENT_ID);
		args.SetScopes(Client.GetDefaultPresenceScopes());
		args.SetCodeChallenge(Verifier.Challenge());
		
		DiscordClient.Authorize(args, OnAuthorizeResult);
	}

	private void OnAuthorizeResult(ClientResult result, string code, string redirectUri) {
		Plugin.Logger.LogInfo($"Authorization result: [{result.Error()}] [{code}] [{redirectUri}]");
		if (!result.Successful()) {
			return;
		}

		if (string.IsNullOrEmpty(VerifierCode) || VerifierCode == null)
		{
			Plugin.Logger.LogError("VerifierCode was null?");
			return;
		}
		
		DiscordClient.GetToken(CLIENT_ID,
			code,
			VerifierCode,
			redirectUri,
			(result, token, refreshToken, tokenType, expiresIn, scope) =>
			{
				if (string.IsNullOrEmpty(token))
				{
					Plugin.Logger.LogError("Discord sent back an empty token! Something went wrong?");
					return;
				}
				
				Plugin.Logger.LogInfo("Obtained OAuth2 token, we can now setup Discord invites!");
				DiscordClient.UpdateToken(
					AuthorizationTokenType.Bearer, 
					token, 
					_result => { DiscordClient.Connect(); }
				);
			}
		);
	}
	
	private void OnDiscordLog(string message, LoggingSeverity severity)
	{
		Plugin.Logger.LogInfo($"Discord SDK [{severity}]: {message}");
	}
}