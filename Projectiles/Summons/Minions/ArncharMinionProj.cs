using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Bow;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class ArncharMinionProj : ModProjectile
    {
        private float WhiteTimer;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Arnchar Drone");
            // Sets the amount of frames this minion has on its spritesheet
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Type] = 4;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 1f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = Projectile.rotation;
            Color finalColor = Color.White.MultiplyRGB(lightColor);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, Projectile.Frame(), Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);


            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < 6; i++)
                spriteBatch.Draw(glowTexture, drawPos, frame, finalColor * WhiteTimer, rotation, drawOrigin, Vector2.One, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            for (float f = 0f; f < 4f; f++)
            {
                Vector2 offset = ((f / 4f) * MathHelper.ToRadians(360) + Main.GlobalTimeWrappedHourly * 8).ToRotationVector2() * VectorHelper.Osc(3f, 4f);
                spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition + offset,
                    Projectile.Frame(), Color.White * VectorHelper.Osc(0f, 0.5f), Projectile.rotation, Projectile.Frame().Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }
        public override void AI()
        {

            Player player = Main.player[Projectile.owner];
            Projectile.spriteDirection = Projectile.direction;
            if (!SummonHelper.CheckMinionActive<ArncharMinionBuff>(player, Projectile))
                return;

            if (Main.rand.NextBool(12))
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.OrangeRed, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                else
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity * 0.1f, 0, Color.OrangeRed, 1f).noGravity = true;
            }
            WhiteTimer *= 0.98f;
            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (!foundTarget)
            {
                Timer--;
                if (Timer <= 0)
                    Timer = 0;
                SummonHelper.CalculateIdleValues(Owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }
            else if (foundTarget)
            {
                Timer++;
                Vector2 targetHoverPos = targetCenter - new Vector2(0, 80);
                targetHoverPos.X += MathF.Sin(Timer) * 32;

                Vector2 targetVelocity = (targetHoverPos - Projectile.Center) * 0.05f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.1f);
                if (Timer > 30 && Timer % 10 == 0)
                {
                    int Sound = Main.rand.Next(1, 3);
                    SoundStyle mySound = new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone1");

                    if (Sound == 1)
                    {
                        mySound = new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone1");
                    }
                    else
                    {
                        mySound = new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone2");
                    }

                    mySound.PitchVariance = 0.2f;
                    SoundEngine.PlaySound(mySound, Projectile.position);
                    Vector2 fireVelocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * 12;
                    Projectile.velocity -= fireVelocity;
                    WhiteTimer = 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity,
                            ModContent.ProjectileType<ArchariliteArrowSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0);
                    }

                    if(Main.myPlayer == Projectile.owner)
                    {
                        Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(35));
                        Projectile.netUpdate = true;
                    }
                }
                if (Timer > 120)
                {
                    Timer = 0;
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }
    }
}

