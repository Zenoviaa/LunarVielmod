using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.NPCs.Bosses.singularityFragment
{
    public class SoulBlast : ModProjectile
    {

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Soul Blast");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 35;
			Projectile.height = 35;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = 10;
			Projectile.timeLeft = 900;
			Projectile.tileCollide = false;
			Projectile.damage = 45;
			Projectile.aiStyle = -1;
            Projectile.scale = 1.2f;
        }

		public override bool PreAI()
        {
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;

            }
            return true;
        } 
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightBlue.ToVector3() * 1.75f * Main.essScale);

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (Main.rand.Next(1) == 0)
				target.AddBuff(Mod.Find<ModBuff>("AbyssalFlame").Type, 200);
		}
		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height,
					0, 0, 60, 206);
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
		public override void AI()
        {
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
		}

	}
}