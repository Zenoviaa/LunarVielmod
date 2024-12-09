using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal class CrossbowLodgedArrow : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private Vector2 LodgeOffset;
        private ref float Timer => ref Projectile.ai[0];
        private int ArrowType
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private int NPCToTrack
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(LodgeOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            LodgeOffset = reader.ReadVector2();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            NPC target = Main.npc[NPCToTrack];
            if (!target.active)
            {
                Projectile.Kill();
                return;
            }

            if (LodgeOffset == Vector2.Zero)
            {
                LodgeOffset = Projectile.position - target.position;
            }

            Projectile.position = target.position + LodgeOffset;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private void DrawArrowSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Asset<Texture2D> arrowTexture = TextureAssets.Projectile[ArrowType];
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = arrowTexture.Size() / 2f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            float drawRotation = Projectile.rotation + MathHelper.ToRadians(90);

            Color drawColor = Color.White.MultiplyRGB(lightColor);
            if (Timer > 90)
            {
                float endProgress = (Timer - 90f) / 90f;
                endProgress = 1f - endProgress;
                drawColor *= endProgress;
            }

            Vector2 drawScale = Vector2.One;
            spriteBatch.Draw(arrowTexture.Value, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawArrowSprite(ref lightColor);
            return false;
        }
    }
}
