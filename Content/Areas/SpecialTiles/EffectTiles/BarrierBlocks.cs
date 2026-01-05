using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Foggy;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.EffectTiles
{
    public class BarrierBlockSystem : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Player.DryCollision += PreDryCollision;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Player.DryCollision -= PreDryCollision;
        }
        public static Vector2 BossArenaCenter;

        private bool GetNearestBarrierBlock(Player player, out Vector2 worldPoint)
        {
            Vector2 cameraCenterWorld = player.Center;
            Vector2 cameraTopLeft = cameraCenterWorld;// - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld; // + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

            const float range = 64;
            cameraTopLeft -= new Vector2(range);
            cameraBottomRight += new Vector2(range);

            Point topLeftTile = cameraTopLeft.ToTileCoordinates();
            Point bottomRightTile = cameraBottomRight.ToTileCoordinates();

            Vector2 nearest = Vector2.Zero;
            float nearestDistance = 9999f;
            bool success = false;
            for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
            {
                for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;
                    Tile tile = Main.tile[x, y];
                    if (tile.TileType != ModContent.TileType<BossBarrierBlock>())
                        continue;

                    Point tilePoint = new Point(x, y);
                    Vector2 position = tilePoint.ToWorldCoordinates();
                    float distToPoint = Vector2.Distance(player.Center, position);
                    if(distToPoint < nearestDistance)
                    {
                        nearest = position;
                        nearestDistance = distToPoint;
                        success = true;
                    }
                }
            }
            
            worldPoint = nearest;
            return success;
        }
        private void PreDryCollision(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            int barrierBlockType = ModContent.TileType<BossBarrierBlock>();
            Player player = Main.LocalPlayer;
            if(NPC.AnyDanger() && GetNearestBarrierBlock(player, out Vector2 worldPoint))
            {
                Vector2 tileDirectionToBoss = (BossArenaCenter - worldPoint).SafeNormalize(Vector2.Zero);
                Vector2 tileDirectionToPlayer = (player.Center - worldPoint).SafeNormalize(Vector2.Zero);
                //Need to check if the vectors are within 180 degrees of each other, if not then well you can walk through
                float dp = Vector2.Dot(tileDirectionToBoss, tileDirectionToPlayer);
                if(dp < 0)
                {
                    Main.tileSolid[barrierBlockType] = false;
                }
            }
            orig(self, fallThrough, ignorePlats);
        }
        public override void PostUpdatePlayers()
        {
            base.PostUpdatePlayers();
            Main.tileSolid[ModContent.TileType<BossBarrierBlock>()] = NPC.AnyDanger();
            Main.tileSolid[ModContent.TileType<StarrVeriplantBarrierBlock>()] = !DownedBossTracker.IsDowned(DownedBossFlag.StoneGolem);
            Main.tileSolid[ModContent.TileType<STARBOMBERBarrierBlock>()] = !DownedBossTracker.IsDowned(DownedBossFlag.StarBomber);
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

    public class STARBOMBERBarrierBlockItem : ModItem
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
            Item.createTile = ModContent.TileType<STARBOMBERBarrierBlock>();
        }
    }

    public class STARBOMBERBarrierBlock : BaseBarrierBlock
    {

    }
}
