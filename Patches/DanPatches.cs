using HarmonyLib;
using ScheduleOne.NPCs.CharacterClasses;
using ScheduleOne.Product.Packaging;
using ScheduleOne.UI.Shop;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(Dan))]
public class DanPatches
{
	[HarmonyPatch(nameof(Dan.Awake))]
	[HarmonyPostfix]
	public static void AwakePostfix(Dan __instance)
	{
		if (__instance.ShopInterface == null)
			return;

		__instance.ShopInterface.CreateListingUI(new ShopListing {
			name = "Box",
			Item = Plugin.AssetManager.GetAsset<PackagingDefinition>("BoxDef")
		});
	}
}