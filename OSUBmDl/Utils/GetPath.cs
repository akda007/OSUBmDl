using System;
using System.Runtime.InteropServices;

namespace OSUBmDl.Utils
{
    public class GetPath
    {
        [DllImport("shell32",
        CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        private static extern string SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken
            );

        public static string DownloadsFolder()
        {
            return SHGetKnownFolderPath(
                new Guid("374DE290-123F-4565-9164-39C4925E467B"),
                0, IntPtr.Zero
                );
        }
    }
}
