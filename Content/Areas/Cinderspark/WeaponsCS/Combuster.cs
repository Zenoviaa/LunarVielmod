using Stellamod.Assets;
using Stellamod.Core.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class Combuster : ModItem
    {
        private int _combo;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToArtifact();
            Item.width = 20;
            Item.height = 54;
            Item.damage = 13;
            Item.knockBack = 8;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 25;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<CombusterSparkProj1>();
            Item.shootSpeed = 5;
            Item.rare = ItemRarityID.LightRed;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(8f, -8f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

            int slowdown = 6;
            int maxCombo = 15;
            if (_combo == maxCombo)
            {
                type = ModContent.ProjectileType<CombusterSparkProj3>();
                Item.useTime /= slowdown;
                Item.useAnimation /= slowdown;
            }
            else if (_combo == maxCombo - 1)
            {
                type = ModContent.ProjectileType<CombusterSparkProj2>();
                Item.useTime *= slowdown;
                Item.useAnimation *= slowdown;
            }
            else
            {
                bool alternate = _combo % 2 == 0;
                type = alternate ? ModContent.ProjectileType<CombusterSparkProj1>() : ModContent.ProjectileType<CombusterSparkProj2>();
            }

            _combo++;
            if (_combo >= maxCombo + 1)
                _combo = 0;

            Vector2 targetPosition = Main.MouseWorld;
            if (Collision.CanHitLine(player.Center, 1, 1, targetPosition, 1, 1))
            {
                position = targetPosition;
            }
            else
            {
                float length = ProjectileHelper.PerformBeamHitscan(player.Center, velocity, 1024);
                position = player.Center + velocity.SafeNormalize(Vector2.Zero) * length;
            }
            velocity = Vector2.Zero;
        }
    }
    public class CombustionBoom : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 1024;
            Projectile.height = 1024;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }


        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 30)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }


        public override void AI()
        {
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                switch (Main.rand.Next(0, 4))
                {
                    case 0:
                        target.AddBuff(BuffID.OnFire3, 120);
                        break;
                    case 1:
                        target.AddBuff(BuffID.ShadowFlame, 120);
                        break;
                    case 2:
                        target.AddBuff(BuffID.CursedInferno, 120);
                        break;
                    case 3:
                        target.AddBuff(BuffID.Daybreak, 60);
                        break;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 214;
            float height = 214;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 30;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), 0f, origin, 3f, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class CombusterExplosionProj1 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                for (float f = 0; f < 32; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, Projectile.position);

                var boom = FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);
                boom.Scale *= 0.75f;
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Red,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.OnFire3, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.ShadowFlame, 120);
                    break;
                case 2:
                    target.AddBuff(BuffID.CursedInferno, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Daybreak, 60);
                    break;
            }
        }
    }
    public class CombusterSparkProj1 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float RotationTimer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Timer++;
            float rotationMulti = 1f - (Timer / 60);
            RotationTimer += rotationMulti * 5;
            Projectile.rotation = MathHelper.ToRadians(RotationTimer);
            if (Timer == 1)
            {
                Player owner = Main.player[Projectile.owner];
                for (float f = 0; f < 32; f++)
                {
                    float progress = f / 32f;
                    Vector2 pos = Vector2.Lerp(Projectile.Center, owner.Center, progress);
                    Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
                for (float f = 0; f < 7; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.Yellow, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterSnap") with { PitchVariance = 0.15f }, Projectile.position);
            }

            if (Timer == 45)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
            }

            if (Timer % 4 == 0)
            {
                float scaleMult = Timer / 60;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<CombusterExplosionProj1>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
    public class CombusterSparkProj2 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float RotationTimer => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Timer++;
            float rotationMulti = 1f - (Timer / 60);
            RotationTimer += rotationMulti * 5;
            Projectile.rotation = MathHelper.ToRadians(RotationTimer);
            if (Timer == 1)
            {
                Player owner = Main.player[Projectile.owner];
                for (float f = 0; f < 32; f++)
                {
                    float progress = f / 32f;
                    Vector2 pos = Vector2.Lerp(Projectile.Center, owner.Center, progress);
                    Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
                for (float f = 0; f < 7; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterSnap") with { PitchVariance = 0.15f }, Projectile.position);
            }

            if (Timer == 45)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
            }

            if (Timer % 4 == 0)
            {

            }
        }

        public override void OnKill(int timeLeft)
        {
            FXUtil.ShakeCamera(Projectile.position, 2048, 8);
            SoundStyle kaboom = AssetManager.GetSound("Kaboom");
            kaboom.PitchVariance = 0.3f;
            SoundEngine.PlaySound(kaboom, Projectile.position);

            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<CombustionBoomMini>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);

                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ProjectileID.WandOfSparkingSpark, Projectile.damage, 0f, Projectile.owner);
                }
            }
        }
    }
    public class CombusterSparkProj3 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float RotationTimer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Timer++;
            float rotationMulti = 1f - (Timer / 60f);
            RotationTimer += rotationMulti * 5;
            Projectile.rotation = MathHelper.ToRadians(RotationTimer);
            if (Timer == 1)
            {
                Player owner = Main.player[Projectile.owner];
                for (float f = 0; f < 32; f++)
                {
                    float progress = f / 32f;
                    Vector2 pos = Vector2.Lerp(Projectile.Center, owner.Center, progress);
                    Dust.NewDustPerfect(pos, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
                for (float f = 0; f < 7; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.Red, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterSnap") with { PitchVariance = 0.15f }, Projectile.position);
            }

            if (Timer == 45)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
            }
        }

        public override void OnKill(int timeLeft)
        {
            FXUtil.ShakeCamera(Projectile.position, 2048, 32);
            SoundEngine.PlaySound(SoundRegistry.CombusterBoom, Projectile.position);

            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                       ModContent.ProjectileType<CombustionBoom>(), Projectile.damage * 8, Projectile.knockBack * 2, Projectile.owner);
            }
   
            for (float f = 0; f < 24; f++)
            {
                Color glyphColor = Color.Red;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        glyphColor = Color.Red;
                        break;
                    case 1:
                        glyphColor = Color.OrangeRed;
                        break;
                    case 2:
                        glyphColor = Color.Yellow;
                        break;
                }
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 25f)).RotatedByRandom(19.0), 0, glyphColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
            morrowExp.PitchVariance = 0.3f;
            SoundEngine.PlaySound(morrowExp, Projectile.position);
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: 0.2f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < Main.rand.Next(7, 13); i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ProjectileID.WandOfSparkingSpark, Projectile.damage, 0f, Projectile.owner);
            }
        }
    }
}
