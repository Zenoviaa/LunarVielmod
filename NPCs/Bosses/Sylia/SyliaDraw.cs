using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    public partial class Sylia
    {
        //Extra Textures
        public const string BaseTexturePath = "Stellamod/NPCs/Bosses/Sylia/";
        public Texture2D MagicCircleTexture => ModContent.Request<Texture2D>($"{BaseTexturePath}SyliaMagicCircle").Value;
        public Vector2 MagicCircleSize => new Vector2(512, 512);

        public Texture2D VoidScissorTexture => ModContent.Request<Texture2D>($"{BaseTexturePath}Projectiles/VoidScissor").Value;
        public Vector2 VoidScissorSize => new Vector2(66, 54);

        //Animation Stuffs
        private int _frameCounter;
        private int _frameTick;
        private int _wingFrameCounter;
        private int _wingFrameTick;

        private int _segmentIndex;


        //Trailing
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public bool DrawMagicCircle { get; private set; }
        public bool DrawSylia { get; private set; }
        public float MagicCircleRot { get; private set; }
        public float MagicCircleScale { get; private set; }


        private void PreDrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
            float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            SpriteEffects effects = SpriteEffects.None;
            Player player = Main.player[NPC.target];
            if (player.Center.X < NPC.Center.X)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Color rotatedColor = new Color(60, 0, 118, 75);

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Vector2 rotatedPos = drawPos + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;
                spriteBatch.Draw(texture, rotatedPos,
                    texture.AnimationFrame(ref _frameCounter, ref _frameTick, 2, 30, false),
                rotatedColor, 0f, new Vector2(48, 48), 1f, effects, 0f);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Vector2 rotatedPos = drawPos + new Vector2(0f, 16f * rotationOffset).RotatedBy(radians) * time;
                spriteBatch.Draw(texture, rotatedPos,
                    texture.AnimationFrame(ref _frameCounter, ref _frameTick, 2, 30, false),
                    rotatedColor, 0f, new Vector2(48, 48), 1f, effects, 0f);
            }
        }


        private void PreDrawMagicCircle(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float drawDirection = 1f;
            if (!DrawMagicCircle)
            {
                drawDirection = -1f;
            }
               
            MagicCircleRot += 0.03f * drawDirection;
            MagicCircleScale += 0.01f * drawDirection;
            float maxMagicCircleScale = 0.25f;

            //Clamp Values so they don't go beyond
            MagicCircleScale = MathHelper.Clamp(MagicCircleScale, 0, maxMagicCircleScale);
            MagicCircleRot = MathHelper.Clamp(MagicCircleRot, 0, float.MaxValue);

            //Can't see it, no point to draw it.
            if (MagicCircleScale <= 0)
                return;

            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 origin = MagicCircleSize / 2;   
            Color circleColor = ColorFunctions.MiracleVoid;
            Color color = Color.Multiply(new(circleColor.R, circleColor.G, circleColor.B, 0), 1f);
            spriteBatch.Draw(MagicCircleTexture, drawPosition, null, color, MagicCircleRot, origin, MagicCircleScale, SpriteEffects.None, 0f);


            origin = VoidScissorSize / 2;
            float distance = 64;
            float scissorCount = 4;
       
            //Scissor Circle
            for(int i = 0; i < scissorCount; i++)
            {
                float osc = VectorHelper.Osc(0, 1, offset: i / scissorCount);
                float rotation = (MathHelper.TwoPi * (i / scissorCount)) + MagicCircleRot;
                Vector2 drawOffset = Vector2.UnitY.RotatedBy(rotation) * (distance * osc);
                Vector2 scissorDrawPosition = drawPosition + drawOffset;
                float rot = (scissorDrawPosition - drawPosition).ToRotation() + MathHelper.PiOver4;
                spriteBatch.Draw(VoidScissorTexture, scissorDrawPosition, null, drawColor, rot, origin, 4f * MagicCircleScale, SpriteEffects.None, 0f);
            }
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = NPC.scale * NPC.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(ColorFunctions.MiracleVoid, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!DrawSylia)
                return false;

            Player player = Main.player[NPC.target];
            SpriteEffects effects = SpriteEffects.None;
            if (player.Center.X < NPC.Center.X)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            //Draw Faint Glow
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            if (_attackPhase == ActionPhase.Phase_2)
            {
                TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
                GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.VortexTrail);
                TrailDrawer.DrawPrims(NPC.oldPos, NPC.Size * 0.5f - Main.screenPosition, 155);
            }

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, drawColor, 2);
            PreDrawAfterImage(spriteBatch, screenPos, drawColor);

            //Draw the Magic Circle
            PreDrawMagicCircle(spriteBatch, screenPos, drawColor);

            //Draw the Wings
            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 origin = new Vector2(48, 48);
            Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/SyliaWings").Value;
            int wingFrameSpeed = 2;
            int wingFrameCount = 10;
            spriteBatch.Draw(syliaWingsTexture, drawPosition,
                syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
                drawColor, 0f, origin, 1f, effects, 0f);

            Texture2D syliaIdleTexture = ModContent.Request<Texture2D>(Texture).Value;
            int syliaIdleSpeed = 2;
            int syliaIdleFrameCount = 30;
            spriteBatch.Draw(syliaIdleTexture, drawPosition,
                syliaIdleTexture.AnimationFrame(ref _frameCounter, ref _frameTick, syliaIdleSpeed, syliaIdleFrameCount, true),
                drawColor, 0f, origin, 1f, effects, 0f);
            return false;
        }
    }
}
