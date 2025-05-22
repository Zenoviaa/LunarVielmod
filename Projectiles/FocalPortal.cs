using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.ID;
using Stellamod.Projectiles.IgniterExplosions;

namespace Stellamod.Projectiles
{
    internal class FocalPortal : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        private int _teleportTimer;
        private bool _flash;
        private float _scale;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 60;
        }

        public override void SetDefaults()
        {
            Projectile.width = 75;
            Projectile.height = 95;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = int.MaxValue;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (!_flash)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge"), Projectile.position);
                _flash = true;
            }

            float range = 0.25f;
            float hover = VectorHelper.Osc(-range, range);
            Vector2 targetCenter = Projectile.Center + new Vector2(0, hover);
            Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, targetCenter, 5);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);

            if (NPC.AnyDanger())
            {
                return;
            }

            _teleportTimer++;
            _scale += 2/30f;
            if (_scale >= 2f)
                _scale = 2f;

            if (_teleportTimer < 30)
                return;

        

            Rectangle myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;
       
                Rectangle playerRect = player.getRect();
                if(Projectile.Colliding(myRect, playerRect))
                {
                    //Teleport
                    Teleport(player);
                }
            }
        }

        private void Teleport(Player player)
        {
            int x = (int)Projectile.ai[0];
            int y = (int)Projectile.ai[1];
            Vector2 direction = player.velocity.SafeNormalize(Vector2.Zero);
            Vector2 teleportPos = new Vector2(x, y) + direction * 128;
            player.Teleport(teleportPos);
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPos.X, teleportPos.Y, 1);

            SoundEngine.PlaySound(SoundID.Item115, Projectile.position);
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                ModContent.ProjectileType<KaBoomMagic2>(), 0, 1, player.whoAmI);
        }



        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 60)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 75;
            float height = 95;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 60;

            float scale = NPC.AnyDanger() ? 1f : _scale;
            Color color = NPC.AnyDanger() ? Color.Gray : Color.White;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                color, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
