using System;
using System.IO;

public static class DuctTape
{
    public static string TempDir()
    {
        string temp = Path.GetTempPath();
        temp = Path.Combine(temp, "shoddylauncher");
        if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);
        return temp;
    }
}
