using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Common
{
    public class SpawnerNPC : ModNPC
    {
        private int _frame;
        private int NPCType
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private ref float Timer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 10;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 16;
            NPC.height = 16;
            NPC.damage = 1;
            NPC.defense = 1;
            NPC.lifeMax = 1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.dontCountMe = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawShrinkingEffect(spriteBatch, screenPos, drawColor);
            DrawPreviewNPC(spriteBatch, screenPos, drawColor);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.25f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            if (_frame >= 10)
            {
                _frame = 10;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 100)
            {
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.position.X, (int)NPC.position.Y, NPCType);
                }
                for (int i = 0; i < 24; i++)
                {
                    float rot = i / 24f * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                }

                SoundStyle soundStyle;
                switch (Main.rand.Next(2))
                {
                    case 0:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GintzSummon");
                        soundStyle.PitchVariance = 0.1f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                        break;
                    case 1:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GintzSummon2");
                        soundStyle.PitchVariance = 0.1f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                        break;
                }

                NPC.Kill();
            }
        }
        private void DrawShrinkingEffect(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Restart(blendState: BlendState.Additive);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float rotation = NPC.rotation;
            float scale = NPC.scale;
            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }

        private void DrawPreviewNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPCType == -1)
                return;
            spriteBatch.Restart(blendState: BlendState.Additive);
            ModNPC modNpc = ModContent.GetModNPC(NPCType);
            Texture2D texture = ModContent.Request<Texture2D>(modNpc.Texture).Value;
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height / Main.npcFrameCount[NPCType]);
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawPos = NPC.Center - screenPos;
            float rotation = NPC.rotation;

            float progress = Timer / 100f;
            float easedProgress = Easing.InOutCubic(progress);
            float scale = NPC.scale * easedProgress;
            for (int i = 0; i < 8; i++)
            {
                spriteBatch.Draw(texture, drawPos, frame, drawColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();

        }
    }
}
