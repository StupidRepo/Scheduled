using BepInEx.Logging;
using FishNet.Object;
using UnityEngine;
using Logger = BepInEx.Logging.Logger;

namespace Scheduled.Components;

public class VoiceChatManager: NetworkBehaviour
{
	private readonly ManualLogSource logger = Logger.CreateLogSource("VoiceChatManager");
	
	public override void OnStartNetwork()
	{
		base.OnStartNetwork();
		logger.LogInfo("OnStartNetwork");
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		logger.LogInfo("OnStartServer");
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		logger.LogInfo("OnStartClient");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			logger.LogWarning("Call stack started!");
			DoRPCStack();
		}
	}

	public void DoRPCStack()
	{
		CallServer();
		logger.LogError("Called server!");
	}

	[ServerRpc(RequireOwnership = false)]
	void CallServer()
	{
		CallClient();
		logger.LogError("Server called!");
	}

	[ObserversRpc]
	void CallClient()
	{
		logger.LogError("Client called!");
	}
}