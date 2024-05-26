using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.NPCs.Bosses.SupernovaFragment;
using Stellamod.NPCs.Bosses.Zui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class MagicDoor : ModProjectile
    {
        private bool _hasSpawned;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 144;
        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }


        float trueFrame = 0;
        public void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
            if (trueFrame < minFrame)
            {
                trueFrame = minFrame;
            }
            if (trueFrame > maxFrame)
            {
                trueFrame = minFrame;
            }
        }

        public override void AI()
        {
            Timer++;
            float progress = Timer / 144f;
            float easedProgress = Easing.SpikeInOutCirc(progress);
            Projectile.scale = easedProgress;
            if(easedProgress >= 0.5f && !_hasSpawned && Main.myPlayer == Projectile.owner)
            {
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<SupernovaFragment>());
                }
                else
                {
                    StellaMultiplayer.SpawnBossFromClient((byte)Projectile.owner,
                        ModContent.NPCType<SupernovaFragment>(), (int)Projectile.Center.X, (int)Projectile.Center.Y);
                }
       
                _hasSpawned = true;
            }
            if(Timer >= 144)
            {
                Projectile.Kill();
            }
            //Lighting
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);

            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            UpdateFrame(1f, 1, 144);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Rectangle rectangle = new Rectangle(0, 0, 200, 300);
            rectangle.X = ((int)trueFrame % 12) * rectangle.Width;
            rectangle.Y = (((int)trueFrame - ((int)trueFrame % 12)) / 12) * rectangle.Height;

            Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            SpriteBatch spriteBatch = Main.spriteBatch;
            float drawRotation = Projectile.rotation;
            float drawScale = Projectile.scale * 2f;

            spriteBatch.Draw(texture, drawPosition,
               rectangle,
                (Color)GetAlpha(lightColor), drawRotation, origin, drawScale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
