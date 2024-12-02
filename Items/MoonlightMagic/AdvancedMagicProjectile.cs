using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic
{
    internal class AdvancedMagicProjectile : ModProjectile
    {
        private BaseElement _baseElement;
        private BaseMovement _movement;
        public override string Texture => TextureRegistry.EmptyTexture;

        public Vector2[] OldPos { get; private set; }
        public float[] OldRot { get; private set; }
        public float Size { get; set; } = 16;
        public float ScaleMultiplier => Size / 16f;
        public int TrailLength { get; set; }
        public float GlobalTimer
        {
            get
            {
                return Projectile.ai[0];
            }
            private set
            {
                Projectile.ai[0] = value;
            }
        }
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

        public void ReplaceEnchantment(BaseEnchantment enchantmentPrefab, int index)
        {
            var instance = (ModContent.GetModItem(enchantmentPrefab.Type) as BaseEnchantment).Instantiate();
            instance.MagicProj = this;
            instance.SetMagicDefaults();
            Enchantments[index] = instance;
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
        }

        public void SetMoonlightDefaults(BaseStaff item)
        {
            Projectile.width = Projectile.height = item.Size;
            if (item.primaryElement == null || item.primaryElement.ModItem is not BaseElement || item.primaryElement.IsAir)
                PrimaryElement = new BasicElement();
            else
                PrimaryElement = (item.primaryElement.ModItem as BaseElement).Instantiate();
            Movement = item.Movement;
            Form = item.Form;
            Enchantments.Clear();

            var enchantments = item.equippedEnchantments;
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

        public override void AI()
        {
            base.AI();

            Projectile.width = (int)Size;
            Projectile.height = (int)Size;

            PrimaryElement?.AI();
            Movement?.AI();

            GlobalTimer++;
            if (GlobalTimer == 1)
            {
                if (PrimaryElement != null)
                {
                    SoundEngine.PlaySound(PrimaryElement.CastSound, Projectile.position);
                }
            }

            for (int i = 0; i < Enchantments.Count; i++)
            {
                var enchantment = Enchantments[i];
                enchantment?.AI();
            }

            for (int i = OldPos.Length - 1; i > 0; i--)
            {
                OldPos[i] = OldPos[i - 1];
                OldRot[i] = OldRot[i - 1];
            }
            if (OldPos.Length > 0)
                OldPos[0] = Projectile.position;
            if (OldRot.Length > 0)
                OldRot[0] = Projectile.rotation;
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
            for (int i = 0; i < Enchantments.Count; i++)
            {
                var enchantment = Enchantments[i];
                bool allowKill = enchantment.OnTileCollide(oldVelocity);
                if (!allowKill)
                {
                    shouldKill = false;
                }
            }
            return shouldKill;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PrimaryElement?.DrawTrail();
            if (Form != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                Color drawColor = Color.White.MultiplyRGB(lightColor);
                PrimaryElement?.DrawForm(spriteBatch, Form, Projectile.Center - Main.screenPosition,
                    drawColor, lightColor, Projectile.velocity.ToRotation(), Projectile.scale);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }
    }
}
