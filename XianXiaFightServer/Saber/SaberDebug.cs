using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Email;
using XianXiaFightGameServer.Tool;
using XianXiaFightServer.Tool;

namespace Saber
{

        public static class SaberDebug
        {
            const ConsoleColor originColor = ConsoleColor.White;
            public static string Color(string s, ConsoleColor consoleColor)
            {
                return $"^!{(ushort)consoleColor}~{s}~^";
            }
            private static void ReadColorLog(string s)
            {
                try
                {
                    int index = s.IndexOf('~');

                    ConsoleColor consoleColor = (ConsoleColor)int.Parse(s.Substring(2, index - 2));
                    s = s.Substring(index + 1, s.Length - index - 3);
                    Console.ForegroundColor = consoleColor;
                    Console.Write(s);
                    Console.ForegroundColor = originColor;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ReadColorError,Reson:{ex.Message}");
                }
            }
            private static void _Log(string s, ConsoleColor baseColor = ConsoleColor.White)
            {

                Console.Write($"[{DateTime.Now.ToString("G")}]");
                int endIndex;
                int startIndex;
                while (s.Length > 0)
                {
                    endIndex = s.IndexOf("~^");
                    startIndex = s.IndexOf("^!");

                    if (startIndex < endIndex && startIndex >= 0 && endIndex >= 0)
                    {
                        if (startIndex > 1)
                        {
                            Console.ForegroundColor = baseColor;
                            Console.Write(s.Substring(0, startIndex - 1));
                            Console.ForegroundColor = originColor;
                        }
                        ReadColorLog(s.Substring(startIndex, endIndex - startIndex + 2));
                        if (s.Length <= endIndex + 2) break;
                        s = s.Substring(endIndex + 2);
                    }
                    else
                    {
                        Console.ForegroundColor = baseColor;
                        Console.Write(s);
                        Console.ForegroundColor = originColor;
                        break;
                    }
                }
                Console.Write('\n');
                //int index=s.Contains
            }
            public static void LogError(string s, ConsoleColor baseColor = ConsoleColor.White)
            {
                _Log($"{Color("[Error]:", ConsoleColor.Red)}{s}", baseColor);
            if(OSPlatformUtility.MyPlatformTarget==OSPlatformUtility.PlatformTarget.Linux)
                MailUtility.SendToDefault("[Error]", s);
            }
            public static void LogWarning(string s, ConsoleColor baseColor = ConsoleColor.White)
            {
                _Log($"{Color("[Warning]:", ConsoleColor.Yellow)}{s}", baseColor);
            }
            public static void Log(string s, ConsoleColor baseColor = ConsoleColor.White)
            {
                _Log($"{Color("[Log]:", ConsoleColor.Green)}{s}", baseColor);
            }
        }
    }

