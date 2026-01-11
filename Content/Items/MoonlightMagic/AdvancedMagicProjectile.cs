using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class AdvancedMagicProjectile : ModProjectile,
        IProjectileNetID
    {
        private BaseElement _baseElement;
        private BaseMovement _movement;
        private int _netID;
        private int _numUpdates;
        private float _bounceCooldownTimer;
        public override string Texture => TextureRegistry.EmptyTexture;

        public ref float GlobalTimer => ref Projectile.ai[0];
        private ref float Charge => ref Projectile.ai[1];

        public Vector2[] OldPos { get; private set; }
        public float[] OldRot { get; private set; }
        public float Size { get; set; } = 16;
        public float ChargeSizeMultiplier { get; set; } = 1f;
        public float ScaleMultiplier => ((Size / 16f) * MathHelper.Lerp(0.5f, 1f, Charge) * ChargeSizeMultiplier) + extraScale;
        public int TrailLength { get; set; }

        public bool IsClone { get; set; }
        public Texture2D Form { get; set; }
        public BaseMovement Movement
        {
            get => _movement;
            set
            {
                _movement = value;
                if (_movement != null)
                    _movement.MagicProj = this;
            }
        }

        public BaseElement PrimaryElement
        {
            get => _baseElement;
            set
            {
                _baseElement = value;
                if (_baseElement != null)
                    _baseElement.MagicProj = this;
            }
        }
        public List<BaseEnchantment> Enchantments { get; private set; } = new List<BaseEnchantment>();
        private Player Owner => Main.player[Projectile.owner];
        private AdvancedMagicPlayer MagicPlayer => Owner.GetModPlayer<AdvancedMagicPlayer>();
        public bool damagingTrail;
        public bool laserLike;
        public bool isDying;
        public float extraScale;
        public float extraRotation;
        public float killTime = 60f;
        public int tileHitCount;
        public int stickToTarget;
        public bool spellInteract;
        public Vector2 stickyOffset;
        public Vector2 originalVelocity;
        public float coasterTime;
        public bool orb;
        public int hitboxSize;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int size = (int)(Size + hitboxSize);
            int halfSize = size / 2;
            int x = (int)Projectile.Center.X - halfSize;
            int y = (int)Projectile.Center.Y - halfSize;
            Rectangle myHitbox = new Rectangle(x, y, size, size);
            if (damagingTrail)
            {

                return ProjectileHelper.OldPosColliding(OldPos, projHitbox, targetHitbox, 32);
            }


            return base.Colliding(myHitbox, targetHitbox);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileSets.ResetBossMultihitDamageFalloff[Type] = true;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public int GetNetID()
        {
            return _netID;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_netID);
            writer.Write(_bounceCooldownTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _netID = reader.ReadInt32();
            _bounceCooldownTimer = reader.ReadSingle();
        }

        public void ReplaceEnchantment(BaseEnchantment enchantmentPrefab, int index)
        {
            BaseEnchantment prefab = (ModContent.GetModItem(enchantmentPrefab.Type) as BaseEnchantment);
            if (prefab == null)
                return;
            var instance = prefab.Instantiate();
            instance.MagicProj = this;
            instance.SetMagicDefaults();
            Enchantments[index] = instance;
        }

        public void AddEnchantment(BaseEnchantment enchantmentPrefab)
        {
            BaseEnchantment prefab = (ModContent.GetModItem(enchantmentPrefab.Type) as BaseEnchantment);
            if (prefab == null)
                return;
            var instance = prefab.Instantiate();
            instance.MagicProj = this;
            instance.SetMagicDefaults();
            Enchantments.Add(instance);
        }

        public int IndexOfEnchantment(BaseEnchantment enchantment)
        {
            return Enchantments.IndexOf(enchantment);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
            Projectile.light = 0.78f;

        }

        public void SetMoonlightDefaults(AdvancedMagicProjectile item)
        {

            Projectile.width = Projectile.height = (int)item.Size;
            if (item.PrimaryElement == null || item.PrimaryElement is not BaseElement)
                PrimaryElement = new BasicElement();
            else
                PrimaryElement = (item.PrimaryElement as BaseElement).Instantiate();
            Movement = item.Movement;
            Form = item.Form;
            Enchantments.Clear();
            tileHitCount = 1;
            stickToTarget = -1;
            coasterTime = 0;
            hitboxSize = 0;
            var enchantments = item.Enchantments;
            for (int i = 0; i < enchantments.Count; i++)
            {
                var enchantmentTemplate = enchantments[i];
                if (enchantmentTemplate == null)
                    continue;

                var modItem = enchantments[i];
                if (modItem is BaseEnchantment enchantment)
                {
                    var instance = (ModContent.GetModItem(enchantment.Type) as BaseEnchantment).Instantiate();
                    instance.MagicProj = this;
                    instance.SetMagicDefaults();
                    Enchantments.Add(instance);
                }
            }

            OldPos = new Vector2[TrailLength];
            OldRot = new float[TrailLength];

            if (Main.myPlayer == Projectile.owner)
            {
                _netID = ProjectileNetIDHelper.RegisterID();
                Projectile.netUpdate = true;
            }

        }

        public void SetMoonlightDefaults(AbstractMagicWand item)
        {
            ChargeSizeMultiplier = 1 + MagicPlayer.chargeWidthBonus;
            Projectile.width = Projectile.height = item.Size;
            if (item.primaryElement == null || item.primaryElement.ModItem is not BaseElement || item.primaryElement.IsAir)
                PrimaryElement = new BasicElement();
            else
                PrimaryElement = (item.primaryElement.ModItem as BaseElement).Instantiate();
            Movement = item.Movement;
            Form = item.Form;
            Enchantments.Clear();
            tileHitCount = 1;
            stickToTarget = -1;
            coasterTime = 0;
            hitboxSize = 0;
            var enchantments = item.GetEquippedEnchantments(Owner);
            for (int i = 0; i < enchantments.Length; i++)
            {
                var enchantmentTemplate = enchantments[i];
                if (enchantmentTemplate == null)
                    continue;

                var modItem = enchantments[i].ModItem;
                if (modItem is BaseEnchantment enchantment)
                {
                    var instance = enchantment.Instantiate();
                    instance.MagicProj = this;
                    instance.SetMagicDefaults();
                    Enchantments.Add(instance);
                }
            }


            OldPos = new Vector2[TrailLength];
            OldRot = new float[TrailLength];
        }

        private void AI_HandleTargetSticking()
        {
            if (stickToTarget == -1)
                return;
            //target sticking functionality
            NPC targetToStickTo = Main.npc[stickToTarget];
            if (targetToStickTo.active)
            {
                Vector2 velocityToTarget = targetToStickTo.Center - (Projectile.Center + stickyOffset);
                Projectile.velocity = velocityToTarget;
            }
            else
            {
                stickToTarget = -1;
            }
        }

        private void AI_DustEffects()
        {
            if (GlobalTimer % (Projectile.extraUpdates + 1) == 0)
            {
                if (PrimaryElement != null)
                {
                    PrimaryElement.DustEffects();
                }
            }
        }

        private void AI_BounceIfTouchSpell()
        {
            //Cooldown so it doesn't spam
            if(_bounceCooldownTimer > 0)
            {
                _bounceCooldownTimer--;
                return;
            }
         
            if (!this.OwnedByLocalClient())
                return;
            if (!spellInteract)
                return;

            Rectangle myRect = Projectile.getRect();
            foreach (var p in Main.ActiveProjectiles)
            {
                if (p.type != Projectile.type)
                    continue;
                if (p == Projectile)
                    continue;

                Rectangle otherRect = p.getRect();
                if (Projectile.Colliding(myRect, otherRect))
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    float scale = Main.rand.NextFloat(0.3f, 0.5f);

                    Vector2 bounceVelocity = -Projectile.velocity * 1.5f;
                    Projectile.velocity = bounceVelocity.RotatedByRandom(MathHelper.PiOver4 / 4);
                    Projectile.netUpdate = true;
                    _bounceCooldownTimer = 20;
                }
            }
        }

        public override void AI()
        {
            base.AI();

            GlobalTimer++;
            if (GlobalTimer == 1)
            {
                originalVelocity = Projectile.velocity;
                if (!Owner.HeldItem.IsAir && Owner.HeldItem.ModItem != null)
                {
                    AbstractMagicWand staff = Owner.HeldItem.ModItem as AbstractMagicWand;
                    TrailLength = staff.TrailLength;
                    Size = staff.Size;
                    SetMoonlightDefaults(staff);
                }
            }
    
            PrimaryElement?.AI();
            Movement?.AI();

            if (GlobalTimer == 1)
            {
                if (PrimaryElement != null)
                {
                    SoundEngine.PlaySound(PrimaryElement.CastSound, Projectile.position);
                }
            }

            //Set default extra updates
            //So we can modify stacking freely with enchantments
            Projectile.extraUpdates = 0;
            damagingTrail = false;
            for (int i = 0; i < Enchantments.Count; i++)
            {
                var enchantment = Enchantments[i];
                enchantment?.AI();
            }
            AI_HandleTargetSticking();
            AI_DustEffects();
            AI_BounceIfTouchSpell();

            if (isDying)
            {
                if (Projectile.timeLeft > killTime)
                    Projectile.timeLeft = (int)killTime;
            }
                
            if (laserLike && _numUpdates > 2)
            {
                if(GlobalTimer % (Projectile.extraUpdates+1) == 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        Vector2 start = OldPos[_numUpdates - 1];
                        for (float f = 0; f < 1; f++)
                        {
                            Vector2 vel = -(OldPos[_numUpdates - 1] - OldPos[_numUpdates - 2]);
                            vel = vel.SafeNormalize(Vector2.Zero);
                            DustParticle dp = Particle<DustParticle>.Spawn(start, vel.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(2, 8f),
                                Scale: Main.rand.NextFloat(0.5f, 1f));
                            Color color = PrimaryElement == null ? Color.White : PrimaryElement.GetElementColor();
                            dp.innerColor = Color.Lerp(color, Color.White, 0.5f);
                            dp.outerColor = color;
                            dp.gravity = 0.06f;
                        }

                    }

                }

            }
            if (laserLike && _numUpdates >= TrailLength)
            {
                //Projectile.velocity = Vector2.Zero;
                Projectile.Center = OldPos[0];
                return;
            }

            for (int i = OldPos.Length - 1; i > 0; i--)
            {
                OldPos[i] = OldPos[i - 1];
                OldRot[i] = OldRot[i - 1];
            }
            if (OldPos.Length > 0)
                OldPos[0] = Projectile.Center;
            if (OldRot.Length > 0)
                OldRot[0] = Projectile.rotation;
          
            _numUpdates++;
            if (TrailLength != OldPos.Length)
            {
                float[] newRot = new float[TrailLength];
                Vector2[] newTrail = new Vector2[TrailLength];
                for (int i = 0; i < OldPos.Length && i < newTrail.Length; i++)
                {
                    newTrail[i] = OldPos[i];
                    newRot[i] = OldRot[i];
                }
                OldPos = newTrail;
                OldRot = newRot;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (target.boss)
            {
                float damage = Projectile.damage;
                damage *= 0.5f;
                Projectile.damage = (int)damage;
            }

            PrimaryElement?.OnHitNPC(target, hit, damageDone);
            for (int i = 0; i < Enchantments.Count; i++)
            {
                var enchantment = Enchantments[i];
                enchantment.OnHitNPC(target, hit, damageDone);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            PrimaryElement?.OnKill();
            for (int i = 0; i < Enchantments.Count; i++)
            {
                var enchantment = Enchantments[i];
                enchantment.OnKill(timeLeft);
            }

            if (PrimaryElement != null)
            {
                SoundEngine.PlaySound(PrimaryElement.HitSound, Projectile.position);
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool shouldKill = true;
            tileHitCount--;
            for (int i = 0; i < Enchantments.Count; i++)
            {
                var enchantment = Enchantments[i];
                bool allowKill = enchantment.OnTileCollide(oldVelocity);
                if (!allowKill)
                {
                    shouldKill = false;
                }
            }
            if (shouldKill && laserLike)
                isDying = true;
            else if (shouldKill)
                Projectile.Kill();
                return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Form != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;

                Color drawColor = Color.White.MultiplyRGB(lightColor);
                float scale = Projectile.scale * MathHelper.Lerp(0.5f, 1f, Charge) * ScaleMultiplier;

                Vector2 vel = Projectile.velocity;
                if (_numUpdates > 3 && OldPos.Length > 5)
                {
                    vel = OldPos[0] - OldPos[1];

                }
                float rot = vel.ToRotation();
                if (orb)
                    scale *= 1.05f;
                PrimaryElement?.DrawForm(spriteBatch, Form, Projectile.Center - Main.screenPosition,
                    drawColor, drawColor, rot + extraRotation, scale);
            }

            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlashes, DrawLayer.OverNPCsWithOutline);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated, DrawLayer.OverNPCs);

            return false;
        }
        /*
        private void DrawPixelatedOrb(GraphicsDevice graphicsDevice)
        {
            TrailVertexHelper trailVertexHelper = ModContent.GetInstance<TrailVertexHelper>();
            trailVertexHelper.CreateCircleVertices(Projectile.Center, 80, 16,
                out VertexPositionColorTexture[] vertices, out int[] indices);
            PrimaryElement?.DrawOrbCircle(vertices, indices);
        }*/

        private void DrawPixelatedFlashes(SpriteBatch spriteBatch, Vector2 screenPos)
        {

            if (laserLike && _numUpdates > 2)
            {
                Texture2D muzzleFlash = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash").Value;
                Color glowColor = PrimaryElement == null ? Color.White : PrimaryElement.GetElementColor();
                glowColor.A = 0;
                Vector2 centerpos = OldPos[_numUpdates - 1] - Main.screenPosition;

                float outScale = (float)(Projectile.timeLeft) / 60f;
                outScale = EasingFunction.InOutSine(outScale);

                Vector2 vel = OldPos[_numUpdates - 1] - OldPos[_numUpdates - 2];
                float rot = vel.ToRotation();
                for (int i = 0; i < 4; i++)
                {
                    float ratio = (float)i / 4f;
                    float scale = 0.27f * (7 + 0.6f) * VectorHelper.Osc(0.75f, 1f, speed: 3) * ratio;
                    Vector2 muzzleScale = Vector2.One;
                    muzzleScale.Y *= 2.5f;


                    Main.spriteBatch.Draw(muzzleFlash, centerpos, null, glowColor, rot,
                        muzzleFlash.Size() / 2f, muzzleScale * scale * outScale * 0.5f, SpriteEffects.None, 0f);
                }

                Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
                glowColor = PrimaryElement == null ? Color.White : PrimaryElement.GetElementColor();
                glowColor.A = 0;
                centerpos = OldPos[_numUpdates - 1] - Main.screenPosition;

                outScale = (float)(Projectile.timeLeft) / 60f;
                outScale = EasingFunction.InOutSine(outScale);
                for (int i = 0; i < 6; i++)
                {
                    float ratio = (float)i / 6f;
                    float scale = 0.27f * (7 + 0.6f) * VectorHelper.Osc(0.75f, 1f, speed: 3) * ratio;

                    Main.spriteBatch.Draw(texture2D4, centerpos, null, glowColor, rot,
                        new Vector2(32, 32), scale * outScale, SpriteEffects.None, 0f);
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            PrimaryElement?.DrawTrail(OldPos);


        }

        public float GetTrailLaserWidth(float completionRatio)
        {
            float inScale = (float)_numUpdates / 60f;
            inScale = EasingFunction.InOutSine(inScale);
            float outScale = (float)Projectile.timeLeft / killTime;
            outScale = EasingFunction.InOutSine(outScale);
            float baseWidth = 40;

            float inLocal = MathHelper.Lerp(0f, 1f, MathHelper.Clamp(completionRatio / 0.2f, 0f, 1f));

            float width = baseWidth * inScale * outScale * ScaleMultiplier * inLocal;
            return width;
        }
    }
}
