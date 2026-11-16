using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ExwhyzeeSP.Web.Areas.Service
{
    public class Logger
    {
        private const string IDENTITY = @"HKEY_LOCAL_MACHINE\Ident";
        private const string NAME = "Name";

        public static void log(string name, string path)
        {
            try
            {
                string DeviceName = "Server";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    try
                    {
                        var regVal = Microsoft.Win32.Registry.GetValue(IDENTITY, NAME, "NAME");
                        if (regVal != null)
                        {
                            DeviceName = regVal.ToString();
                        }
                    }
                    catch
                    {
                        // Ignore registry errors on Windows
                    }
                }

                string pcName = Environment.MachineName;
                var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data");
                var fileContents = Path.Combine(logDirectory, "Logger.txt");

                if (File.Exists(fileContents))
                {
                    using (TextWriter sw = new StreamWriter(fileContents, true))
                    {
                        sw.WriteLine(Environment.NewLine + "**username**" + name + " *****path** " + path + " *****date** " + DateTime.UtcNow + " ****device name** " + DeviceName + " ****pc name*** " + pcName + "<br/>");
                        sw.Close();
                    }
                }
            }
            catch
            {
                // Never allow logging to crash view rendering or web requests
            }
        }
    }
}
