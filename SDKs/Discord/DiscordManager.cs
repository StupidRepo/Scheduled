using Discord.Sdk;
using UnityEngine.Events;

namespace Scheduled;

public class DiscordManager
{
	private const ulong CLIENT_ID = 1358163596479303941;

	public readonly Client client;
	private string verificationCode;

	public Action<Client> OnReady;

	public DiscordManager()
	{
		client = new Client();
        
		client.AddLogCallback((message, severity) =>
		{
			Plugin.Logger.LogInfo($"Discord SDK [{severity}]: {message}");
		}, LoggingSeverity.Error);
		client.SetStatusChangedCallback(OnStatusChanged);
	}
    
	public void StartOAuthFlow() {
		var authorizationVerifier = client.CreateAuthorizationCodeVerifier();
		verificationCode = authorizationVerifier.Verifier();
        
		var args = new AuthorizationArgs();
		args.SetClientId(CLIENT_ID);
		args.SetScopes(Client.GetDefaultPresenceScopes());
		args.SetCodeChallenge(authorizationVerifier.Challenge());
		
		client.Authorize(args, OnAuthorizeResult);
	}
	
	private void OnAuthorizeResult(ClientResult result, string code, string redirectUri)
	{
		Plugin.Logger.LogInfo($"Discord authorization result: [{result.Error()}] [{code}] [{redirectUri}]");
		if (!result.Successful()) return;
        
		client.GetToken(CLIENT_ID,
			code,
			verificationCode,
			redirectUri,
			(result, token, refreshToken, tokenType, expiresIn, scope) =>
			{
				if (string.IsNullOrEmpty(token))
				{
					Plugin.Logger.LogError($"Failed to get token: {result.Error()}");
					return;
				}
				
				Plugin.Logger.LogInfo("Obtained token for Discord SDK!");
				client.UpdateToken(AuthorizationTokenType.Bearer, token, _ => { client.Connect(); });
			}
		);
	}

	private void OnStatusChanged(Client.Status status, Client.Error error, int errorCode)
	{
		if (error != Client.Error.None)
		{
			Plugin.Logger.LogError($"Error: {error}, code: {errorCode}");
			return;
		}

		if (status != Client.Status.Ready) return;
		
		Plugin.Logger.LogInfo("Discord SDK is ready!");
		OnReady.Invoke(client);
	}
}