using System.IO;

namespace DiscordRandomPicBot
{
    public class FromFolder
    {
        const string _folder = "E:\\photocards";

        public static string RandomFile()
        {
            string file = null;
            if (!string.IsNullOrEmpty(_folder))
            {
                var extensions = new string[] { ".png", ".jpg", "jpeg", ".gif", ".mp4", ".webm",};
                try
                {
                    var di = new DirectoryInfo(_folder);
                    var rgFiles = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                    Random R = new Random();
                    file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).FullName;
                }
                // probably should only catch specific exceptions
                // throwable by the above methods.
                catch { }
            }
            return file;
        }

        public static string RandomVideo()
        {
            string file = null;
            if (!string.IsNullOrEmpty(_folder))
            {
                var extensions = new string[] { ".mp4", ".webm", };
                try
                {
                    var di = new DirectoryInfo(_folder);
                    var rgFiles = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                    Random R = new Random();
                    file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).FullName;
                }
                // probably should only catch specific exceptions
                // throwable by the above methods.
                catch { }
            }
            return file;
        }

        public static string RandomGif()
        {
            string file = null;
            if (!string.IsNullOrEmpty(_folder))
            {
                var extensions = ".gif";
                try
                {
                    var di = new DirectoryInfo(_folder);
                    var rgFiles = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                    Random R = new Random();
                    file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).FullName;
                }
                // probably should only catch specific exceptions
                // throwable by the above methods.
                catch { }
            }
            return file;
        }

        public static string RandomPicture()
        {
            string file = null;
            if (!string.IsNullOrEmpty(_folder))
            {
                var extensions = new string[] { ".png", ".jpg", "jpeg",".webp", };
                try
                {
                    var di = new DirectoryInfo(_folder);
                    var rgFiles = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                    Random R = new Random();
                    file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).FullName;
                }
                // probably should only catch specific exceptions
                // throwable by the above methods.
                catch { }
            }
            return file;
        }
    }
}
