using Stellamod.Common.Shaders;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.EffectTiles
{
    public class FogSpawnerWallItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("This is a modded wall.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 400;
        }
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<FogSpawnerWall>();
        }
    }

    public class FogSpawnerWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(1, 1, 1));
        }

        public override bool CanExplode(int i, int j) => false;
        public override bool Drop(int i, int j, ref int type)
        {
            return false;
        }
        /*
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            LunarLightingRenderer fogSystem = ModContent.GetInstance<LunarLightingRenderer>();
            Point point = new Point(i, j);
            Fog fog = fogSystem.SetupFog(point, FogCreateFunction);
            fog.updateFunc = FogUpdateFunction;
            fog.shaderFunc = FogShaderFunction;
            return false;
        }

        public virtual void FogCreateFunction(Fog fog)
        {

            fog.startColor = Color.White;
            fog.startScale = new Vector2(Main.rand.NextFloat(0.75f, 1.0f), Main.rand.NextFloat(0.7f, 0.9f)) * 0.9f;
            fog.pulseWidth = Main.rand.NextFloat(0.96f, 0.98f);
            fog.texture = TextureRegistry.Clouds6;
            fog.rotation = Main.rand.NextFloat(-1f, 1f);
            fog.offset = Main.rand.NextVector2Circular(16, 16);
        }


        public virtual void FogUpdateFunction(Fog fog)
        {

        }

        public virtual BaseShader FogShaderFunction()
        {
            var fogShader = FogShader.Instance;
            fogShader.FogTexture = TextureRegistry.Clouds6;
            fogShader.ProgressPower = 0.75f;
            fogShader.EdgePower = 1f;
            fogShader.Speed = 1f;
            fogShader.Apply();
            return fogShader;
        }*/
    }
}
