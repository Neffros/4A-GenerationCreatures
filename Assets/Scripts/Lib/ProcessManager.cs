using System.Diagnostics;
using System.IO;

public static class ProcessManager
{
    public static void Launch(string path)
    {
        if (File.Exists(path))
        {
            Process process = new Process();

            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = path;
            process.EnableRaisingEvents = true;
            process.Start();
            process.WaitForExit();
        }
    }
}
