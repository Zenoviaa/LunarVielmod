using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.DialogueSystem
{
    public class PortraitLoader : ModSystem
    {
        public static Asset<Texture2D>[] PortraitAssets;
        public override void OnModLoad()
        {
            base.OnModLoad();
            PortraitAssets = new Asset<Texture2D>[32];
            string[] names = Enum.GetNames(typeof(PortraitType));
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                string fileName = name;
                LoadPortrait(i, fileName);
            }
        }

        private static void LoadPortrait(int i, string fileName)
        {
            string texturePath = typeof(PortraitLoader).DirectoryHere() + "/"+fileName;
            PortraitAssets[i] = ModContent.Request<Texture2D>(texturePath);
        }

        public static Asset<Texture2D> LoadPortrait(PortraitType type)
        {

            return PortraitAssets[(byte)type];
        }

        public static PortraitType NameToType(string name)
        {
            return Enum.Parse<PortraitType>(name);
        }
    }

    public enum PortraitType : byte
    {
        Bishinine,
        Daedus,
        Delgrim,
        Fenix,
        GardenerWilly,
        Gintzia,
        Gothivia,
        Lia,
        Irradia,
        Jack,
        ManMan,
        Merena,
        Mimi,
        Mordred,
        Ordin,
        Sanguimi,
        Sirestias,
        Sylia,
        Travoi,
        Veizal,
        Veldris,
        Verlia,
        Zui,
        Rysa,
        Gilatine,
        Jiitas,
        Minerva
    }
}
