using System;
using System.Collections.Generic;
using System.IO;
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

        public override void Unload()
        {
            base.Unload();
            if (_colorAtlas == null)
                return;
            Main.QueueMainThreadAction(() =>
            {
                foreach (var kvp in _colorAtlas)
                {
                    kvp.Value?.Dispose();
                }
                _colorAtlas = null;
            });

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
                            string fileName = new FileInfo(file).Name; ;

                            Vector3[] palette = ReadPaletteVector3(stream);
                            Texture3D colorSpectrum = CreateColorSpectrumTexture(palette);

                            _colorAtlas.Add(fileName, colorSpectrum);
                        }
                    });

                }
            }
        }
        public static Vector3[] ReadPaletteVector3(Stream stream)
        {
            int lineNum = 1;
            int pal = 0;
            List<Vector3> palette = new List<Vector3>();

            using (var streamReader = new StreamReader(stream))
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
                        pal++;
                        palette.Add(new Vector3(r, g, b));
                    }
                    lineNum++;
                }
                return palette.ToArray();
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

        public static Vector3 RGBToLab(Color rgb)
        {
            Vector3 RGB = Vector3.Zero;


            void RGBThing(ref float v, float input)
            {
                float value = input / 255f;

                if (value > 0.04045f)
                    value = MathF.Pow(((value + 0.055f) / 1.055f), 2.4f);
                else
                {
                    value = value / 12.92f;
                }


                v = value * 100;
            }
            RGBThing(ref RGB.X, rgb.R);
            RGBThing(ref RGB.Y, rgb.G);
            RGBThing(ref RGB.Z, rgb.B);

            Vector3 XYZ = Vector3.Zero;

            float X = RGB.X * 0.4124f + RGB.Y * 0.3576f + RGB.Z * 0.1805f;
            float Y = RGB.X * 0.2126f + RGB.Y * 0.7152f + RGB.Z * 0.0722f;
            float Z = RGB.X * 0.0193f + RGB.Y * 0.1192f + RGB.Z * 0.9505f;


            XYZ.X = MathF.Round(X, 4);
            XYZ.Y = MathF.Round(Y, 4);
            XYZ.Z = MathF.Round(Z, 4);

            XYZ.X = XYZ.X / 95.047f;
            XYZ.Y = XYZ.Y / 100.0f;
            XYZ.Z = XYZ.Z / 108.883f;


            void XYZThing(ref float v)
            {
                if (v > 0.008856)
                    v = MathF.Pow(v, 0.3333333333333333f);
                else
                    v = (7.787f * v) + (16f / 116f);
            }


            XYZThing(ref XYZ.X);
            XYZThing(ref XYZ.Y);
            XYZThing(ref XYZ.Z);

            Vector3 Lab = Vector3.Zero;
            Lab.X = (116 * XYZ.Y) - 16;
            Lab.Y = 500 * (XYZ.X - XYZ.Y);
            Lab.Z = 200 * (XYZ.Y - XYZ.Z);

            Lab.X = MathF.Round(Lab.X, 4);
            Lab.Y = MathF.Round(Lab.Y, 4);
            Lab.Z = MathF.Round(Lab.Z, 4);
            return Lab;
        }
        public static float ColorDistance5(Color a, Color b)
        {
            float d = 0.3f * MathF.Pow(b.R - a.R, 2f) + 0.59f * MathF.Pow(b.G - a.G, 2f) + 0.11f * MathF.Pow(b.B - a.B, 2f);

            return d;
        }
        public static float ColorDistance4(Color a, Color b)
        {
            Vector3 lab1 = RGBToLab(a);
            Vector3 lab2 = RGBToLab(b);


            float d = MathF.Sqrt((MathF.Pow(lab1.X - lab2.X, 2) + MathF.Pow(lab1.Y - lab2.Y, 2) + MathF.Pow(lab1.Z - lab2.Z, 2)));
            return d;
        }
        public static float ColorDistance3(Color a, Color b)
        {
            float d = 0.3f * MathF.Pow(b.R - a.R, 2f) + 0.59f * MathF.Pow(b.G - a.G, 2f) + 0.11f * MathF.Pow(b.B - a.B, 2f);

            return d;
        }
        public static float Grayscale(Color rgb)
        {
            return (rgb.R * 0.3f + rgb.G * 0.59f + rgb.B * 0.11f);
        }

        public static float ColorDistance6(Color a, Color b)
        {
            return MathF.Abs(Grayscale(b) - (Grayscale(a)));
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
        public static float ColorDistance(Vector3 a, Vector3 b)
        {
            float ar = MathF.Abs(b.X - a.X);
            float ag = MathF.Abs(b.Y - a.Y);
            float ab = MathF.Abs(b.Z - a.Z);
            float d = ar + ag + ab;
            return d;
        }
        public static Vector3 FindNearestColorInPalette(Vector3 originalColor, Vector3[] palette)
        {
            Vector3 selectedColor = palette[0];
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

                float diff = dist - currentDist;
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

                        Color newColor = new Color(r, g, b);
                        if (palette != null)
                        {
                            newColor = FindNearestColorInPalette(newColor, palette);
                        }

                        pixelsToSet[indexOfPixel] = newColor;
                    });
                }
            }

            colorSpectrumTexture.SetData(pixelsToSet);
            Console.WriteLine($"Created Color Spectrum, with color dimension {colorDimension}, {pixelsToSet.Length}");
            return colorSpectrumTexture;
        }
        public static Texture3D CreateColorSpectrumTexture(Vector3[] palette)
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

                        Vector3 rgb = new Vector3();
                        rgb.X = ((float)x / dimension) * 255f;
                        rgb.Y = ((float)y / dimension) * 255f;
                        rgb.Z = ((float)z / dimension) * 255f;


                        if (palette != null)
                        {
                            rgb = FindNearestColorInPalette(rgb, palette);
                        }

                        Color newColor = new Color((int)rgb.X, (int)rgb.Y, (int)rgb.Z);
                        pixelsToSet[indexOfPixel] = newColor;
                    });
                }
            }

            colorSpectrumTexture.SetData(pixelsToSet);
            Console.WriteLine($"Created Color Spectrum, with color dimension {colorDimension}, {pixelsToSet.Length}");
            return colorSpectrumTexture;
        }

    }
}
