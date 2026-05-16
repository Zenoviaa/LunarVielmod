using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.LunarianVoid
{
    public class LunarianVoidCounterSlash : ModProjectile
    {
        private Vector2[] RiftPoints = new Vector2[32];
        private ref float Timer => ref Projectile.ai[0];
        private ref float RandScale => ref Projectile.ai[1];
        private bool IsLong => Projectile.ai[2] == 1;
        private float Interpolant;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 25;
            Projectile.friendly = true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 startCenter = Projectile.Center - Projectile.velocity * 64;
            Vector2 endCenter = Projectile.Center;
            Vector2 center = Vector2.Lerp(startCenter, endCenter, EasingFunction.OutExpo(Timer / 25f));
            Vector2 start = center - Projectile.velocity * 16 * RandScale;
            Vector2 end = center + Projectile.velocity * 16 * RandScale;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 12, ref collisionPoint);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                if (IsLong)
                {
                    Projectile.timeLeft += 180;
                }
                if (this.OwnedByLocalClient())
                {
                    RandScale = Main.rand.NextFloat(0.5f, 1f);
                }
                SoundStyle eventHorizonSound = new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Rune_SoulStar");
                eventHorizonSound.PitchVariance = 0.3f;
                if (Main.rand.NextBool(2))
                {
                    eventHorizonSound = new SoundStyle("Stellamod/Assets/Sounds/CinderBraker");
                    eventHorizonSound.PitchVariance = 0.3f;
                }
                eventHorizonSound.Volume = 0.5f;
                SoundEngine.PlaySound(eventHorizonSound, Projectile.position);
            }
            if (Timer % 9 == 0)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Projectile.velocity.RotatedByRandom(4f) * Main.rand.NextFloat(0.1f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.innerColor = Color.Black;
                dp.outerColor = Color.DarkBlue;
            }
            Interpolant = EasingFunction.InExpo(Timer / 25f);
            if (IsLong)
            {
                Interpolant = EasingFunction.InExpo(Timer / 260f);
                Projectile.velocity = Projectile.velocity.RotatedBy(0.005f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedTrails, DrawLayer.OverNPCs);
            return false;
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.White;
        }
        private Color GetTrailColor2(float completionRatio)
        {
            return Color.Black;
        }
        private float GetTrailWidth(float completionRatio)
        {
            float baseWidth = EasingFunction.QuadraticBump(completionRatio) * 32;
            float outScale = MathHelper.Lerp(1f, 0f, Interpolant);
            float inScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 15f));
            return baseWidth * outScale * inScale;
        }
        private float GetTrailWidth2(float completionRatio)
        {
            return GetTrailWidth(completionRatio) * 0.3f;
        }

        private void RenderPixelatedTrails(GraphicsDevice graphicsDevice)
        {
            float numPoints = 32;

            float length = 16;
            if (IsLong)
                length *= 0.5f;
            Vector2 startCenter = Projectile.Center - Projectile.velocity * 64;
            Vector2 endCenter = Projectile.Center;
            Vector2 center = Vector2.Lerp(startCenter, endCenter, EasingFunction.OutExpo(Timer / 25f));
            Vector2 start = center - Projectile.velocity * length * RandScale;
            Vector2 end = center + Projectile.velocity * length * RandScale;
            for (int n = 0; n < numPoints; n++)
            {
                ref Vector2 point = ref RiftPoints[n];
                float ratio = (float)n / numPoints;
                point = Vector2.Lerp(start, end, ratio);
                point += Main.rand.NextVector2Circular(2, 2);
            }

            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Black;
            Color innerColor = Color.Lerp(Color.LightSkyBlue, Color.DarkBlue, 0.75f);
            shader.InnerColor = innerColor;
            shader.OuterColor = Color.Blue;
            if (Timer < 15)
            {
                shader.OuterColor = Color.Lerp(Color.White, Color.LightSkyBlue, EasingFunction.InOutSine(Timer / 15f));
                shader.InnerColor = Color.Lerp(Color.White, innerColor, EasingFunction.InOutSine(Timer / 15f));
                shader.LaserColor = Color.Lerp(Color.White, Color.Black, EasingFunction.InOutSine(Timer / 15f));
            }
            TrailDrawer.Draw(Main.spriteBatch, RiftPoints, GetTrailColor, GetTrailWidth, shader);


            var blackShader = BasicLaserAlphaShader.Instance;
            blackShader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, RiftPoints, GetTrailColor2, GetTrailWidth2, blackShader);
        }
    }
    public class LunarianVoidPlayer : ModPlayer
    {
        public int counterAttackTimer;
        public bool hasLunarianVoidSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasLunarianVoidSetBonus = false;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (counterAttackTimer > 0)
                counterAttackTimer--;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);
            counterAttackTimer = 30;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPCWithProj(proj, target, ref modifiers);
            if (!hasLunarianVoidSetBonus)
                return;
            if (counterAttackTimer <= 0)
                return;

            Vector2 spawnVelocity = new Vector2(24);
            Vector2 spawnCenter = target.Center - spawnVelocity;
            Projectile.NewProjectile(Player.GetSource_FromThis(), spawnCenter, spawnVelocity,
                ModContent.ProjectileType<LunarianVoidCounterSlash>(), proj.damage * 4, proj.knockBack, proj.owner);

            FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: 0.12f);

            for (float n = 0; n < 4; n++)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4f, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.outerColor = Color.Blue;
            }
            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            counterAttackTimer = 0;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class LunarianVoidHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorSetSystem.RegisterArmorSet<LunarianVoidHead, LunarianVoidBody, LunarianVoidLegs>(ArmorGroup.Act_I);
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            /*
             4 Defense
1 Accessory Slot
1 Stamina
10 Aggressiveness (100)
*/

            stats.defenseBonus += 4;
            stats.accessorySlots += 1;
            stats.stamina += 1;
            stats.meleeAggressiveness += 100;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LunarianVoidBody>() && legs.type == ModContent.ItemType<LunarianVoidLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<LunarianVoidPlayer>().hasLunarianVoidSetBonus = true;
         //   Lighting.AddLight(player.position, TorchID.Blue);
        }
    }
   
    
    [AutoloadEquip(EquipType.Body)]
    public class LunarianVoidBody : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.defenseBonus += 6;
            stats.accessorySlots += 1;
            stats.stamina += 1;
            stats.meleeDamage += 0.15f;
        }
    }


    [AutoloadEquip(EquipType.Legs)]
    public class LunarianVoidLegs : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.defenseBonus += 2;
            stats.accessorySlots += 1;
            stats.meleeArmorPenetration += 5;
        }
    }
}
