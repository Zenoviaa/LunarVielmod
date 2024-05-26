using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Buffs.Whipfx;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Orbs
{
    internal class BlackBallProj : OrbProjectile
    {
        public enum ActionState
        {
            Orbit,
            Swing_1,
            Swing_2,
            Swing_3
        }

        public const float Swing_Time = 20 * Swing_Speed_Multiplier;
        public const float Swing_Time_2 = 64 * Swing_Speed_Multiplier;
        public const float Final_Swing_Distance = 252;
        public const float Combo_Time = 8;
        public const int Swing_Speed_Multiplier = 8;

        public override float MaxThrowDistance => 384;

        ref float ComboCounter => ref Projectile.ai[0];
        public ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        ref float Timer => ref Projectile.ai[2];
        float SwingTime;
        float EasedProgress;
        float OrbitRotation;
        Vector2 SwingStart;
        Vector2 SwingTarget;
        Vector2 SwingVelocity;
        int DustTimer;
        int ComboCounter2;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(SwingStart);
            writer.WriteVector2(SwingTarget);
            writer.WriteVector2(SwingVelocity);
            writer.Write(SwingTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingStart = reader.ReadVector2();
            SwingTarget = reader.ReadVector2();
            SwingVelocity = reader.ReadVector2();
            SwingTime = reader.ReadSingle();
        }


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = Swing_Speed_Multiplier;
        }

        public override bool? CanDamage()
        {
            //Only deal damage while swinging
            return State != ActionState.Orbit;
        }

        public override void AI()
        {
            //Kill yourself if not holding the item
            if (!Owner.HasBuff(ModContent.BuffType<OrbMaster>()))
            {
                Projectile.Kill();
                return;
            }

            switch (State)
            {
                case ActionState.Orbit:
                    Orbit();
                    break;
                case ActionState.Swing_1:
                    Swing1();
                    break;
                case ActionState.Swing_2:
                    Swing2();
                    break;
                case ActionState.Swing_3:
                    Swing3();
                    break;
            }
        }

        private void Orbit()
        {
            //Orbiting
            //Orbit around the player
            float orbitDistance = 256;
            OrbitRotation += 0.003f;
            Vector2 targetOrbitPos = MovementHelper.OrbitAround(Owner.Center, Vector2.UnitY, orbitDistance, OrbitRotation);

            //Lerp
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetOrbitPos, 0.12f / Swing_Speed_Multiplier);
            if (ComboCounter >= 1)
            {
                OrbHelper.PlaySummonSound(Projectile.position, 0.5f);
                Reset();
                SwingVelocity = Owner.DirectionTo(SwingTarget);
                SwingStart = Owner.Center;
                if (Main.myPlayer == Projectile.owner)
                {
                    SwingTarget = GetSwingTarget();
                    Projectile.netUpdate = true;
                }

                SwingTime = Swing_Time;
                State = ActionState.Swing_1;
                ComboCounter = 0;
                Timer = 0;
            }
        }

        private void Reset()
        {
            EasedProgress = 0;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }

        private void SwingDusts()
        {
            DustTimer++;
            if (DustTimer >= 4 * Swing_Speed_Multiplier)
            {
                DustTimer = 0;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GunFlash>(), newColor: ColorFunctions.MiracleVoid, Scale: 0.8f);
            }

            if (Main.rand.NextBool(3 * Swing_Speed_Multiplier))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch);
                Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                ParticleManager.NewParticle(Projectile.Center, speed * 1, ParticleManager.NewInstance<VoidParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
            }
        }

        private void Swing1()
        {
            SwingDusts();
            Timer++;

            float progress = Timer / SwingTime;
            EasedProgress = Easing.SpikeInOutExpo(progress);

            float dir = ComboCounter2 % 2 == 0 ? -1 : 1;
            float rot = MathHelper.Lerp(MathHelper.Pi * dir, 0, EasedProgress);

            Vector2 start = SwingStart;
            Vector2 end = SwingTarget.RotatedBy(rot, SwingStart);
            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);

            Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
            if (Timer > SwingTime)
            {
                if (ComboCounter >= 1)
                {
                    OrbHelper.PlaySummonSound(Projectile.position, 0.5f);
                    Reset();
                    SwingVelocity = Owner.DirectionTo(SwingTarget);
                    float distance = 180;
                    SwingStart = Owner.Center + SwingVelocity.RotatedBy(MathHelper.Pi) * distance;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        SwingTarget = GetSwingTarget();
                        Projectile.netUpdate = true;
                    }

                    SwingTime = Swing_Time;
                    ComboCounter2++;
                    if (ComboCounter2 >= 7)
                    {
                        ComboCounter2 = 0;
                        SwingTime = Swing_Time_2;
                        State = ActionState.Swing_3;
                    }
                    else
                    {
                        SwingTime = Swing_Time;
                        State = ActionState.Swing_1;
                    }
                    ComboCounter = 0;
            
                    Timer = 0;
                }
                else if (Timer > SwingTime + Combo_Time)
                {
                    ComboCounter = 0;
                    Timer = 0;
                    State = ActionState.Orbit;
                }
            }
        }

        private void Swing2()
        {
            SwingDusts();
            Timer++;

            float progress = Timer / SwingTime;
            EasedProgress = Easing.SpikeInOutExpo(progress);

            Vector2 start = Owner.Center;
            Vector2 end = SwingTarget + (SwingTarget - start).SafeNormalize(Vector2.Zero) * Final_Swing_Distance;

            //This should be cool
            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);
            lerpPosition += MathF.Sin(EasedProgress * 32) * (SwingVelocity.RotatedBy(MathHelper.PiOver2)) * 64;

            Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
            if (Timer > SwingTime)
            {
                if (ComboCounter >= 1)
                {
                    OrbHelper.PlaySummonSound(Projectile.position, 0.5f);
                    Reset();
                    SwingVelocity = Owner.DirectionTo(SwingTarget);
                    SwingStart = Projectile.Center;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        SwingTarget = GetSwingTarget();
                        Projectile.netUpdate = true;
                    }



                    ComboCounter2++;
                    if (ComboCounter2 >= 7)
                    {
                        ComboCounter2 = 0;
                        SwingTime = Swing_Time_2;
                        State = ActionState.Swing_3;
                    }
                    else
                    {
                        SwingTime = Swing_Time;
                        State = ActionState.Swing_1;
                    }

                    ComboCounter = 0;
                    Timer = 0;
                }
                else if (Timer > SwingTime + Combo_Time)
                {
                    ComboCounter = 0;
                    Timer = 0;
                    State = ActionState.Orbit;
                }
            }
        }

        private void Swing3()
        {
            SwingDusts();
            Timer++;
            float progress = Timer / SwingTime;
            EasedProgress = Easing.SpikeInOutExpo(progress);

            Vector2 start = Owner.Center;
            Vector2 end = SwingTarget + (SwingTarget - start).SafeNormalize(Vector2.Zero) * Final_Swing_Distance;
            Vector2 lerpPosition = Vector2.Lerp(start, end, EasedProgress);

            Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPosition, 0.54f / Swing_Speed_Multiplier);
            if (Timer > SwingTime)
            {
                ComboCounter = 0;
                Timer = 0;
                State = ActionState.Orbit;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(ModContent.BuffType<BlackballDebuff>(), 240);
            switch (State)
            {
                case ActionState.Swing_1:
                case ActionState.Swing_2:
                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                            ModContent.DustType<GunFlash>(), Scale: 0.8f);
                    }

                    for(int i = 0; i < 4; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                        ParticleManager.NewParticle(Projectile.Center, speed * 1, 
                            ParticleManager.NewInstance<VoidParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
                    }

                    SoundStyle soundStyle = SoundID.DD2_WitherBeastDeath;
                    soundStyle.PitchVariance = 0.15f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                    break;

                case ActionState.Swing_3:
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Black, 1f).noGravity = true;
                    }

                    for (int i = 0; i < 14; i++)
                    {

                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, ColorFunctions.MiracleVoid, 1f).noGravity = true;
                    }
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + new Vector2(0, -80), Vector2.Zero, ModContent.ProjectileType<KaBoomSigil2>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
          
                    target.SimpleStrikeNPC(Projectile.damage, hit.HitDirection);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower3") { PitchVariance = 0.15f, Pitch = -0.75f }, Projectile.position);
                    break;
            }
        }

        public TrailRenderer SwordSlash;
        public TrailRenderer SwordSlash2;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);


            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StarTrail").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StringTrail").Value;
            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);
            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass,
                    (p) => Vector2.Lerp(new Vector2(28), new Vector2(12), p),
                    (p) => ColorFunctions.MiracleVoid * (1f - p));
                SwordSlash.drawOffset = Projectile.Size / 2f;
            }
            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass,
                    (p) => Vector2.Lerp(new Vector2(28), new Vector2(12), p),
                    (p) => new Color(0, 0, 0, 100) * (1f - p));
                SwordSlash2.drawOffset = Projectile.Size / 2f;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            float[] rotation = new float[Projectile.oldRot.Length];
            for (int i = 0; i < rotation.Length; i++)
            {
                rotation[i] = Projectile.oldRot[i] - MathHelper.ToRadians(45);
            }

            SwordSlash.Draw(Projectile.oldPos, rotation);
            SwordSlash2.Draw(Projectile.oldPos, rotation);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();


            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
