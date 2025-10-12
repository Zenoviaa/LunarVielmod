using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Paint;
using Stellamod.Trails;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Dusts;
using Stellamod.UI.Systems;
using Terraria.Audio;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Guuts
{
    public class SpadedBladeEnchantment : BaseEnchantment
    {
        private bool _hasFired;
        public override float GetStaffManaModifier()
        {
            return 2.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }



        public override void AI()
        {
            base.AI();
            if(!_hasFired && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<SpadedSawblade>(), (int)(Projectile.damage * 0.25f), 0, Projectile.owner, 
                    ai0: MagicProj.GetNetID());
                _hasFired = true;
            }
        }

    }

    public class SpadedSawblade : ModProjectile
    {
        private bool _setParent;
        private int ParentIndex => (int)Projectile.ai[0];
        private Projectile _parent;

        public Projectile GetParent()
        {
            if (ProjectileNetIDHelper.TryFindProjectile(ParentIndex, Projectile.owner, out Projectile parent))
            {
                return parent;
            }
            return Projectile;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            _parent ??= Projectile;
            if (!_setParent)
            {
                Projectile parent = GetParent();
                if(parent != Projectile)
                {
                    _parent = parent;
                    _setParent = true;
                }
            
            }
            Projectile.rotation += 0.2f;

         
            Projectile.Center = _parent.Center;
            if (!_parent.active)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Rectangle drawFrame = Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            float scale = Projectile.scale;
            float rotation = Projectile.rotation;
            Color drawColor = Color.White;
            drawColor *= 0.15f;
            drawColor *= ExtraMath.Osc(0.5f, 1f, speed: 3);
            drawColor.A = 0;
            spriteBatch.Draw(texture, drawPos, drawFrame, drawColor, rotation, drawOrigin, scale, spriteEffects, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            FXUtil.GlowCircleBoom(target.Center,
              innerColor: Color.LightPink,
              glowColor: Color.LightBlue,
              outerGlowColor: Color.Blue, duration: Main.rand.NextFloat(12, 25), baseSize: Main.rand.NextFloat(0.03f, 0.06f));
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Parendine2"), target.position);
            ShakeModSystem.Shake = 4;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.Gray, 0.5f).noGravity = true;
            }

            for (float f = 0; f < 4; f++)
            {
                float progress = f / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(25f, 35f);
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
                var particle = FXUtil.GlowStretch(target.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.Gray;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                particle.VectorScale *= 0.5f;

            }
        }
    }
}
