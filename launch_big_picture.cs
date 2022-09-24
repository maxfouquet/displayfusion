using System;
using System.Drawing;

// The 'windowHandle' parameter will contain the window handle for the:
//   - Active window when run by hotkey
//   - Window Location target when run by a Window Location rule
//   - TitleBar Button owner when run by a TitleBar Button
//   - Jump List owner when run from a Taskbar Jump List
//   - Currently focused window if none of these match
public static class DisplayFusionFunction
{
	public static void Run(IntPtr windowHandle)
	{
		//replace these variables with ones that will work on your system
		string defaultProfile = "Default";
		string bigPictureProfile = "Big picture";
		string steamDirectory = "C:\\Program Files (x86)\\Steam\\steam.exe";
		const int waitForever = 0;
		
		// get current profile name
		string currentProfileName = BFS.DisplayFusion.GetCurrentMonitorProfile();
		if(currentProfileName == "")
			currentProfileName = defaultProfile;

		// close Steam if it is running
		uint steamID;
		bool wasSteamRunning = BFS.Application.IsAppRunningByFile(steamDirectory);
		if(wasSteamRunning)
		{
			steamID = BFS.Application.GetAppIDByFile(steamDirectory);
			BFS.Application.Kill(steamID);

			// wait for Steam to exit
			BFS.Application.WaitForExitByAppID(steamID, waitForever);
		}

		// wake up the monitors
		BFS.General.WakeMonitors();
		
		// load the big picture monitor profile and wait for it to switch
		BFS.DisplayFusion.LoadMonitorProfile(bigPictureProfile);
		string profileName = BFS.DisplayFusion.GetCurrentMonitorProfile();
		while(!profileName.Equals(bigPictureProfile, StringComparison.OrdinalIgnoreCase))
		{			
			BFS.General.ThreadWait(100);
			profileName = BFS.DisplayFusion.GetCurrentMonitorProfile();
		}

		// start Steam in Big Picture mode
		steamID = BFS.Application.Start(steamDirectory, "-bigpicture");

		// wait for Steam to exit
		BFS.Application.WaitForExitByAppID(steamID, waitForever);

		// restart Steam in normal mode
		if(wasSteamRunning)
			BFS.Application.Start(steamDirectory, "");

		// wake up the monitors
		BFS.General.WakeMonitors();
		
		// load the last running monitor profile and wait for it to switch
		BFS.DisplayFusion.LoadMonitorProfile(currentProfileName);
	}
}
