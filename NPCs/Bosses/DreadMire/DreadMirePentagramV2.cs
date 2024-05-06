using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DreadMire
{
    internal class DreadMirePentagramV2 : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Morrowed Swampster");
            Main.npcFrameCount[NPC.type] = 11;
        }
        // Current state
        public int frameTick;
        // Current state's timer
        public float timer;

        public override void SetDefaults()
        {
            NPC.width = 600;
            NPC.height = 600;
            NPC.damage = 210;
            NPC.defense = 30;
            NPC.lifeMax = 500000;
            NPC.value = 0f;
            NPC.timeLeft = 450;
            NPC.knockBackResist = .0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;

        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Since the NPC sprite naturally faces left, we want to flip it when its X velocity is positive

            SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(texture, new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) + new Vector2(0, NPC.frame.Height / 2), NPC.frame, new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 0.66f, effects, 0);


            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)


            // Using a rectangle to crop a texture can be imagined like this:
            // Every rectangle has an X, a Y, a Width, and a Height
            // Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
            // Our Width and Height values specify how big of an area we want to sample starting from X and Y
            return false;
        }

        float trueFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 600;
            NPC.frame.X = ((int)trueFrame % 5) * NPC.frame.Width;
            NPC.frame.Y = (((int)trueFrame - ((int)trueFrame % 5)) / 5) * NPC.frame.Height;
        }

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
            UpdateFrame(0.8f, 1, 55);
            NPC.alpha -= 10;
            Timer++;
            if(Timer >= 43)
            {
                NPC.Kill();
            }
        }
    }
}
