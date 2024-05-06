using Microsoft.Win32;
using OSUBmDl.Utils;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Web;
using OSUBmDl.Downloader;
using System.Diagnostics;

namespace OSUBmDl
{
    public class Program
    {
        static string ASSEMBLY_LOC = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string CONFIG_PATH;

        static void Init()
        {
            //SetConsole TopMost = true
            ConfigureConsole.SetTopMost();

            //Ensure ASSEMBLY_LOC points to the program .exe
            if (ASSEMBLY_LOC.EndsWith(".dll"))
                ASSEMBLY_LOC = ASSEMBLY_LOC.Replace(".dll", ".exe");

            //Set config path
            CONFIG_PATH = Path.Combine(Path.GetDirectoryName(ASSEMBLY_LOC), "cfg.json");
            //Console.WriteLine("Config Path: {0}", CONFIG_PATH);

            //CheckRegistry on FirstRun
            if (ConfigManager.Load(CONFIG_PATH).firstRun)
                Reg.SetUp(ConfigManager.Load(CONFIG_PATH), ASSEMBLY_LOC, CONFIG_PATH);
        }        

        static void Main(string[] args)
        {
            Init(); //Init Program

            //Get raw data as string
            string data = string.Join(" ", args);
            data = HttpUtility.UrlDecode(data);

#if DEBUG //Debug string
            data = "://1767295/";
#endif

            //Show config page if data is null
            if (string.IsNullOrEmpty(data))
            {
                Configure();
                return;
            }

            //Ensure data contais http-like syntax
            if (!data.Contains("://")) 
                return;

            //Get content Index
            int Index0 = data.IndexOf("://") + 3;
            int Index1 = data.LastIndexOf("/");

            //avoid errors if string dosen't end with '/'
            if (Index1 == Index0 - 1)
                Index1 = data.Length;


            /*    *Split in args array: (id; data)*
            string[] dataArr = data.Substring(Index0, Index1 - Index0).Split("==");
            
            string id = dataArr[0];
            string name = dataArr[1];
            */

            string id = data.Substring(Index0, Index1 - Index0);

            bool autoInstall = ConfigManager.Load(CONFIG_PATH).autoInstall;
            string saveDir = Path.GetTempPath();
            
            //Set manual filePath+name
            //string savePath = Path.Combine(saveDir, name + ".osk");


            //Star downloader
            var beatmapDownloader = new BeatmapDownloader();

            beatmapDownloader.DownloadAsync(id, saveDir, autoInstall)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Console.Write("\nBeatmap Installed Succesfully!\n");

            
            Thread.Sleep(2500);
        }

        

        private static void Configure()
        {
            bool ainstall = ConfigManager.Load(CONFIG_PATH).autoInstall;
            Console.WriteLine($"Auto Install: {ainstall}");

            int index = Console.WindowHeight-1;
            Console.SetCursorPosition(0, index);
            Console.BufferHeight = index+1;

            Console.Write($@"{(ainstall ? "Disable" : "Enable")} Auto Install? (y;n)");

            var cfg = new Config();
            ConsoleKey k;
            while ((k = Console.ReadKey(true).Key) != ConsoleKey.Y || k != ConsoleKey.N)
            {
                switch (k)
                {
                    case ConsoleKey.Y:
                        cfg.autoInstall = !ainstall;
                        ConfigManager.Save(CONFIG_PATH, cfg);
                        return;

                    default:return;
                }
            }
        }
    }
}
