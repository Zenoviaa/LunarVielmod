using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Palettes
{
    [Autoload(Side = ModSide.Client)]
    public class PaletteHelper : ModSystem
    {
        private static Dictionary<string, Texture3D> _colorAtlas;
        public override void OnModLoad()
        {
            base.OnModLoad();
            LoadPalettes();
        }


        public static Texture3D GetColorSpectrum(string path)
        {
            return _colorAtlas[path];
        }

        public static void LoadPalettes()
        {
            _colorAtlas = new Dictionary<string, Texture3D>();
            Mod mod = Stellamod.Instance;
            foreach (var file in mod.GetFileNames())
            {
                if (file.Contains(".pal"))
                {
            
                    Main.QueueMainThreadAction(() =>
                    {
                        using (var stream = mod.GetFileStream(file))
                        {
                            Color[] palette = ReadPalette(stream);
                            Texture3D colorSpectrum = CreateColorSpectrumTexture(palette);

                            string fileName = new FileInfo(file).Name; ;

                            _colorAtlas.Add(fileName, colorSpectrum);
                            Console.WriteLine(fileName);
                        }
                    });

                }
            }
        }

        public static Color[] ReadPalette(Stream stream)
        {
            int lineNum = 1;
            List<Color> palette = new List<Color>();
            const Int32 BufferSize = 128;
            using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, BufferSize))
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


        /// <summary>
        /// Returns the distance between two colors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float ColorDistance(Color a, Color b)
        {
            float ar = MathF.Abs(b.R - a.R);
            float ag = MathF.Abs(b.G - a.G);
            float ab = MathF.Abs(b.B - a.B);
            float d = ar + ag + ab;
            return d;
        }

        /// <summary>
        /// Finds the closet color to the base color in the specified palette
        /// </summary>
        /// <param name="originalColor"></param>
        /// <param name="palette"></param>
        /// <returns></returns>
        public static Color FindNearestColorInPalette(Color originalColor, Color[] palette)
        {
            Color selectedColor = palette[0];
            float dist = ColorDistance(originalColor, selectedColor);
            float currentDist;

            // For loop with the same loops than the color palette.
            for (int i = 1; i < palette.Length; i++)
            {
                currentDist = ColorDistance(originalColor, palette[i]);
                //Branchless way to do this
                //We want to avoid using if-statements in shaders if possible, as creating branches GREATLY slows them down
                //We can evaluate a check like this to a 0 or 1, and since only 1 can be true we can invert it simply :) 
                if (currentDist < dist)
                {
                    selectedColor = palette[i];
                    dist = currentDist;
                }
            }

            return selectedColor;
        }

        public static Texture3D CreateColorSpectrumTexture(Color[] palette)
        {
            int colorDimension = 16;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            Texture3D colorSpectrumTexture = new Texture3D(graphicsDevice, colorDimension, colorDimension, colorDimension, false, SurfaceFormat.Color);

            float numColors = colorDimension * colorDimension * colorDimension;
            Color[] pixelsToSet = new Color[colorDimension * colorDimension * colorDimension];
            float dimension = colorDimension - 1;
            for (int x = 0; x < colorDimension; x++)
            {
                for (int y = 0; y < colorDimension; y++)
                {
                    Parallel.For(0, colorDimension, z =>
                    {
                        int indexOfPixel = z * colorDimension * colorDimension + y * colorDimension + x;

                        //Calculate RGB values based on the size of the texture

                        float r = x;
                        r /= dimension;

                        float g = y;
                        g /= dimension;

                        float b = z;
                        b /= dimension;


                        Color color = new Color(r, g, b);
                        if (palette != null)
                        {
                            color = FindNearestColorInPalette(color, palette);
                        }
                        pixelsToSet[indexOfPixel] = color;
                    });
                }
            }

            colorSpectrumTexture.SetData(pixelsToSet);
            Console.WriteLine($"Created Color Spectrum, with color dimension {colorDimension}, {pixelsToSet.Length}");
            return colorSpectrumTexture;
        }

    }
}
