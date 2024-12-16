using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Accessories;
using Stellamod.Particles;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class TorrientialLanceDashPlayer : ModPlayer
    {
        // These indicate what direction is what in the timer arrays used
        public int DashCooldown = 67; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public int DashDuration = 30; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public float DashVelocity = 10f;


        // The fields related to the dash accessory
        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 0; // frames remaining in the dash
        public int DashDir;

        public override void ResetEffects()
        {
            DashVelocity = 25f;
            DashDuration = 30;
            DashCooldown = 67;
        }

        // This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
        // If they double tapped this frame, they'll move fast this frame
        public override void PreUpdateMovement()
        {
            // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
            if (DashDir != 0 && Main.myPlayer == Player.whoAmI)
            {
                Vector2 newVelocity = Player.velocity;
                float dashDirection = DashDir == 1 ? 1 : -1;
                newVelocity.X = DashDir * DashVelocity;

                // start our dash
                DashDir = 0;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;

            }


            if (DashTimer > 0)
            {
                Player.velocity *= 0.98f;
                DashTimer--;
            }

        }
    }
    internal class TorrentialLanceProj : ModProjectile
    {
        private Vector2[] _oldPos;
        private ref float Timer => ref Projectile.ai[0];
        private bool SpawnBubbles;

        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float HoldOffset => 30;
        private float SwingTime => 60;
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 48;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            _oldPos = new Vector2[32];
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.timeLeft = (int)SwingTime;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {

            Timer++;
            if(SpawnBubbles && Timer % 8 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item85, Projectile.position);
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, 
                    ModContent.ProjectileType<TorrentialLanceBubbleProj>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner);
            }

            for (int i = _oldPos.Length - 1; i > 0; i--)
            {
                _oldPos[i] = _oldPos[i - 1];
            }

            if (_oldPos.Length > 0)
                _oldPos[0] = Owner.Center;


 
            AI_Immune();
            AI_AttachToPlayer();
            Visuals();
            if (Timer == 1)
            {
                Owner.GetModPlayer<TorrientialLanceDashPlayer>().DashDir = Owner.direction;
            }
        }


        private void AI_Immune()
        {
            if (Timer < 30)
            {
                int minImmuneTime = 5;
                Owner.immune = true;

                //Using Math.Max returns the highest value, so you won't override any existing immune times.
                //Just prevent it from going low
                Owner.immuneTime = Math.Max(Owner.immuneTime, minImmuneTime);

                //No clue why you need to set this, but if you don't you'll still sometimes take damage
                Owner.hurtCooldowns[0] = Math.Max(Owner.hurtCooldowns[0], minImmuneTime);
                Owner.hurtCooldowns[1] = Math.Max(Owner.hurtCooldowns[1], minImmuneTime);
                Owner.noKnockback = true;
            }
        }

        private void AI_AttachToPlayer()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
                return;

            // the actual rotation it should have
            float rotation = Projectile.velocity.ToRotation();

            // offsetted cuz sword sprite
            Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
            position += rotation.ToRotationVector2() * HoldOffset;
            Projectile.Center = position;
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi - MathHelper.PiOver4;
            }

            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = rotation * player.direction;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }


        private void Visuals()
        {


            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!SpawnBubbles)
            {
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(target.Center,
                        innerColor: Color.White,
                        glowColor: Color.CornflowerBlue,
                        outerGlowColor: Color.Black,
                        baseSize: Main.rand.NextFloat(0.06f, 0.12f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (float f = 0; f < 12; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.CornflowerBlue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                SoundEngine.PlaySound(SoundRegistry.BubbleIn, target.position);
            }

            SpawnBubbles = true;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = 36;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Aquamarine, Easing.SpikeOutExpo(completionRatio)) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 textureSize = texture.Size();
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            }

            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            TrailDrawer.DrawPrims(_oldPos, -Main.screenPosition, 255);

            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer.DrawPrims(_oldPos, -Main.screenPosition, 255);
            return true;
        }
    }
}
