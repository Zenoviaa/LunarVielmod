using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Beachthrow : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Beachthrow");
		}
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.aiStyle = 3;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 10;
			Projectile.timeLeft = 600;
			Projectile.scale = 0.65f;
			AIType = ProjectileID.PossessedHatchet;
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Asset<Texture2D> asset = TextureAssets.Projectile[Projectile.type];
			Texture2D tex = asset.Value;
			//Redraw the Projectile with the color not influenced by light
			int height = Main.player[Projectile.owner].height / 9; // 5 is frame count
			int y = height * Projectile.frame;
			var rect = new Rectangle(0, y, Main.player[Projectile.owner].width, height);
			var drawOrigin = new Vector2(Main.player[Projectile.owner].width / 2, Projectile.height / 2);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, rect, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			return true;
		}
	}
}
