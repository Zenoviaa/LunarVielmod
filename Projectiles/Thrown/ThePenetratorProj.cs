using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Effects;
using Stellamod.Helpers;
using Stellamod.Projectiles.Crossbows.Gemmed;
using Stellamod.Projectiles.Swords;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public class ThePenetratorProj : ModProjectile
    {
        private enum ActionState
        {
            Spin,
            Throw
        }

        ref float Timer => ref Projectile.ai[0];
        ref float Speed => ref Projectile.ai[1];
        ref float Sound => ref Projectile.ai[2];
        float SoundPitch;
        const float Charge_Time = 150;
        ActionState State;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;//number of frames the animation has
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 98;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = int.MaxValue;
            Projectile.localNPCHitCooldown = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ArmorPenetration = int.MaxValue;
        }

        public override bool? CanDamage()
        {
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            switch (State)
            {
                case ActionState.Spin:
                    if(Timer == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashCharge"), Projectile.position);
                    }

                    if(Timer < Charge_Time)
                    {
                        Timer++;
                        Speed += 0.005f;
                        if (Timer >= Charge_Time)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage1"), Projectile.position);
                        }
                    }

                    Sound++;
                    if (Sound % 15 == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SkyrageShasher") { Pitch = SoundPitch }, Projectile.position);
                        SoundPitch += 0.03f;
                    }

        
                    Vector2 playerCenter = player.MountedCenter;
            
                    Projectile.rotation += Speed;
                    Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position
                    player.heldProj = Projectile.whoAmI;
                    player.itemTime = 2;
                    player.itemAnimation = 2;
                    player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (!player.channel)
                        {
                            float speed = 30 * (Timer / Charge_Time) + 5;
                            Projectile.timeLeft = 180;

                            float newDamage = Projectile.damage;
                            newDamage *= 13 * (Timer / Charge_Time);
                            player.heldProj = -1;
                            Projectile.penetrate = 6;
                            Projectile.damage = (int)newDamage;
                            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                            State = ActionState.Throw;


                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 24f);
                            switch (Main.rand.Next(3))
                            {
                                case 0:
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj2"), Projectile.position);
                                    break;
                                case 1:
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj3"), Projectile.position);
                                    break;
                                case 2:
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj4"), Projectile.position);
                                    break;
                            }
                        }
                    }
                    break;

                case ActionState.Throw:
                    Projectile.rotation += Speed;
                    Projectile.velocity.Y += 0.1f;              
                    break;
            }

            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            switch (State)
            {
                case ActionState.Spin:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<RipperSlashProjSmall>(), 0, 0f, Projectile.owner,
                        ai1: Projectile.velocity.ToRotation() + MathHelper.ToRadians(45));
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.Center, 1024f, 4);
                    break;
                case ActionState.Throw:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner,
                        ai1: Projectile.velocity.ToRotation() + MathHelper.ToRadians(45));
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.Center, 1024f, 32f);
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                        var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 3f);
                        d.noGravity = true;
                    }
                    break;
            }
        }

        public TrailRenderer Trail;
        public override bool PreDraw(ref Color lightColor)
        {
            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StarTrail").Value;

            if (Trail == null)
            {
                Trail = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, 
                    (p) => Vector2.Lerp(new Vector2(90), Vector2.Zero, p), 
                    (p) => ColorFunctions.MiracleVoid * (1f - p));
                Trail.drawOffset = Projectile.Size / 2f;
            }

            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Transparent, ref lightColor);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            
            Trail.Draw(Projectile.oldPos);
            Texture2D spinTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Spiin").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(spinTexture, Projectile.Center - Main.screenPosition, null,
                    ColorFunctions.MiracleVoid * (Timer / Charge_Time) * 0.4f,
                    Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();



            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, 
                null, Main.GameViewMatrix.TransformationMatrix);

            Projectile projectile = Projectile;
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            int projFrames = Main.projFrames[projectile.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;

            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.Lerp(Color.Transparent, ColorFunctions.MiracleVoid, (1f / projectile.oldPos.Length * k) * (1f - 1f / projectile.oldPos.Length * k)) * (Timer / Charge_Time));
                Main.spriteBatch.Draw(glowTexture, drawPos, sourceRectangle, color, projectile.oldRot[k], drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Turn white
            for (int i = 0; i < 1; i++)
            {
                Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
                Color drawColor = Color.Lerp(Color.Transparent, ColorFunctions.MiracleVoid, (Timer / Charge_Time)) * (Timer / Charge_Time);
                Main.spriteBatch.Draw(glowTexture, drawPosition, sourceRectangle, drawColor, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
        }
    }
}