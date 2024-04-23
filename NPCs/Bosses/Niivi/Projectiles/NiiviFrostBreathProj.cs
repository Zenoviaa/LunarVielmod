using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostBreathProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private string FrostTexture => "Stellamod/Effects/Masks/ZuiEffect";
        private float LifeTime = 90;
        private float MaxScale = 3f;

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 0.99f;
            Projectile.rotation += 0.05f;

            if(Timer % Main.rand.Next(15, 32) == 0 && Main.rand.NextBool(12))
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 16);
                int type = ModContent.ProjectileType<NiiviFrostFlowerProj>();
                int damage = Projectile.damage / 2;
                float knockback = Projectile.knockBack / 2;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    type, damage, knockback, Projectile.owner);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Chance to freeze the ground with an icey flower!

            //Return false to not kill itself
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.Frostburn, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.Chilled, 320);
                    break;
                case 2:
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Frozen, 60);
                    break;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.LightCyan.R, 
                Color.LightCyan.G, 
                Color.LightCyan.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(FrostTexture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            float progress = Timer / LifeTime;
            float easedProgress = Easing.OutCubic(progress);
            float scale = easedProgress * MaxScale;

            //This should make it fade in and then out
            float alpha = Easing.SpikeCirc(progress);
            alpha += 0.05f;
            Color drawColor = (Color)GetAlpha(lightColor);
            drawColor = drawColor * alpha;

            SpriteBatch spriteBatch = Main.spriteBatch;
            for(int i = 0; i < 4; i++)
            {
                float drawScale = scale * (i / 4f);
                float drawRotation = Projectile.rotation * (i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            }

            //I think that one texture will work
            //The vortex looking one
            //And make it spin
            return false;
        }
    }
}
