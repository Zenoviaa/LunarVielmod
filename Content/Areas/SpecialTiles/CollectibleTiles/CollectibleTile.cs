using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.XixianFlaskSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.CollectibleTiles
{
    public class CollectibleDrawLayerSystem : ModSystem
    {
        public static Vector2 TileAdj => Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy ? Vector2.Zero : Vector2.One * 12;
        private int TileDrawWidth => Main.screenWidth / 16;
        private int TileDrawHeight => Main.screenHeight / 16;
        public override void Load()
        {
            base.Load();
            On_Main.DoDraw_WallsAndBlacks += DrawWalls;
        }
        public override void Unload()
        {
            base.Unload();
            On_Main.DoDraw_WallsAndBlacks -= DrawWalls;
        }

        private void DrawWalls(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            //Draw Behind the walls
            orig(self);
            DrawCollectibles();
        }

        private void DrawCollectibles()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            //Draw In Front Walls
            int width = TileDrawWidth;
            int height = TileDrawHeight;
            Point bottomLeft = Main.LocalPlayer.Center.ToTileCoordinates() - new Point(width / 2, height / 2);
            int left = bottomLeft.X;
            int right = left + width;
            int bottom = bottomLeft.Y;
            int top = bottom + height;
            for (int x = left; x < right; x++)
            {
                if (x < 0)
                    continue;
                if (x >= Main.maxTilesX)
                    continue;
                for (int y = bottom; y < top; y++)
                {
                    if (y >= Main.maxTilesY)
                        continue;
                    if (y < 0)
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                        continue;
                    var modTile = ModContent.GetModTile(tile.TileType);
                    if (modTile == null)
                        continue;
                    if (modTile is BaseCollectibleTile collectibleTile)
                    {
                        collectibleTile.Draw(x, y, spriteBatch);
                    }
                }
            }
        }
    }
    public abstract class BaseCollectibleTileItem : ModItem
    {
        public override string Texture => this.PathHere() + "/CollectibleItem";
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
            Item.value = 1;
        }
    }

    public abstract class BaseCollectibleTile : ModTile
    {
        public int CollectibleItem { get; set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileSolid[Type] = false;
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
            MinPick = 210;
        }

        public override bool CanDrop(int i, int j)
        {
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            base.ModifyLight(i, j, ref r, ref g, ref b);
            r += 1f;
            g += 1f;
            b += 1f;
        }

        public virtual void Draw(int i, int j, SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            Texture2D texture = TextureAssets.Tile[Type].Value;
            Vector2 drawPos = new Vector2(i, j) * 16;
            drawPos.Y += VectorHelper.Osc(-4f, 4f);
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(Lighting.GetColor(i, j));
            if (!CanCollect(player, drawPos))
            {
                drawColor = drawColor.MultiplyRGB(Color.Lerp(Color.White, Color.Black, 0.75f));
            }

            float drawRotation = VectorHelper.Osc(-0.2f, 0.2f, speed: 2);

            Vector2 tileCheckPos = new Vector2(i, j).ToWorldCoordinates();
            bool canCollect = CanCollect(player, tileCheckPos);

            if (canCollect)
            {
                if (Main.rand.NextBool(8))
                {
                    Dust.NewDustPerfect(tileCheckPos - Main.rand.NextVector2Circular(64, 64) - texture.Size() / 2, ModContent.DustType<GlowSparkleDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0,
                        Color.White, 0.5f).noGravity = true;
                }
                float o = 2;
                var shader = ShaderRegistry.MiscSilPixelShader;
                //The color to lerp to
                shader.UseColor(Color.White);

                //Should be between 0-1
                //1 being fully opaque
                //0 being the original color
                shader.UseSaturation(1f);

                // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
                shader.Apply(null);
                spriteBatch.Restart(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, effect: shader.Shader);
                spriteBatch.Draw(texture, drawPos - Main.screenPosition - Vector2.UnitX * o, null, drawColor, drawRotation, drawOrigin, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos - Main.screenPosition + Vector2.UnitX * o, null, drawColor, drawRotation, drawOrigin, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos - Main.screenPosition - Vector2.UnitY * o, null, drawColor, drawRotation, drawOrigin, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawPos - Main.screenPosition + Vector2.UnitY * o, null, drawColor, drawRotation, drawOrigin, 1f, SpriteEffects.None, 0);

                shader.UseColor(Color.Red * 0.3f);
                shader.Apply(null);
                spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Shader);
                for (float f = 0f; f < 1f; f += 0.2f)
                {
                    float rot = f * MathHelper.TwoPi;
                    rot += Main.GlobalTimeWrappedHourly;
                    Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(12, 16);
                    spriteBatch.Draw(texture, drawPos - Main.screenPosition + offset, null, Color.White, drawRotation, drawOrigin, 1f, SpriteEffects.None, 0);


                }
                spriteBatch.RestartDefaults();
            }

            spriteBatch.Draw(texture, drawPos - Main.screenPosition, null, drawColor, drawRotation, drawOrigin, 1f, SpriteEffects.None, 0);


            float distanceToPlayer = Vector2.Distance(player.Center, tileCheckPos);
            if (distanceToPlayer < 64 && canCollect)
            {
                Collect(player, tileCheckPos);
            }
        }

        public virtual bool CanCollect(Player player, Vector2 position)
        {
            return !player.HasItem(CollectibleItem);
        }

        public virtual void Collect(Player player, Vector2 position)
        {
            player.QuickSpawnItem(player.GetSource_FromThis(), CollectibleItem);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CollectSpecial"), player.position);
            for (float i = 0; i < 12; i++)
            {
                float rot = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(5f, 25f);
                var particle = FXUtil.GlowStretch(position, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.04f, 0.07f);
                particle.VectorScale *= 0.5f;
            }
        }
    }
    public class XixianFlaskCollectibleItem : BaseCollectibleTileItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<XixianFlaskCollectible>();
        }
    }

    public class XixianFlaskCollectible : BaseCollectibleTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CollectibleItem = ModContent.ItemType<XixianFlask>();
        }

        public override bool CanCollect(Player player, Vector2 position)
        {
            return !player.GetModPlayer<FlaskPlayer>().unlockedFlask;
        }
        public override void Collect(Player player, Vector2 position)
        {
            base.Collect(player, position);
            //   player.QuickSpawnItem(player.GetSource_FromThis(), ModContent.ItemType<HealthyInsource>());
        }
    }
    public class StaminaFlaskCollectibleItem : BaseCollectibleTileItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<StaminaFlaskCollectible>();
        }
    }

    public class StaminaFlaskCollectible : BaseCollectibleTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
          //  CollectibleItem = ModContent.ItemType<StaminaPotion>();
        }
    }
}
