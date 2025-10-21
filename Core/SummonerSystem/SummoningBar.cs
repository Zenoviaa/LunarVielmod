using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.GunSystem;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Core.SummonerSystem
{
    public class SummoningBar : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        private BellPlayer BellPlayer => Owner.GetModPlayer<BellPlayer>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle gunTossSound = AssetRegistry.Sounds.Gun.GunToss;
                gunTossSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(gunTossSound);
            }
            if (BellPlayer.isSummoning)
            {
                Projectile.timeLeft = 2;
            }
            Projectile.Center = Owner.Center + new Vector2(0, 64);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D reloadBar = ModContent.Request<Texture2D>(Texture).Value;


            Texture2D reloadHandle = ModContent.Request<Texture2D>(Texture + "_Handle").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = reloadBar.Size() / 2f;


            float width = reloadBar.Width;
            float offset = MathHelper.Lerp(-width / 2f, width / 2f, BellPlayer.summonRatio);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(effect: SpriteWhiteShader.Instance.Effect);
            spriteBatch.Draw(reloadBar, drawPos - Vector2.UnitX * 2, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadBar, drawPos + Vector2.UnitX * 2, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadBar, drawPos - Vector2.UnitY * 2, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadBar, drawPos + Vector2.UnitY * 2, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);



            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0) - Vector2.UnitX * 2, null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0) + Vector2.UnitX * 2, null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0) - Vector2.UnitY * 2, null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0) + Vector2.UnitY * 2, null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();

            spriteBatch.Draw(reloadBar, drawPos, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0), null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
