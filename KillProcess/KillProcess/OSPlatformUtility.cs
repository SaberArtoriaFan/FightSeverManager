using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianXiaFightGameServer.Tool
{
    internal static class OSPlatformUtility
    {
        public enum PlatformTarget
        {
            Windows,
            Linux,
            MaxOsX
        }
        private static PlatformTarget _platformTarget=Init();

        internal static PlatformTarget MyPlatformTarget { get => _platformTarget;}

        static PlatformTarget Init()
        {
            string windir = Environment.GetEnvironmentVariable("windir");
            PlatformTarget platformTarget=default;
            if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
            {
               platformTarget = PlatformTarget.Windows;
            }
            else if (File.Exists(@"/proc/sys/kernel/ostype"))
            {
                string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
                {
                    // Note: Android gets here too
                    platformTarget=PlatformTarget.Linux;
                }
                else
                {
                    //throw new UnsupportedPlatformException(osType);
                }
            }
            else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
            {
                // Note: iOS gets here too
                platformTarget= PlatformTarget.MaxOsX;
            }
            else
            {
                //throw new UnsupportedPlatformException();
            }
           Console.WriteLine($"TargetPlatform is {platformTarget}");


            return platformTarget;
        }
    }
}
