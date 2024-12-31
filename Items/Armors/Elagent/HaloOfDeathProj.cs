using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Elagent
{
    internal class HaloOfDeathProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        //Gonna do some fancy drawing, so we don't need a texture
        public override string Texture => TextureRegistry.EmptyTexture;

        private enum ActionState
        {
            In,
            Out
        }

        private NPC TargetNPC
        {
            get => Main.npc[(int)Projectile.ai[0]];
            set
            {
                if (value == null)
                {
                    Projectile.ai[0] = -1;
                }
                else
                {
                    Projectile.ai[0] = value.whoAmI;
                }
            }
        }

        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private ActionState State
        {
            get => (ActionState)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }

        private Vector2[] HaloPos;
        private float Duration = 60;
        internal PrimitiveTrail BeamDrawer;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            HaloPos = new Vector2[16];
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.In:
                    if (!TargetNPC.active)
                    {
                        State = ActionState.Out;
                    }
                    else
                    {
                        AI_In();
                    }
                    break;
                case ActionState.Out:
                    AI_Out();
                    break;
            }
            Visuals();
        }

        private void AI_In()
        {
            //Set the position of this thing
            Projectile.Center = TargetNPC.Center;
            Timer++;
            if (Timer >= Duration)
            {
                //Put your Explosion/Sounds/VFX here
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 16f);
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(TargetNPC.Center, ModContent.DustType<GlowDust>(),
                        (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(TargetNPC.Center, ModContent.DustType<TSmokeDust>(),
                        (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                }

                //An angelic boom would be cool I think
                TargetNPC.SimpleStrikeNPC(Projectile.damage, 1);
                TargetNPC = null;
                State = ActionState.Out;
            }
        }

        private void AI_Out()
        {
            Projectile.velocity += new Vector2(0, -0.1f);
            Projectile.alpha+=4;
            Timer++;
            if(Timer >= Duration * 2)
            {
                Projectile.Kill();
            }
        }

        private void MakeOval(Vector2 center, Vector2 radius, float angle, float rotation)
        {
            //Calculate Points On Oval
            DrawHelper.DrawChainOval(center, radius.X, radius.Y, angle, rotation, ref HaloPos);
        }

        private void Visuals()
        {
            float progress = Timer / Duration;
            float easedProgress = Easing.InOutCirc(progress);

            //Size of the oval
            Vector2 radiusStart = new Vector2(128, 48);
            Vector2 radiusEnd = new Vector2(20.8f, 8);
            Vector2 radius = Vector2.Lerp(radiusStart, radiusEnd, easedProgress);

            //This is how far the oval draws the line
            float angleStart = 0;
            float angleEnd = MathHelper.TwoPi * 2;
            float angle = MathHelper.Lerp(angleStart, angleEnd, easedProgress);

            Vector2 centerStart = Projectile.Center + new Vector2(0, -64);
            Vector2 centerEnd = Projectile.Center;
            Vector2 center = Vector2.Lerp(centerStart, centerEnd, easedProgress);

            float rotation = 0;
            MakeOval(center, radius, angle, rotation);
        }


        public float WidthFunction(float completionRatio)
        {
            float width = 3;
            return Projectile.scale * width;
        }

        public Color ColorFunction(float completionRatio)
        {
            float a = (float)Projectile.alpha / 255f;
            a = 1 - a;
            return Color.LightGoldenrodYellow * a;
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true);
            BeamDrawer.DrawPixelated(HaloPos, -Main.screenPosition, HaloPos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
