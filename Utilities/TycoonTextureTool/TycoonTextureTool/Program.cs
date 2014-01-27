using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace TycoonTextureTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TextureTool.Instance.WorkingDirectory = @"C:\Users\Innominate\Dropbox\Projects\FarmTycoon\FarmTycoon\bin\Debug\Data\DefaultTextures\";
            FileReader reader = new FileReader();
            reader.ReadTextures();

            if (Directory.Exists(TextureTool.Instance.TexturesDirectory) == false)
            {
                Directory.CreateDirectory(TextureTool.Instance.TexturesDirectory);
                reader.CreateIndividualImages(TextureSheet.Game, "texturemap.bmp");
                reader.CreateIndividualImages(TextureSheet.Window, "wintexturemap.bmp");
            }

            Application.Run(new MainWindow());
        }
    }
}
