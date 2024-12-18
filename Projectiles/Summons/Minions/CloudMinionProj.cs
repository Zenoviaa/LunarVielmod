using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Buffs.Minions;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class CloudMinionProj : ModProjectile
    {
        private float FlashTimer;
        private Vector2 FlashPos;
        private ref float Timer => ref Projectile.ai[0];
        private ref float LightningTimer => ref Projectile.ai[1];
        private ref float TornadoTimer => ref Projectile.ai[2];
        private bool DoLightning => Projectile.minionSlots >= 3;
        private bool DoTornado => Projectile.minionSlots >= 6;
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 34;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<CloudMinionBuff>(owner, Projectile))
                return;

            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Timer++;
            if (Main.rand.NextBool(100))
            {
         
    
                FlashPos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height));
                for (float f = 0; f < 16; f++)
                {
                    int d = Dust.NewDust(FlashPos, 1, 1, ModContent.DustType<GlyphDust>(), newColor: GetMainColor(), Scale: Main.rand.NextFloat(0.5f, 2f));
                    Main.dust[d].velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 9f);
                }
                FlashTimer = 1.5f;
            }
            if(Timer % 6 == 0)
            {

                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlyphDust>(), newColor: GetMainColor(), Scale: Main.rand.NextFloat(0.5f, 2f));
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Rain, newColor: GetMainColor(), Scale: Main.rand.NextFloat(0.5f, 2f));
            }
            FlashTimer *= 0.912f;
            if(Timer > 12 && foundTarget)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Vector2 offset = new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height));
                    Vector2 pos = Projectile.position + offset;
                    Vector2 velocity = (targetCenter - pos).SafeNormalize(Vector2.Zero) * 12;
                   
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, velocity,
                            ProjectileID.WandOfFrostingFrost, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                Timer = 0;
            }
            if (DoLightning && foundTarget)
            {
          
                LightningTimer++;
       
                if(LightningTimer == 60)
                {
                    FlashTimer = 2f;
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY,
                            ModContent.ProjectileType<TempestLightningBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                if(LightningTimer > 240)
                {
                    LightningTimer = 0;
                }
            }

            if (DoTornado && foundTarget)
            {
                TornadoTimer++;
                if(TornadoTimer > 120 && TornadoTimer % 30 == 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                            ModContent.ProjectileType<ClimateTornadoProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                if(TornadoTimer > 180)
                {

                    TornadoTimer = 0;
                }
            }
            Vector2 targetPosition = Owner.Center + new Vector2(0, -Projectile.height * 4);
            Projectile.velocity = (targetPosition - Projectile.Center) * 0.1f;
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.005f;


            // Some visuals here
            Lighting.AddLight(Projectile.Center, GetMainColor().ToVector3() * 2.5f);
        }

    
        private Color GetMainColor()
        {
            if (DoTornado)
                return Color.Green;
            if (DoLightning)
                return Color.Goldenrod;
            return Color.Cyan;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ThunderCloudShader shader = ThunderCloudShader.Instance;
            shader.CloudColor = Color.Lerp(GetMainColor(), Color.Black, 0.7f);
            shader.CloudColor = Color.White;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            shader.SourceSize = texture.Size();


            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            shader.CloudColor = GetMainColor();
            shader.Apply();

            float off = 112;
            spriteBatch.Restart(effect: shader.Effect, sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);
            spriteBatch.Draw(texture, drawPos + new Vector2(off, 0), null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            for (float f = 0; f < 8; f++)
            {
                float p = f / 8f;
                float rot = p * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly;
                Vector2 offset = rot.ToRotationVector2() * 8;
                spriteBatch.Draw(texture, drawPos + new Vector2(off, 0) + offset, null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            shader.CloudColor = Color.Black;
            shader.Apply();
            spriteBatch.Restart(effect: shader.Effect, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            spriteBatch.Draw(texture, drawPos + new Vector2(off, 0), null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
  
            shader.CloudColor = Color.Lerp(Color.Black, GetMainColor(), FlashTimer);
            shader.Apply();
            spriteBatch.Restart(effect: shader.Effect, sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);

           
            for(int i = 0; i < 1; i++)
                spriteBatch.Draw(texture, drawPos + new Vector2(off, 0), null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.RestartDefaults();


            shader.CloudColor = Color.Lerp(Color.Black, Color.Gold, FlashTimer);
            shader.Apply();
            spriteBatch.Restart(effect: shader.Effect, sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);


            for (int i = 0; i < 1; i++)
                spriteBatch.Draw(texture, drawPos + new Vector2(off, 0), null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Color glowColor = GetMainColor();
            glowColor.A = 0;
            glowColor *= FlashTimer;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(texture2D4, FlashPos - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            }
        }

    }
}
