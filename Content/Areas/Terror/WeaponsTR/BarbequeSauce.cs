using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR
{
    public class BarbequeSauce : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToCombatTool(0, 0, ammoCount: 1);
            Item.shoot = ModContent.ProjectileType<BarbequeSauceThrow>();

        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<TerrorFragments, BlankJuggler>();
        }
    }

    public class BarbequeSauced : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 6;
            if (Main.rand.NextBool(3))
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
                sp.initialColor = Color.Lerp(Color.OrangeRed, Color.RosyBrown, Main.rand.NextFloat(0f, 1f)) * 0.4f;
                sp.expand = true;
            }
            if (Main.rand.NextBool(3))
            {
                LegacyParticle.NewParticle<EmberParticle>(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            }
        }
    }

    public class BarbequeSaucedGlobalNPC : GlobalNPC
    {
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            base.ModifyIncomingHit(npc, ref modifiers);
            if (npc.HasBuff<BarbequeSauced>())
            {
                modifiers.FinalDamage += 0.2f;
            }
        }
    }

    public class BarbequeSauceThrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => ModContent.GetInstance<BarbequeSauce>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;

        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle useSound = SoundID.DD2_GhastlyGlaivePierce;
                useSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(useSound, Projectile.position);
            }

            if (Timer % 8 == 0)
            {
                LegacyParticle.NewParticle<EmberParticle>(Projectile.Center, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.4f, 0.78f));
            }

            if (Timer % 15 == 0)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Vector2.Zero, Color.White, 0.2f);
                dp.outerColor = Color.Brown;
            }

            Projectile.velocity.Y += 0.3f;
            Projectile.rotation += Projectile.velocity.X * 0.015f;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BarbequeSauced>(), 30 * 60);
            CreateImpactEffects();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            CreateImpactEffects();
            return true;
        }
        private void CreateImpactEffects()
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 0.5f).noGravity = true;
            }

            int numDust = 8;
            for(int n = 0; n < numDust; n++)
            {
                var sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY, Scale: Main.rand.NextFloat(1f, 2f));
                sp.initialColor = Color.Brown;
            }

            for (int n = 0; n < numDust; n++)
            {
                var dp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.5f, 25), Scale: Main.rand.NextFloat(1f, 2f));
            }


            ShakeScreenPosition.Shake = 3;
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;

            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"), Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawCenter = oldPos + Projectile.Size / 2f - Main.screenPosition;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, (float)i / (float)Projectile.oldPos.Length) * 0.1f;
                spriteBatch.Draw(texture, oldDrawCenter, null, afterImageColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, drawPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
