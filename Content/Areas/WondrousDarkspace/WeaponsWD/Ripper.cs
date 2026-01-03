using Microsoft.Xna.Framework;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Swords;
using Stellamod.Trailing;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class Ripper : BaseSwingItemV2
    {

        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 18;

            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Green;
            Item.knockBack = 6;
            Item.shoot = ModContent.ProjectileType<RipperSlash>();
            Item.shootSpeed = 20;
            staminaProjectileShoot = ModContent.ProjectileType<RipperSwordProj>();
            meleeWeaponType = MeleeWeaponType.Sword;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<HypnotizedSoul>());
        }
    }

    public class RipperSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);
            Trailer = TrailPresets.Miracle;
            useAfterImage = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                Vector2 position = target.Center;
                Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                         lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                    }
                }
                _hit = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;

            }
        }
    }






    public class RipperSwordProj : ModProjectile
    {
        private Vector2 _targetCenter;
        private Vector2 _velocity;
        private const int Freeze = 45;
        private const int Fire = 80;

        ref float AttackNum => ref Projectile.ai[1];
        ref float Spawner => ref Projectile.ai[2];
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(_targetCenter);
            writer.WriteVector2(_velocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            _targetCenter = reader.ReadVector2();
            _velocity = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 54;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 112;
            Projectile.extraUpdates = 1;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement

            //Accelerate to being on top of the player
            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
                if (Projectile.velocity.X > distX)
                    Projectile.velocity.X = distX;

            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
                if (Projectile.velocity.X < distX)
                    Projectile.velocity.X = distX;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
                if (Projectile.velocity.Y > distY)
                    Projectile.velocity.Y = distY;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
                if (Projectile.velocity.Y < distY)
                    Projectile.velocity.Y = distY;
            }
        }

        public override void AI()
        {
            ref float ai_Counter = ref Projectile.ai[0];
            if (ai_Counter == 0 && Main.myPlayer == Projectile.owner)
            {
                float radius = 384;
                Player owner = Main.player[Projectile.owner];
                _targetCenter = owner.Center + Main.rand.NextVector2Circular(128, 128);
                _targetCenter -= Vector2.UnitY * 256;
                Projectile.netUpdate = true;
            }

            ai_Counter++;
            if(Spawner == 0 && ai_Counter % 16 == 0)
            {
                if (AttackNum < 7)
                {
                    if(Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY, Type, 
                            Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: 1);
                    }
                  
                    AttackNum++;
                }
            }

      

            if (ai_Counter >= Fire)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 targetVelocity = (Main.MouseWorld - Projectile.Center);
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    targetVelocity *= 25;
                    Projectile.velocity = targetVelocity;
                    Projectile.netUpdate = true;
                }

                float targetRotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.4f);
            }
            else if (ai_Counter > Freeze)
            {
                float targetRotation = _velocity.ToRotation() + MathHelper.ToRadians(45);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.4f);
            }
            else if (ai_Counter == Freeze)
            {
                //I made the projectile just move super slow when it spawned, so gotta do this to return to normal speed.
                Projectile.velocity = Vector2.Zero;
                if (Main.myPlayer == Projectile.owner)
                {
                    _velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * 45;
                    Projectile.netUpdate = true;
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit"), Projectile.position);
            }
            else if (ai_Counter < Freeze)
            {
                AI_Movement(_targetCenter, 25, 5);
                Projectile.rotation += ai_Counter * 0.01f;
            }

            Visuals();
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            this.DrawCentered(ref lightColor);
            return false;
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(Projectile.Center, 2, 2, DustID.GemAmethyst);
            }

            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage *= 3;
        }

        public override void OnKill(int timeLeft)
        {
            for (float f = 0; f < 2f; f++)
            {
                float progress = f / 2f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                particle.InnerColor = Color.LightPink;
                particle.GlowColor = Color.Purple;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                particle.VectorScale *= 0.5f;

            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(1, 1),
              ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner,
              ai1: Projectile.velocity.ToRotation() + MathHelper.ToRadians(45));
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 3f);
                d.noGravity = true;
            }
        }
    }




    public class RipperSlashProjBig : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 110;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            //Returning false here makes the position not change
            return false;
        }

        public override bool PreAI()
        {
            Projectile.ai[0]++;
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.ai[0] <= 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RipperSlash1");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                FXUtil.GlowCircleBoom(Projectile.Center, Color.LightPink, Color.Purple, Color.Violet);
                FXUtil.ShakeCamera(Projectile.Center, 512, 8);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 7)
                {
                    Projectile.active = false;
                }
            }

            return true;
        }
    }



    public class RipperSlashProjSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 192;
            Projectile.height = 192;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 110;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override bool PreAI()
        {
            Projectile.ai[0]++;
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.ai[0] <= 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RipperSlash2");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 7)
                {
                    Projectile.active = false;
                }


            }
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.ShimmerSplash, 0, 60, 133);
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
