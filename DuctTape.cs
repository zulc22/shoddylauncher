using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

public static class DuctTape
{
    public static string TempDir()
    {
        string temp = Path.GetTempPath();
        temp = Path.Combine(temp, "shoddylauncher");
        if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);
        return temp;
    }

    public static void Extract7zBinaries()
    {
        File.WriteAllBytes(SevenzPath(), ShoddyLauncher.Properties.Resources.exe_7z);
        File.WriteAllBytes(Path.Combine(SevenzPath(), @"..\7z.dll"), ShoddyLauncher.Properties.Resources.dll_7z);
    }

    public static string SevenzPath() {
        return TempDir() + @"\7z.exe";
    }

    public static Process NewSevenz()
    {
        Process _7z = new Process();
        _7z.StartInfo.FileName = SevenzPath();
        _7z.StartInfo.UseShellExecute = false;
        _7z.StartInfo.RedirectStandardOutput = true;
        _7z.StartInfo.CreateNoWindow = true;
        return _7z;
    }

    public static List<string> ListArchive(string filePath)
    {
        Process _7z = NewSevenz();
        _7z.StartInfo.Arguments = "l -bd \"" + filePath + "\"";
        _7z.Start();
        string list_output = _7z.StandardOutput.ReadToEnd();
        string[] lines = list_output.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        if (lines[3].EndsWith("Can not open file as archive"))
        {
            return null;
        }
        // Collect file list from archive
        int linesUntil = -1;
        List<string> filesInArchive = new List<string>();
        foreach (string line in lines)
        {
            if (linesUntil > 0) linesUntil--;
            if (line.StartsWith("   Date"))
            {
                linesUntil = 2;
            }
            if (linesUntil != 0) continue;
            // The file list table has ended
            if (line.StartsWith("-------------------"))
            {
                break;
            }
            // File list table has started
            string filename = line.Substring(53);
            filesInArchive.Add(filename);
        }
        return filesInArchive;
    }

    public static string AhkPath()
    {
        return TempDir() + @"\AutoHotkey.exe";
    }

    public static void ExtractAhkBinary()
    {
        File.WriteAllBytes(AhkPath(), ShoddyLauncher.Properties.Resources.exe_AutoHotkeyInterpreter);
    }

    public static void ExecuteAhk(byte[] script, string args="")
    {
        string ahkPath = AhkPath();
        if (!File.Exists(ahkPath)) ExtractAhkBinary();
        File.WriteAllBytes(TempDir() + @"\temp.ahk", script);
        Process AhkInterpreter = new Process();
        AhkInterpreter.StartInfo.WorkingDirectory = TempDir();
        AhkInterpreter.StartInfo.FileName = ahkPath;
        AhkInterpreter.StartInfo.Arguments = "temp.ahk "+args;
        AhkInterpreter.Start();
        AhkInterpreter.WaitForExit();
    }
}
