
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class AimingReticle : ModProjectile
    {
        private float _lerp;
        private ref float Timer => ref Projectile.ai[0];
        private enum AIState
        {
            In,
            Idle,
            Out
        }
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.hide = true;
        }
        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.In:
                    AI_In();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Out:
                    AI_Out();
                    break;
            }
        }
        private void AI_In()
        {
            Timer++;
            _lerp = Timer / 30f;
            Projectile.rotation += MathHelper.Lerp(0.05f, 0.01f, _lerp);
            if(Timer >= 30f)
            {
                SwitchState(AIState.Idle);
            }
        }
        private void AI_Idle()
        {
            Timer++;
            _lerp = 1f;
            if(Timer >= 30f)
            {
                SwitchState(AIState.Out);
            }
        }
        private void AI_Out()
        {
            Timer++;
            _lerp = MathHelper.Lerp(1f, 0f, Timer / 30f);
            Projectile.rotation += MathHelper.Lerp(0.01f, 0.05f, _lerp);
            if (Timer >= 30f)
            {
                Projectile.Kill();
            }
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color glowColor = Color.Red;

            glowColor *= _lerp;
            glowColor.A = 0;
            Vector2 drawScale = Vector2.One + Vector2.Lerp(Vector2.One, Vector2.Zero, _lerp);
            spriteBatch.Draw(texture, drawPosition, null, glowColor, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            overPlayers.Add(index);
        }
    }
}
