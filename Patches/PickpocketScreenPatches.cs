using FishNet;
using GameKit.Utilities;
using HarmonyLib;
using ScheduleOne;
using ScheduleOne.DevUtilities;
using ScheduleOne.ItemFramework;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using ScheduleOne.Storage;
using ScheduleOne.UI;
using ScheduleOne.UI.Items;
using Steamworks;
using UnityEngine;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(PickpocketScreen))]
public class PickpocketScreenPatches
{
	public static void MyOwnOpen(PickpocketScreen myself, NPC npc)
	{
		// myself.IsOpen = true;
		// myself.npc = npc;
		// myself.npc.SetIsBeingPickPocketed(true);
		// Singleton<GameInput>.Instance.ExitAll();
		// PlayerSingleton<PlayerInventory>.Instance.SetEquippingEnabled(false);
		// PlayerSingleton<PlayerMovement>.Instance.canMove = false;
		// PlayerSingleton<PlayerCamera>.Instance.SetCanLook(false);
		// PlayerSingleton<PlayerCamera>.Instance.AddActiveUIElement(myself.name);
		// PlayerSingleton<PlayerCamera>.Instance.FreeMouse();
		// PlayerSingleton<PlayerCamera>.Instance.SetDoFActive(true, 0.0f);
		// Singleton<ItemUIManager>.Instance.SetDraggingEnabled(true);
		// Singleton<HUD>.Instance.SetCrosshairVisible(false);
		// Player.Local.VisualState.ApplyState("pickpocketing", PlayerVisualState.EVisualState.Pickpocketing);
		//
		// ItemSlot[] array = npc.Inventory.ItemSlots.ToArray();
		// array.Shuffle();
		//
		// for (var index = 0; index < myself.Slots.Length; ++index)
		// {
		// 	if (index < array.Length)
		// 		myself.Slots[index].AssignSlot(array[index]);
		// 	else myself.Slots[index].ClearSlot();
		// }
		//
		// Singleton<ItemUIManager>.Instance.EnableQuickMove(
		// 	[..(IEnumerable<ItemSlot>)PlayerSingleton<PlayerInventory>.Instance.GetAllInventorySlots()], array.ToList()
		// );
		//
		// myself.isFail = false;
		// myself.isSliding = false;
		// myself.sliderPosition = 0.0f;
		// myself.slideDirection = 1;
		// myself.slideTimeMultiplier = 0f;
		// myself.Canvas.enabled = true;
		// myself.Container.gameObject.SetActive(true);
		
		// make new StorageEntity

		var storage = npc.gameObject.GetComponent<StorageEntity>();
		storage ??= npc.gameObject.AddComponent<StorageEntity>();
		InstanceFinder.ServerManager.Spawn(npc.gameObject);
		
		storage.StorageEntityName = $"{npc.FirstName}'s Inventory";
		storage.StorageEntitySubtitle = $"Currently unconscious. Take what you want.";

		List<ItemSlot> array = npc.Inventory.ItemSlots.ToList();
		array.Shuffle();

		storage.ItemSlots.Clear();
		for (var index = 0; index < storage.ItemSlots.Count; ++index)
		{
			if (index < array.Count)
				storage.ItemSlots.Add(array[index]);
		}
		
		storage.Open();
		storage.onContentsChanged.AddListener(() =>
		{
			Plugin.Logger.LogError("THINGY CHANGED!!!!!!!!!!!!!! AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
			npc.Inventory.ItemSlots = storage.ItemSlots;
		});
	}

	[HarmonyPatch(nameof(PickpocketScreen.Open))]
	[HarmonyPrefix]
	public static bool OpenPatch(PickpocketScreen __instance, NPC _npc)
	{
		if (_npc.IsConscious) return true;
		
		MyOwnOpen(__instance, _npc);
		return false;
	}
}