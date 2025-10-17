using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Foggy;
using Stellamod.Helpers;
using Stellamod.Core.Shaders;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.EffectTiles
{
    public class BarrierBlockSystem : ModSystem
    {

        private bool _hasLockedPlayerIn;
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();

            Player player = Main.LocalPlayer;
            //This should only run on the client :P
            if (!_hasLockedPlayerIn && NPC.AnyDanger())
            {
                //Raycast to see if straight shot to boss
                NPC boss = null;
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.boss)
                    {
                        boss = npc;
                    }
                }

                if (boss != null)
                {
                    //Raycast to this boss            
                    if (Collision.CanHitLine(player.position, 1, 1, boss.position, 1, 1))
                    {
                        _hasLockedPlayerIn = true;
                    }
                }
            }
            if (player.dead || !NPC.AnyDanger())
            {
                _hasLockedPlayerIn = false;
            }

            Main.tileSolid[ModContent.TileType<BossBarrierBlock>()] = _hasLockedPlayerIn;
            Main.tileSolid[ModContent.TileType<StarrVeriplantBarrierBlock>()] = !DownedBossSystem.downedStoneGolemBoss;
        }
    }

    public abstract class BaseBarrierBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(178, 163, 190), name);

            MineResist = 1f;
            MinPick = 145;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            FogSystem fogSystem = ModContent.GetInstance<FogSystem>();
            Point point = new Point(i, j);
            Fog fog = fogSystem.SetupFog(point, FogCreateFunction);
            fog.updateFunc = FogUpdateFunction;
            return false;
        }

        private void FogCreateFunction(Fog fog)
        {
            fog.shaderFunc = FogShaderFunction;
            fog.startColor = Color.Red;
            fog.startScale = new Vector2(Main.rand.NextFloat(0.75f, 1.0f), Main.rand.NextFloat(0.7f, 0.9f)) * 0.25f;
            fog.pulseWidth = Main.rand.NextFloat(0.96f, 0.98f);
            fog.texture = TextureRegistry.Clouds6;
            fog.rotation = Main.rand.NextFloat(-1f, 1f);
            fog.offset = Main.rand.NextVector2Circular(16, 16);
        }
        private void FogUpdateFunction(Fog fog)
        {
            bool isSolid = Main.tileSolid[Type];
            if (!isSolid)
            {
                fog.startColor = Color.Lerp(fog.startColor, Color.Transparent, 0.1f);
            }
            else
            {
                fog.startColor = Color.Lerp(fog.startColor, Color.Red, 0.1f);
            }
        }

        public BaseShader FogShaderFunction()
        {
            var shader = Fog2Shader.Instance;
            shader.FogTexture = TextureRegistry.Clouds6;
            shader.EdgePower = 0.5f;
            shader.ProgressPower = 1.5f;
            shader.Speed = 10f;
            shader.Apply();
            return shader;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);

            if (!tileAbove.HasTile || !tileBelow.HasTile)
            {
                r = 0.05f;
                g = 0.15f;
                b = 0.25f;
            }
        }
    }
    public class BossBarrierBlockItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<BossBarrierBlock>();
        }
    }

    public class BossBarrierBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(178, 163, 190), name);

            MineResist = 1f;
            MinPick = 145;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            FogSystem fogSystem = ModContent.GetInstance<FogSystem>();
            Point point = new Point(i, j);
            Fog fog = fogSystem.SetupFog(point, FogCreateFunction);
            fog.updateFunc = FogUpdateFunction;
            return false;
        }

        private void FogCreateFunction(Fog fog)
        {
            fog.shaderFunc = FogShaderFunction;
            fog.startColor = Color.Gray;
            fog.startScale = new Vector2(Main.rand.NextFloat(0.75f, 1.0f), Main.rand.NextFloat(0.7f, 0.9f)) * 0.25f;
            fog.pulseWidth = Main.rand.NextFloat(0.96f, 0.98f);
            fog.texture = TextureRegistry.Clouds6;
            fog.rotation = Main.rand.NextFloat(-1f, 1f);
            fog.offset = Main.rand.NextVector2Circular(16, 16);
        }
        private void FogUpdateFunction(Fog fog)
        {
            bool isSolid = Main.tileSolid[Type];
            if (!isSolid)
            {
                fog.startColor = Color.Lerp(fog.startColor, Color.Transparent, 0.1f);
            }
            else
            {
                fog.startColor = Color.Lerp(fog.startColor, Color.Gray, 0.1f);
            }
        }

        public BaseShader FogShaderFunction()
        {
            var shader = Fog2Shader.Instance;
            shader.FogTexture = TextureRegistry.Clouds6;
            shader.EdgePower = 0.5f;
            shader.ProgressPower = 1.5f;
            shader.Speed = 10f;
            shader.Apply();
            return shader;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);

            if (!tileAbove.HasTile || !tileBelow.HasTile)
            {
                r = 0.05f;
                g = 0.15f;
                b = 0.25f;
            }
        }
    }
    public class StarrVeriplantBarrierBlockItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<StarrVeriplantBarrierBlock>();
        }
    }

    public class StarrVeriplantBarrierBlock : BaseBarrierBlock
    {

    }
}
