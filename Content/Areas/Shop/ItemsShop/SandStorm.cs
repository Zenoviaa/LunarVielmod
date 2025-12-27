using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Net.Sockets;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.ItemsShop
{
    public class SandStorm : BaseTome
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.CloneDefaults(ItemID.LastPrism);
            Item.shoot = ModContent.ProjectileType<SandStormTornado>();
            Item.shootSpeed = 10f;
            Item.damage = 12;
       
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<SandStormTornado>()] <= 0;
        }

        public override Color GetTomeHintColor()
        {

            return Color.SandyBrown;
        }
    }

    public class SandStormTornado : ModProjectile
    {

        private LittleStarParticleManager _tornadoStreakParticlesBackingField;
        private LittleStarParticleManager TornadoStreakParticles
        {
            get
            {
                _tornadoStreakParticlesBackingField ??= new LittleStarParticleManager(300, 8, GetTrailWidth, GetTrailColor);
                return _tornadoStreakParticlesBackingField;
            }
        }


        // These values place caps on the mana consumption rate of the Prism.
        // When first used, the Prism consumes mana once every MaxManaConsumptionDelay frames.
        // Every time mana is consumed, the pace becomes one frame faster, meaning mana consumption smoothly increases.
        // When capped out, the Prism consumes mana once every MinManaConsumptionDelay frames.
        private const float MaxManaConsumptionDelay = 15f;
        private const float MinManaConsumptionDelay = 5f;

        // This property encloses the internal AI variable Projectile.ai[0]. It makes the code easier to read.
        private float FrameCounter
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // This property encloses the internal AI variable Projectile.ai[1].
        private float NextManaFrame
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private ref float DeathTimer => ref Projectile.ai[2];
        private float ManaConsumptionRate
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }
        private void UpdateDamageForManaSickness(Player player)
        {
            Projectile.damage = (int)player.GetDamage(DamageClass.Magic).ApplyTo(player.HeldItem.damage);
        }

        private bool ShouldConsumeMana()
        {
            // If the mana consumption timer hasn't been initialized yet, initialize it and consume mana on frame 1.
            if (ManaConsumptionRate == 0f)
            {
                NextManaFrame = ManaConsumptionRate = MaxManaConsumptionDelay;
                return true;
            }

            // Should mana be consumed this frame?
            bool consume = FrameCounter == NextManaFrame;

            // If mana is being consumed this frame, update the rate of mana consumption and write down the next frame mana will be consumed.
            if (consume)
            {
                // MathHelper.Clamp(X,A,B) guarantees that A <= X <= B. If X is outside the range, it will be set to A or B accordingly.
                ManaConsumptionRate = MathHelper.Clamp(ManaConsumptionRate - 1f, MinManaConsumptionDelay, MaxManaConsumptionDelay);
                NextManaFrame += ManaConsumptionRate;
            }
            return consume;
        }

        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);


            
            // Update the Prism's damage every frame so that it is dynamically affected by Mana Sickness.
            UpdateDamageForManaSickness(player);

            // Update the frame counter.
            FrameCounter += 1f;

            // Update the Prism's behavior: project beams on frame 1, consume mana, and despawn if out of mana.
            if (Projectile.owner == Main.myPlayer && DeathTimer == 0)
            {
                Vector2 targetPosition = Main.MouseWorld;
                Projectile.velocity = (targetPosition - Projectile.Center) * 0.005f;
                Projectile.netUpdate = true;

                // player.CheckMana returns true if the mana cost can be paid. Since the second argument is true, the mana is actually consumed.
                // If mana shouldn't consumed this frame, the || operator short-circuits its evaluation player.CheckMana never executes.
                bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);

                // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                // player.channel indicates whether the player is still holding down the mouse button to use the item.
                bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;

                // Spawn in the Prism's lasers on the first frame if the player is capable of using the item.
                if (stillInUse && FrameCounter == 1f)
                {

                }

                // If the Prism cannot continue to be used, then destroy it immediately.
                else if (!stillInUse)
                {
                    DeathTimer++;
                }
            }

            if (DeathTimer > 0)
                DeathTimer++;
            // This ensures that the Prism never times out while in use.
            Projectile.timeLeft = 30;


            float inTornado = FrameCounter / 30f;
            float outTornado = DeathTimer / 30f;
            if (DeathTimer >= 30f)
                Projectile.Kill();

            if(FrameCounter == 1)
            {
                SoundStyle hurricaneBlack = AssetRegistry.Sounds.E.HurricaneBlack;
                hurricaneBlack.Volume = 1.5f;
                SoundEngine.PlaySound(hurricaneBlack);
            }
            if (FrameCounter % 12 == 0)
            {
                Vector2 vel = -Vector2.UnitY * 15;
                vel = vel.RotatedByRandom(MathHelper.ToRadians(45));
                Particle<DustParticle>.Spawn(Projectile.Center, vel, Color.White, Main.rand.NextFloat(0.3f, 1f));

                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasLightSpin;
                jiitasSit.PitchVariance = 0.2f;
                jiitasSit.Pitch = 0f;
                jiitasSit.Volume = 0.25f;
                SoundEngine.PlaySound(jiitasSit, Projectile.position);
            }
            float strength = 0.15f;
            foreach(var npc in Main.ActiveNPCs)
            {
                GlobalNPCSucker npcSucker = npc.GetGlobalNPC<GlobalNPCSucker>();
                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (!npc.friendly && dist <= 384)
                {
                    float timer = FrameCounter;
                    timer += npc.whoAmI * 3;
                    float xRadius = MathF.Sin(timer * 0.15f) * 256;
                    float yRadius = MathF.Cos(timer * 0.15f) * 24f;
                    Vector2 suckPosition = Projectile.Center + new Vector2(xRadius, yRadius);
                    suckPosition.Y -= 64;
                    suckPosition.Y += ExtraMath.Osc(0f, 32, 0, npc.whoAmI);
                
                    Vector2 diff = suckPosition - npc.Center;
                    Vector2 velocity = Vector2.Lerp(Vector2.Zero, diff, strength) * npc.knockBackResist;
                    Vector2 diffVelocity = velocity - npcSucker.SuckVelocity;
                    npcSucker.SuckVelocity += diffVelocity;

                }
            }
            ShakeModSystem.Shake = 1;

            inTornado = EasingFunction.InOutSine(inTornado);
            outTornado = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(outTornado));
            float alpha = inTornado * outTornado;
            TornadoStreakParticles.xOvalRadius = 2;
            TornadoStreakParticles.yOvalRadius = MathHelper.Lerp(75, 350, EasingFunction.InOutSine(FrameCounter / 150f));
            TornadoStreakParticles.minX = ExtraMath.Osc(25, 45, speed: 3) + MathHelper.Lerp(0f, 25f, EasingFunction.InOutSine(FrameCounter / 150f));
            TornadoStreakParticles.spinTime = 25;
            TornadoStreakParticles.rotationAxis = new Vector3(0, 1, 0.2f);
            TornadoStreakParticles.alpha = 0.65f * alpha;
            TornadoStreakParticles.topOnly = true;
            TornadoStreakParticles.scale = 0.2f;
            TornadoStreakParticles.Update(Projectile.Center);
        }
        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(completionRatio));
        }
        
        private Color GetTrailColor(float completionRatio)
        {
            Color trailColor = Color.Lerp(Color.Lerp(Color.Goldenrod, Color.Black, 0.5f), Color.SandyBrown, EasingFunction.QuadraticBump(completionRatio));
            float alpha = EasingFunction.QuadraticBump(completionRatio);
            trailColor *= alpha;
            return trailColor;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated, DrawLayer.OverNPCsWithOutline);
            return false;
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            TornadoStreakParticles.Draw();
        }
    }
}