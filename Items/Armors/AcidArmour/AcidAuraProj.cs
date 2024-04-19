using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.AcidArmour
{
    internal class AcidAuraProj : ModProjectile
    {
        private enum ActionState
        {
            Idle,
            Kill
        }
        public override string Texture => TextureRegistry.EmptyTexture;

        private float _fade;
        private ActionState State
        {
            get => (ActionState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }


        public override void SetDefaults()
        {
            Projectile.width = 312;
            Projectile.height = 312;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Kill:
                    AI_Kill();
                    break;
            }
  
            Projectile.rotation += MathHelper.PiOver4 / 32;
        }

        private void AI_Idle()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            if (player.velocity != Vector2.Zero)
            {
                State = ActionState.Kill;
            }
            _fade += 0.01f;
            _fade = MathHelper.Clamp(_fade, 0, 1);
        }

        private void AI_Kill()
        {
            _fade -= 0.01f;
            _fade = MathHelper.Clamp(_fade, 0, 1);
            if(_fade <= 0)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiation>(), 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string texture = "Stellamod/Effects/Masks/Extra_67";
            Texture2D maskTexture = ModContent.Request<Texture2D>(texture).Value;

            Vector2 textureSize = maskTexture.Size();
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = VectorHelper.Osc(0, 1);
            float alpha = VectorHelper.Osc(0.5f, 1);
            Color color = Color.Lerp(Color.LightGreen, Color.DarkSeaGreen, progress) * _fade;
            Color drawColor = Color.Multiply(new(color.R, color.G, color.B, 0), alpha);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            Main.spriteBatch.Draw(maskTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return base.PreDraw(ref lightColor);
        }
    }
}
