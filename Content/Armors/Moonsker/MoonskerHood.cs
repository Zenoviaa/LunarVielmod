using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Moonsker
{
    public class MoonskerBlast : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => ModContent.GetInstance<MoonskerMoon>().Texture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
         
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                {
                    innerColor = Color.Goldenrod,
                    outerColor = Color.DarkGoldenrod
                };
                ShockOvalParticle sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.4f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.6f;

                sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.3f;
            }
            if(Timer < 30)
                Projectile.velocity *= 1.01f;
            if (Timer >= 30)
                Projectile.tileCollide = true;

            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC nearest = NPCHelper.FindClosestNPC(Projectile.position, 64);
            if(nearest != null)
            {
                Vector2 velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center);
                Projectile.velocity = velocity;
            }

            if (Main.rand.NextBool(12))
            {
               var sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.45f);
                sp.flickering = true;
                sp.innerColor = Color.Goldenrod;
                sp.outerColor = Color.DarkGoldenrod;
                sp.gravity = 0f;
            }
            if (Main.rand.NextBool(8))
            {


                LightningSparkParticle dp = Particle<LightningSparkParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), color: Color.Goldenrod, Scale: Main.rand.NextFloat(0.2f, 0.35f));

                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                dp.fast = true;
                dp.Scale *= 0.5f;

                if (Main.rand.NextBool(8))
                {
                    FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                        color: Color.Goldenrod, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                    sp.gravity = 0f;
                    sp.fast = true;
                    sp.dampening = 0.1f;
                    sp.Scale *= 0.5f;
                }

            }
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Gold, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 24;
            return MathHelper.SmoothStep(width, 0f, completionRatio);
        }

        private void DrawMoonskerTrails(GraphicsDevice graphicsDevice)
        {
            var shader2 = RichLaserShader.Instance;
            shader2.LaserColor = Color.Wheat;
            shader2.InnerColor = Color.Gold * 0.5f;
            shader2.OuterColor = Color.DarkGoldenrod;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);
        }

        private void DrawMoonskerOrb(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Vector2 drawPos = Projectile.Center - screenPos;
            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Goldenrod;
            glowColor = Color.Lerp(Color.Goldenrod, Color.DarkGoldenrod, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.1f, SpriteEffects.None, 0);
            // spriteBatch.RestartDefaults();


            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Goldenrod;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (i % 2 != 0)
                    continue;
                glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
                glowDrawOrigin = glowMask.Size() / 2f;
                glowColor = Color.Orange;
                glowColor *= 0.2f;
                glowColor.A = 0;
                drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - screenPos;
                float rotation = Projectile.oldRot[i];
                float ratio = (float)i / (float)Projectile.oldPos.Length;
                float outScale = MathHelper.SmoothStep(1f, 0.2f, ratio);
                spriteBatch.Draw(glowMask, drawPos, null, glowColor, rotation, glowDrawOrigin, new Vector2(2f, 1f) * 
                    Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.15f * outScale, SpriteEffects.None, 0);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawMoonskerTrails);
            PixelationManager.QueueSpritebatchDrawAction(DrawMoonskerOrb);

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            Color drawColor = Color.Lerp(lightColor, Color.Black, ExtraMath.Osc(0.5f, 1f, speed: 2));
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null,
                drawColor, Projectile.rotation, drawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8), SpriteEffects.None, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Goldenrod,
                    outerGlowColor: Color.DarkGoldenrod, duration: 25, baseSize: 0.075f);

            for (float n = 0; n < 4; n++)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4f, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.outerColor = Color.Goldenrod;
            }
            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Goldenrod,
                    outerGlowColor: Color.DarkGoldenrod,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
                particle.Scale *= 0.5f;
            }
        }
    }

    public class MoonskerMoon : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        private ref float ScaleTimer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
        }

   
        private void CastMoonBlast(Player player, AdvancedMagicProjectile projectile)
        {


        }

        public override void AI()
        {
            base.AI();
            if (Owner.GetModPlayer<MoonskerPlayer>().hasMoonskerSetBonus)
                Projectile.timeLeft = 10;
            Timer += MathHelper.TwoPi * 0.003f;
            ScaleTimer++;
            Vector2 offset = Vector2.UnitY.RotatedBy(Timer);
            Vector2 targetCenter = Owner.Center + offset * 64;
            Vector2 targetVelocity = targetCenter - Projectile.Center;
            Projectile.velocity = targetVelocity;
            Projectile.rotation = ExtraMath.Osc(-0.1f, 0.1f, offset: Projectile.whoAmI);
            Projectile.scale = MathHelper.SmoothStep(0f, 1f, ScaleTimer / 60f);
        }
        private void DrawMoonskerOrb(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Vector2 drawPos = Projectile.Center - screenPos;
            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Goldenrod;
            glowColor = Color.Lerp(Color.Goldenrod, Color.DarkGoldenrod, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.1f, SpriteEffects.None, 0);
            // spriteBatch.RestartDefaults();


            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Goldenrod;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.2f, SpriteEffects.None, 0);


        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawMoonskerOrb);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            drawCenter.Y += ExtraMath.Osc(-1f, 1f);
            spriteBatch.Draw(texture, drawCenter, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class MoonskerPlayer : ModPlayer
    {
        public bool hasMoonskerSetBonus;
        public override void Load()
        {
            base.Load();
            AdvancedMagicStaffHold.OnCastMagic += CastMoonMagic;
        }


        public override void Unload()
        {
            base.Unload();
            AdvancedMagicStaffHold.OnCastMagic -= CastMoonMagic;
        }
        private void CastMoonMagic(Player player, AdvancedMagicProjectile projectile)
        {
            if (!player.GetModPlayer<MoonskerPlayer>().hasMoonskerSetBonus)
                return;
            if (player.whoAmI != Main.myPlayer)
                return;

            int type = ModContent.ProjectileType<MoonskerMoon>();
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != player.whoAmI)
                    continue;
                if (proj.type != type)
                    continue;

                Vector2 fireVelocity = (Main.MouseWorld - proj.Center).SafeNormalize(Vector2.Zero) * 12;
                Vector2 firePoint = proj.Center;
                Projectile.NewProjectile(player.GetSource_FromThis(), firePoint, fireVelocity, ModContent.ProjectileType<MoonskerBlast>(),
                    (int)(projectile.Projectile.damage * 0.5f), proj.knockBack, proj.owner);
                proj.Kill();
            }
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            hasMoonskerSetBonus = false;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (!hasMoonskerSetBonus)
                return;
            if (Player.whoAmI != Main.myPlayer)
                return;
            int type = ModContent.ProjectileType<MoonskerMoon>();
            if (Player.ownedProjectileCounts[type] != 0)
                return;

            for (float i = 0; i < 3; i++)
            {
                float ratio = i / 3f;
                float radians = ratio * MathHelper.TwoPi;
                int damage = 50;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, type, damage, 1, 
                    Player.whoAmI, ai0: radians);
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class MoonskerHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<MoonskerHood, MoonskerRobe, MoonskerPants>();
        }


        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.artifactManaReduction += 0.1f;
            stats.defenseBonus += 15;
            stats.criticalStrikeDamage += 0.5f;
            stats.accessorySlots += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MoonskerRobe>() && legs.type == ModContent.ItemType<MoonskerPants>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<MoonskerPlayer>().hasMoonskerSetBonus = true;
            player.GetModPlayer<AdvancedMagicPlayer>().chargeTimeBonus += 0.3f;

        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class MoonskerRobe : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.magicDamage += 0.56f;
            stats.accessorySlots += 1;
            stats.defenseBonus += 25;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class MoonskerPants : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.totalMana += 150;
            stats.accessorySlots += 1;
            stats.defenseBonus += 10;
        }
    }
}
