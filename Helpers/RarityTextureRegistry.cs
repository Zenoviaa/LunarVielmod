using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public class RarityTextureRegistry : ModSystem
    {
        public const string Path = "Stellamod/Particles/Sparkles/";

        public static Texture2D BaseRarityGlow
        {
            get;
            private set;
        }

        public static Texture2D BaseRaritySparkleTexture
        {
            get;
            private set;
        }
        public static Texture2D ThornedRarityGlow
        {
            get;
            private set;
        }

        public override void Load()
        {
            BaseRarityGlow = ModContent.Request<Texture2D>($"{Path}BaseRarityGlow", AssetRequestMode.ImmediateLoad).Value;
            BaseRaritySparkleTexture = ModContent.Request<Texture2D>($"{Path}BaseRaritySparkleTexture", AssetRequestMode.ImmediateLoad).Value;
            ThornedRarityGlow = ModContent.Request<Texture2D>($"{Path}ThornedRarityGlow", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void Unload()
        {
            BaseRarityGlow = null;
            BaseRaritySparkleTexture = null;
            ThornedRarityGlow = null;
        }
    }
}
