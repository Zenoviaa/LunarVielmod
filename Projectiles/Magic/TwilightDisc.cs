using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class TwilightDisc : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float Style => ref Projectile.ai[1];
        private ref float SpinTimer => ref Projectile.ai[2];
        private Color MainColor
        {
            get
            {
                if (Style == 0)
                    return Color.LightPink;
                else
                    return Color.LightBlue;
            }
        }

        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.tileCollide = false;
        }
        private bool ShouldConsumeMana()
        {
            // Should mana be consumed this frame?
            bool consume = Timer % 18 == 0;
            return consume && Style == 0;
        }

        public override void AI()
        {
            base.AI();
            if (Style == 0)
            {
                Player player = Main.player[Projectile.owner];
                Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
                UpdatePlayerVisuals(player, rrp);
            }



            if (Main.myPlayer == Projectile.owner)
            {

                bool manaIsAvailable = !ShouldConsumeMana() || Owner.CheckMana(Owner.HeldItem.mana, true, false);

                // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                // player.channel indicates whether the player is still holding down the mouse button to use the item.
                bool stillInUse = Owner.channel && manaIsAvailable && !Owner.noItems && !Owner.CCed;
                if (stillInUse && Timer % 4 == 0)
                {

                }
                else if (!stillInUse)
                {
                    Projectile.Kill();
                }
            }

            Timer++;
            if(Timer % 15 == 0)
            {
                SoundStyle soundStyle = SoundRegistry.WindCast;
                soundStyle.Pitch = MathHelper.Lerp(0f, 1f, Timer / 120f);
                soundStyle.PitchVariance = 0.3f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            SpinTimer++;
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0,
                    MainColor, Main.rand.NextFloat(0.75f, 1.5f)).noGravity = true;
            }

            float speed = 2;
            float swingRange = MathHelper.TwoPi;
            float swingXRadius = 256;
            float swingYRadius = 32;
            float swingProgress = (SpinTimer / 30f);

            float chargeUpProgress = MathHelper.Clamp(Timer / 120f, 0f, 1f);

            float xOffset = swingXRadius * MathF.Sin(swingProgress * swingRange + swingRange);
            float yOffset = swingYRadius * MathF.Cos(swingProgress * swingRange + swingRange);
            Vector2 offset = new Vector2(xOffset, yOffset);
            Vector2 targetCenter = Owner.Center + offset * chargeUpProgress;
            Projectile.velocity = (targetCenter - Projectile.Center) * 0.5f;
        }
        private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {
            // The Prism is a holdout Projectile, so change the player's variables to reflect that.
            // Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // If you do not multiply by Projectile.direction, the player's hand will point the wrong direction while facing left.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 2.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(MainColor, Color.Transparent, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.DottedTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;


            Color drawColor = MainColor;
            drawColor.A = 0;
            Vector2 drawScale = Vector2.One;
            drawScale.X *= 1.2f;
            drawScale.Y *= 0.5f;
            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, drawScale * 0.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.Frostburn, 180);
            }
            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.LightPink,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.Blue, duration: Main.rand.NextFloat(12, 25), baseSize: Main.rand.NextFloat(0.03f, 0.06f));
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Parendine2"), target.position);
            ShakeModSystem.Shake = 4;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, MainColor, 0.5f).noGravity = true;
            }

            for (float f = 0; f < 4; f++)
            {
                float progress = f / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(25f, 35f);
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
                var particle = FXUtil.GlowStretch(target.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = MainColor;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                particle.VectorScale *= 0.5f;

            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Color glowColor = MainColor;
            glowColor.A = 0;
            glowColor *= 0.5f;
            for (int i = 0; i < 2; i++)
            {
                //   Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f) * 2f, SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
