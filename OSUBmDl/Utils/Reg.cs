using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace OSUBmDl.Utils
{
    public class Reg
    {
        private static bool IsElevated()
        {
            bool elevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                elevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return elevated;
        }

        private static void Register( RegistryKey key, string exePath)
        {
            if (!key.GetSubKeyNames().Contains("osubmdl"))
            {
                var osubmdl = key.CreateSubKey("osubmdl", true);
                osubmdl.SetValue("URL Protocol", "");

                var shell = osubmdl.CreateSubKey("shell", true);
                var open = shell.CreateSubKey("open", true);
                var command = open.CreateSubKey("command", true);

                command.SetValue("", exePath);
            }
        }

        private static void Verify(RegistryKey key, string exePath)
        {
            var _osubmdl = key.OpenSubKey("osubmdl", true);
            var _shell = _osubmdl.OpenSubKey("shell", true);
            var _open = _shell.OpenSubKey("open", true);
            var _command = _open.OpenSubKey("command", true);

            if (_command.GetValue("").ToString() != exePath)
            {
                _command.SetValue("", exePath);
            }
        }

        public static void SetUp(Config config, string ASSEMBLY_LOC, string CONFIG_PATH)
        {
            //Verify if program is elevated
            bool isElevated = IsElevated();
            
            if (isElevated)
            {
                string str = $"\"{ASSEMBLY_LOC}\" \"%1\"";

                RegistryKey key = Registry.ClassesRoot;

                Register(key, str);

                //Verify path location
                Verify(key, str);

                //Set firstRun to false in cfg
                config.firstRun = false;
                ConfigManager.Save(CONFIG_PATH, config);

                return;
            }

            //Exception if program is not elevated
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("This program requires admin rights on the first run!");

            Console.ReadLine();

            Environment.Exit(-2);            
        }
    }
}
