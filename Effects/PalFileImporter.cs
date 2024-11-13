using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria.ModLoader;

namespace Stellamod.Effects
{
    internal static class PalFileImporter
    {
        static Mod Mod = ModContent.GetInstance<Stellamod>();
        public static Color[] ReadPalette(string path)
        {
            int lineNum = 1;
            List<Color> palette = new List<Color>();
            const Int32 BufferSize = 128;
            using (var fileStream = Mod.GetFileStream(path + ".pal"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    // Process line
                    if (lineNum > 3)
                    {
                        //We have colors to parse!!!
                        string[] rgb = line.Split(null);
                        float r = float.Parse(rgb[0]);
                        float g = float.Parse(rgb[1]);
                        float b = float.Parse(rgb[2]);
                        Color color = new Color(r / 255f, g / 255f, b / 255f);
                        palette.Add(color);
                    }
                    lineNum++;
                }
                return palette.ToArray();
            }
        }
    }
}
