using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Trailers;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{
    public class PharaohsBlade : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 12;
            Item.shoot = ModContent.ProjectileType<PharaohsBladeSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<PharaohsBladeStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<GintzlMetal, BlankSword>();
        }
    }


    public class DustStorm : ModProjectile
    {
        public class DustStormPointComparer : IComparer<DustStormPoint>
        {
            public int Compare(DustStormPoint x, DustStormPoint y)
            {
                return x.offset.Y.CompareTo(y.offset.Y);
            }
        }
 
        private float _inScale;
        public struct DustStormPoint
        {
            public Color color;
            public Vector3 offset;
            public float rotation;
        }
        private ref float Timer => ref Projectile.ai[0];
        private bool IsBig
        {
            get => Projectile.ai[1] == 1;
        }

        private ref float ExtraLifeTime => ref Projectile.ai[2];
        private DustStormPointComparer _dustStormComparer;
        private DustStormPoint[] _dustPointsBackingField;
        private DustStormPoint[] DustPoints
        {
            get
            {
                if (_dustPointsBackingField == null)
                {
                    int dustPoints = 32;
                    _dustPointsBackingField = new DustStormPoint[dustPoints];
                    for (int i = 0; i < dustPoints; i++)
                    {
                        DustStormPoint stormPoint = new DustStormPoint();
                        stormPoint.color = Color.SandyBrown;

                        Vector3 offset = new Vector3();
                        offset.Y = Main.rand.NextFloat(-1f, 0);

                        float completionRatio = (float)i / (float)dustPoints;
                        offset.X = MathHelper.Lerp(-1f, 1f, completionRatio);

                        offset.Z = Main.rand.NextFloat(-2f, 2f);
                        stormPoint.offset = offset;
                        stormPoint.rotation = Main.rand.NextFloat(-1f, 1f);
                        _dustPointsBackingField[i] = stormPoint;
                    }

                }
                return _dustPointsBackingField;
            }
        }
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 80;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            base.AI();
            foreach(var npc in Main.ActiveNPCs)
            {
                if (npc.friendly)
                    continue;

                float distanceToTarget = Vector2.Distance(Projectile.Center, npc.Center);
                if(distanceToTarget <= 128)
                {
                    Vector2 vel = (Projectile.Center - npc.Center);
                    npc.velocity += vel.SafeNormalize(Vector2.Zero) * 0.05f;
             
                }
    
            }
            Timer++;
            if(Timer == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Projectile.position);
            }

            if (IsBig)
            {

                if (Main.myPlayer == Projectile.owner)
                {

                    bool manaIsAvailable = Owner.CheckMana(2, false, false);

                    // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                    // player.channel indicates whether the player is still holding down the mouse button to use the item.
                    bool stillInUse = manaIsAvailable;
                    if (stillInUse && Timer % 4 == 0)
                    {
                        Owner.CheckMana(4, true, false);
                        ExtraLifeTime = 4;
                        Projectile.netUpdate = true;
                    }
                }
            }

            if(ExtraLifeTime > 0)
            {
                Projectile.timeLeft += (int)ExtraLifeTime;
                ExtraLifeTime = 0;
            }

            if (Timer % 16 == 0)
            {
                var ember = LegacyParticle.NewParticle<EmberParticle>(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2) - Vector2.UnitY * 4);
                ember.innerColor = Color.White;
                ember.outerColor = Color.Tan;
                ember.fadeToColor = Color.DarkBlue;
            }
            float radiansToRotateBy = 0.1f;
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(new Vector3(0, -1, 0), radiansToRotateBy);
            Matrix rotationMatrix = Matrix.CreateFromQuaternion(quaternion);
            for (int i = 0; i < DustPoints.Length; i++)
            {
                ref DustStormPoint dustPoint = ref DustPoints[i];
                Vector3 oldOffset = dustPoint.offset;
                dustPoint.offset = Vector3.Transform(dustPoint.offset, rotationMatrix);
                dustPoint.rotation += Main.rand.NextFloat(-0.2f, 0.2f);
                if (Main.rand.NextBool(64))
                {

                    Dust.NewDustPerfect(Projectile.Center + new Vector2(dustPoint.offset.X * 54, dustPoint.offset.Y), DustID.Sand);
                }
            }
            Projectile.extraUpdates = 1;
            Projectile.velocity.X *= 0.97f;
            Projectile.velocity.Y += 0.5f;
         
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            _dustStormComparer ??= new DustStormPointComparer();
            Array.Sort(DustPoints, _dustStormComparer);

            float inScale = Timer / 15f;
            inScale = EasingFunction.InOutSine(inScale);

            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);

            float scale = inScale * outScale;

            float sizer = 0.5f;

            if (IsBig)
            {
                sizer = 1f;
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            
            for (int i = 0; i < DustPoints.Length; i++)
            {
                ref DustStormPoint dustPoint = ref DustPoints[i];
                Vector2 o = new Vector2(dustPoint.offset.X, dustPoint.offset.Y);
                o.X *= 54 * o.Y * scale * sizer; 
                o.Y *= 0;
  
                Vector2 drawPosition = Projectile.Center + o - Main.screenPosition;
                drawPosition.Y += ExtraMath.Osc(0f, -16, speed: 4, offset:i);
                Color drawColor = dustPoint.color.MultiplyRGB(lightColor);

                Vector2 drawScale = Vector2.One * (dustPoint.offset.Z * 0.5f + 0.5f) * 2.2f;
                drawScale *= scale * sizer;
                spriteBatch.Draw(texture, drawPosition, null, drawColor * 0.1f, dustPoint.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }



            List<Vector2> dustPoints = new List<Vector2>();
            float numPoints = 32;
            for(float n = 0; n < numPoints; n++)
            {
                dustPoints.Add(Vector2.Lerp(Projectile.Center, Projectile.Center - Vector2.UnitY * 252 * sizer, n / numPoints));
            }
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.SandyBrown, Color.Tan, 0.5f);
            shader.NoiseColor = Color.SandyBrown;
            shader.OutlineColor = Color.SandyBrown;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 4;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;
            TrailDrawer.Draw(Main.spriteBatch, dustPoints.ToArray(), ColorFunction, WidthFunction, shader);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            float sizer = 0.5f;

            if (IsBig)
            {
                sizer = 1f;
            }
            return MathHelper.SmoothStep(45, 190, completionRatio) * sizer;
        }

        public Color ColorFunction(float completionRatio)
        {
            float inScale = Timer / 15f;
            inScale = EasingFunction.InOutSine(inScale);

            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);

            float scale = inScale * outScale;
            return Color.Lerp(Color.Transparent, Color.Tan, EasingFunction.QuadraticBump(completionRatio)) * 0.5f * scale;

        }
    }


    public class PharaohsBladeSlash : BaseSwingProjectileV2
    {
        private bool _shotProjectile;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);
            Trailer = new DesertBlazingTrail();
            useAfterImage = true;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Tan * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_shotProjectile)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Projectile.velocity,
                    ModContent.ProjectileType<DustStorm>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.WindCast2;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, target.position);
                _shotProjectile = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 1f);
            }
        }
    }

    public class PharaohsBladeStaminaSlash : BaseSwingProjectileV2
    {
        private bool _shotProjectile;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;
            Add(new OvalSwing
            {
                Duration = 48,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 222,
                Easing = EasingFunction.Anticipation2,
                Sound = swingSound1,
            });

            Trailer = new DesertBlazingTrail();
            useAfterImage = true;
        }
        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Tan * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
            if (!_shotProjectile && Interpolant >= 0.3f && this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                    ModContent.ProjectileType<DustStorm>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
                _shotProjectile = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;
        }
    }
}
