using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Abyss.AccAB
{
    public class FastFlightPlayer : ModPlayer
    {
        private float _frameCounter;
        private int _frame;
        private float _frameSpeed;
        private float _wingTimer;
        public bool hasFastFlight;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasFastFlight = false;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (!hasFastFlight)
                return;
            _wingTimer++;
            if (_wingTimer % 7 == 0)
            {
              //  Dust.NewDustPerfect(Player.Center, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.White, Scale: 0.5f);
            }

            if (IsFlying())
            {
                _frameSpeed = 4;
                _frameCounter++;
                if (_frameCounter >= _frameSpeed)
                {
                    _frameCounter = 0;
                    _frame++;
                    if (_frame >= 8)
                    {
                        _frame = 0;
                    }
                }
            }
            else
            {
                if (_frame > 0)
                {
                    _frameCounter--;
                    if (_frameCounter <= 0)
                    {
                        _frameCounter = _frameSpeed;
                        _frame--;
                    }
                }
            }
        }

        private bool IsFlying()
        {
            return Player.controlJump && !Player.mount.Active && Player.wingTime > 0;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            if (drawInfo.shadow != 0f)
                return;
            if (!hasFastFlight)
                return;
            if (Player.dead)
                return;
            if (Player.GetModPlayer<MovePlayer>().eaten)
                return;

            float alpha = EasingFunction.InOutSine(_wingTimer / 60f);

            Asset<Texture2D> wingsTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/FastFlightProj");
            Asset<Texture2D> glowMaskWingsTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/FastFlightProj_GlowMask");
            Rectangle frame = wingsTextureAsset.Value.GetFrame(_frame, 8);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color glowColor = Color.White;
            glowColor *= alpha;
            glowColor.A = 0;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawScale = Vector2.One * 0.75f;
            Vector2 drawPosition = Player.Center - Main.screenPosition;
     
            drawPosition.Y -= 12;
            drawPosition.Y += Player.gfxOffY;

            //This draws it in persepctive
            drawScale.X *= 0.65f;
            drawPosition.X -= Player.direction * 12;
            drawPosition.Y += ExtraMath.Osc(0f, -6);
            Texture2D zuiTexyt = AssetManager.GlowMask.SimpleGlowCircle.Value;
            spriteBatch.Draw(glowMaskWingsTextureAsset.Value, drawPosition, frame, glowColor * 0.5f, 0, frame.Size() / 2f, drawScale * 1.2f, SpriteEffects.None, 0);
            //  spriteBatch.Draw(wingsTexture, drawPosition, frame, glowColor * 0.7f, 0, drawOrigin, drawScale, SpriteEffects.None, 0);

            Color wingColor = glowColor * 0.7f;
            wingColor.A = 0;
         //   wingColor *=
            var drawData = new DrawData(
                wingsTextureAsset.Value,
                drawPosition,
                frame,
                wingColor,
                0,
                frame.Size() * 0.5f,
                drawScale,
               SpriteEffects.None,
                0
            );
            drawData.shader = drawInfo.cWings;
            drawInfo.DrawDataCache.Add(drawData);
        }
    }

    [AutoloadEquip(EquipType.Wings)]
    public class FastFlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(14, 9f, 3);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f; // Falling glide speed
            ascentWhenRising = 0.15f; // Rising speed
            maxCanAscendMultiplier = 2;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<FastFlightPlayer>().hasFastFlight = true;
        }
    }
}
