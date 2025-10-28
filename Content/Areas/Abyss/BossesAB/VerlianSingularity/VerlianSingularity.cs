using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Abyss.BossesAB.VerlianSingularity.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Abyss.BossesAB.VerlianSingularity
{
    public class SingularitySuckPlayer : ModPlayer
    {
        public Vector2? pullVelocity;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (pullVelocity.HasValue)
            {
                Vector2 velocity = pullVelocity.Value;
                Player.velocity = velocity;
                pullVelocity = null;
            }
        }
    }

    public class VerlianSingularity : ScarletBoss
    {
        private float _spinTimer;
        private float _spawnScale;
        private float _spazzingTimer;
        private bool _focusOn;
        private bool _spawnedCrescentMoon;
        private Vector2 _shakeOffset;
        private enum AIState
        {
            Spawn,
            Idle,
            OrbitingStarPull,
            SpiralStarPull,
            ZigzagStorm
        }
        private int ShootingStarDamage => 24;
        private int SpiralStarDamage => 16;
        private ref float Timer => ref NPC.ai[0];
        private ref float AttackCounter => ref NPC.ai[1];
        private AIState State
        {
            get => (AIState)NPC.ai[2];
            set => NPC.ai[2] = (float)value;
        }
        private ref float AttackCycle => ref NPC.ai[3];



        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 11;
            NPC.lifeMax = 4500;
            NPC.scale = 1f;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;

            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SingularityFragment");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (State == AIState.Spawn)
                return false;
            return base.CanHitPlayer(target, ref cooldownSlot);
        }

        public override void AI()
        {
            base.AI();
            _spinTimer++;

            float startRadians = -MathHelper.ToRadians(22);
            float endRadians = startRadians + MathHelper.ToRadians(2);
            float interpolant = ExtraMath.Osc(0f, 1f, speed: 1);
            NPC.rotation = MathHelper.Lerp(startRadians, endRadians, interpolant);
            NPC.velocity = Vector2.UnitY.RotatedBy(_spinTimer * 0.02f) * 0.5f;
            NPC.velocity.X = 0;


            if (_spazzingTimer > 0)
            {
                if (_spazzingTimer % 10 == 0)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.innerColor = Color.White;
                    spark.outerColor = Color.Cyan;
                    spark.fadeToColor = Color.Blue;
                    spark.Scale *= 0.5f;

                }
                _shakeOffset = Main.rand.NextVector2Circular(4, 4);
                _spazzingTimer--;
            }

            SuckNearbyPlayers();
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    if (!_spawnedCrescentMoon)
                    {
                        if (StellaMultiplayer.IsHost)
                        {
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                                ModContent.NPCType<VerlianSingularity>(), ai0: NPC.whoAmI);
                        }

                        _spawnedCrescentMoon = true;    
                    }
                    _spawnScale = MathHelper.Lerp(_spawnScale, 1, 0.1f);
                    SuckingParticles();
                    AI_Idle();
                    break;
                case AIState.OrbitingStarPull:
                    AI_OrbitingStarPull();
                    break;
                case AIState.SpiralStarPull:
                    AI_SpiralStarPull();
                    break;
                case AIState.ZigzagStorm:
                    AI_ZigzagStorm();
                    break;
            }
        }

        private void ChooseAttack()
        {
            if (!StellaMultiplayer.IsHost)
                return;

            switch (AttackCycle)
            {
                case 0:
                    SwitchState(AIState.OrbitingStarPull);
                    break;
                case 1:
                    SwitchState(AIState.SpiralStarPull);
                    break;
                case 2:
                    SwitchState(AIState.ZigzagStorm);
                    break;
            }
            AttackCycle++;
            if (AttackCycle >= 3)
            {
                AttackCycle = 0;
            }
        }

        private void SuckNearbyPlayers()
        {
            foreach(var player in Main.ActivePlayers)
            {
                float distanceToPlayer = NPC.DistanceFrom(player);
                if(distanceToPlayer > 1000 && distanceToPlayer < 2000)
                {
                    Vector2 pullingVelocity = player.NormalizedVelocityTo(NPC);
                    pullingVelocity *= 2;

                    SingularitySuckPlayer suckPlayer = player.GetModPlayer<SingularitySuckPlayer>();
                    suckPlayer.pullVelocity = pullingVelocity;
                }
            }
        }


        /// <summary>
        /// Creates a pulsing effect, good way to show the singularity is summoning projectiles
        /// </summary>
        private void SpazOut()
        {
            _spawnScale *= 0.9f;
            _spazzingTimer = 120;
            FXUtil.ShakeCamera(NPC.position, 1024, 8);
            FXUtil.GlowCircleBoom(NPC.Center,
                       innerColor: Color.White,
                       glowColor: Color.LightBlue,
                       outerGlowColor: Color.Blue, duration: 25, baseSize: 0.08f);

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = Particle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.innerColor = Color.White;
                spark.outerColor = Color.Cyan;
                spark.fadeToColor = Color.Blue;
            }

            SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot1");
            crackSound.PitchVariance = 0.1f;
            SoundEngine.PlaySound(crackSound, NPC.position);
        }
        private void MiniSpazOut()
        {
            _spawnScale *= 0.97f;
            _spazzingTimer = 60;

            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = Particle.NewParticle<SparkParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.innerColor = Color.White;
                spark.outerColor = Color.Cyan;
                spark.fadeToColor = Color.Blue;
            }

            SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot1");
            crackSound.PitchVariance = 0.1f;
            SoundEngine.PlaySound(crackSound, NPC.position);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
        }

        public override void OnKill()
        {
            base.OnKill();
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSOMBoss, -1);
        }
        private void AI_ZigzagStorm()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
            }
            if (Timer == 10)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
            }
            if (Timer == 20)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
            }

            if(Timer > 60 && Timer % 10 == 0)
            {
                if (StellaMultiplayer.IsHost)
                {
                    int orbitingStarType = ModContent.ProjectileType<ZigzaggingStar>();
                    float rot = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
                    rot += Timer * 0.05f;
                    Vector2 offset = rot.ToRotationVector2();
                    offset *= 1000;
                    Vector2 spawnPos = NPC.Center + offset;
                    Vector2 spawnVelocity = Vector2.Zero;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, spawnVelocity, orbitingStarType, SpiralStarDamage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                }
                AttackCounter++;
            }

            if(AttackCounter >= 16)
            {
                SwitchState(AIState.Idle);
            }

        }
        private void AI_SpiralStarPull()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
            }

            if (Timer % 10 == 0)
            {
                MiniSpazOut();

                float num = 2;
                for (float f = 0; f < num; f++)
                {
                    int orbitingStarType = ModContent.ProjectileType<SpiralFallingStar>();
                    float interpolant = f / num;
                    float rot = MathHelper.TwoPi * interpolant;
                    rot += Timer * 0.05f;
                    Vector2 offset = rot.ToRotationVector2();
                    offset *= 1000;
                    Vector2 spawnPos = NPC.Center + offset;
                    Vector2 spawnVelocity = Vector2.Zero;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, spawnVelocity, orbitingStarType, SpiralStarDamage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                }
                AttackCounter++;
            }

            if (AttackCounter >= 32)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_OrbitingStarPull()
        {
            Timer++;
            if (Timer == 1)
            {
                SpazOut();
            }

            //Asgore attack basically
            if (Timer >= 60)
            {
                var part = Particle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
                part.Scale *= 4;
                part.shrink = true;
                part.noStretch = true;
                if (StellaMultiplayer.IsHost)
                {
                    int orbitingStarType = ModContent.ProjectileType<OrbitingShootingStar>();

                    float num = 16;
                    float direction = AttackCounter % 2 == 0 ? 1 : -1;
                    float randOffset = Main.rand.NextFloat(-0.5f, 0.5f);
                    for (float f = 0; f < num; f++)
                    {
                        float interpolant = f / num;
                        float rot = MathHelper.TwoPi * interpolant;
                        rot += (AttackCounter / 3f) * MathHelper.TwoPi;
                        rot += randOffset;
                        Vector2 offset = rot.ToRotationVector2();
                        offset *= 1200;
                        Vector2 spawnPos = NPC.Center + offset;
                        Vector2 spawnVelocity = Vector2.Zero;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, spawnVelocity, orbitingStarType, ShootingStarDamage, 1, Main.myPlayer,
                            ai0: NPC.whoAmI,
                            ai2: direction);
                    }

                }
                Timer = 0;
                AttackCounter++;
            }

            if (AttackCounter >= 6)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void SuckingParticles()
        {
            if (_spinTimer % 7 == 0)
            {
                float radius = 800;
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                Vector2 velocity = NPC.Center - spawnPos;
                velocity = velocity.SafeNormalize(Vector2.Zero);
                velocity *= Main.rand.NextFloat(16, 64);
                FXUtil.GlowStretch(spawnPos, velocity);
            }
        }
        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                AttackCounter = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void SpawnPulse()
        {
            float strength = MathHelper.Lerp(0f, 128, AttackCounter / 3f);
            FXUtil.ShakeCamera(NPC.position, 1024, strength);
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = Particle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.innerColor = Color.White;
                spark.outerColor = Color.Cyan;
                spark.fadeToColor = Color.Blue;
            }

            var part = Particle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
            part.Scale *= 4;
            part.shrink = true;
            part.noStretch = true;
            for (float f = 0; f < 3; f++)
            {
                float radius = 800;
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                Vector2 velocity = NPC.Center - spawnPos;
                velocity = velocity.SafeNormalize(Vector2.Zero);
                velocity *= Main.rand.NextFloat(16, 64);
                FXUtil.GlowStretch(spawnPos, velocity);
            }

            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                pVelocity *= Main.rand.NextFloat(0.5f, 8f);
                var spark = Particle.NewParticle<EmberParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
            }

            SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot");
            SoundEngine.PlaySound(crackSound, NPC.position);
        }
        private void AI_Spawn()
        {
            if (!_focusOn)
            {
                FXUtil.FocusCamera(NPC.Center, 400);
                _focusOn = true;
            }
            Timer++;
            if (Timer == 1)
            {
                SpawnPulse();
            }
            float targetScale = MathHelper.Lerp(0f, 1f, AttackCounter / 4f);
            _spawnScale = MathHelper.Lerp(_spawnScale, targetScale, 0.1f);

            if (Timer >= 102)
            {
                Timer = 0;
                AttackCounter++;

                if (AttackCounter >= 3)
                {
                    ShowNamePlate();
                    SpawnPulse();
                    SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn");
                    SoundEngine.PlaySound(crackSound, NPC.position);
                    SwitchState(AIState.Idle);
                }
            }
        }
        private void AI_Idle()
        {
            Timer++;
            if (Timer >= 300)
            {
                ChooseAttack();
            }
        }

        #region Draw Code
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = NPC.scale * Vector2.One * _spawnScale * 2;
            drawPosition += _shakeOffset;

            float spinRotOffset = _spinTimer * -0.01f;
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Main.DiscoColor;
            sparkyShader.Distortion = -0.15f;
            sparkyShader.Time = -Main.GlobalTimeWrappedHourly * 40;
            sparkyShader.Tiling = Vector2.One * 2;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: sparkyShader.Effect);


            var lightTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 lightDrawOrigin = lightTexture.Size() / 2f;

            float sparkyRot = NPC.rotation + spinRotOffset;
            float scaleOsc2 = ExtraMath.Osc(0.4f, 0.5f, speed: 1);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.White * 0.75f, sparkyRot, lightDrawOrigin, drawScale * 3 * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.White * 0.25f, sparkyRot + 0.2f, lightDrawOrigin, drawScale * 8 * scaleOsc2, SpriteEffects.None, 0);


            var shader = SingularityShader.Instance;
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawPosition, null, Color.White, NPC.rotation, drawOrigin, drawScale * 1.5f * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();

            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 2));
            diskDrawColor.A = 0;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.65f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc, SpriteEffects.None, 0);


            for (float f = 0; f < 4; f++)
            {

                spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(1.5f, 0.2f), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(3.5f, 0.2f), SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(7.5f, 0.2f), SpriteEffects.None, 0);


            Texture2D extra67 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_67").Value;
            Vector2 extra67DrawOrigin = extra67.Size() / 2f;
            Color extra67DrawColor = Color.Lerp(Color.White, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 2));
            extra67DrawColor.A = 0;
            spriteBatch.Draw(extra67, drawPosition, null, extra67DrawColor, NPC.rotation, extra67DrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            return false;
        }
        #endregion
    }
}
