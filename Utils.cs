using ScheduleOne.DevUtilities;
using ScheduleOne.UI.MainMenu;

namespace Scheduled;

public static class Utils
{
	public static void ShowError(string title, string message)
	{
		if (Singleton<MainMenuPopup>.InstanceExists) Singleton<MainMenuPopup>.Instance.Open(title, message, true);
	}
}