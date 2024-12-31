using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
    internal class WaterLauncher : ModNPC
    {
        public Vector3 HuntrianColorXyz;
        public float HuntrianColorOffset;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.width = 58;
            NPC.height = 34;
            NPC.damage = 1;
            NPC.defense = 10;
            NPC.lifeMax = 20000;
            NPC.value = 1f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
        }

        private void LookAtTarget()
        {
            NPC npc = NPC;

            //Look at target
            Player player = Main.player[NPC.target];

            // First, calculate a Vector pointing towards what you want to look at
            Vector2 vectorFromNpcToPlayer = player.Center - npc.Center;

            // Second, use the ToRotation method to turn that Vector2 into a float representing a rotation in radians.
            float desiredRotation = vectorFromNpcToPlayer.ToRotation();

            // A second approach is to use that rotation to turn the npc while obeying a max rotational speed. Experiment until you get a good value.
            npc.rotation = npc.rotation.AngleTowards(desiredRotation, 0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            int npcFrames = Main.npcFrameCount[NPC.type];
            int frameHeight = texture.Height / npcFrames;

            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = NPC.GetAlpha(Color.Lerp(Color.LightBlue, Color.Transparent, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, NPC.oldRot[k], drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawHelper.DrawDimLight(NPC, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.Yellow, Color.White, 2);
            return true;
        }

        private ref float ai_Counter => ref NPC.ai[0];
        private ref float attack_Count => ref NPC.ai[1];
        private ref float follow => ref NPC.ai[2];

        private void FollowNPC()
        {
            int npcIndex = (int)follow;
            if (npcIndex == -1)
                return;

            NPC npcToFollow = Main.npc[npcIndex];
            if (npcToFollow == null || !npcToFollow.active)
                return;

            Vector2 followVel = VectorHelper.VelocitySlowdownTo(NPC.Center, npcToFollow.Center, 20);
            NPC.velocity = followVel;
        }

        public override void AI()
        {
            NPC npc = NPC;
            npc.TargetClosest();
            if (!npc.HasValidTarget)
            {
                //WheelMovement(2);
                npc.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, 8), 0.025f);
                npc.EncourageDespawn(120);
                return;
            }

            HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(85, 45, 115),
                new Vector3(15, 60, 160),
                new Vector3(3, 3, 3), HuntrianColorOffset);

            Player player = Main.player[NPC.target];
            LookAtTarget();
            FollowNPC();

            ai_Counter++;
            if(ai_Counter > 72)
            {      
                Vector2 velocity = NPC.Center.DirectionTo(player.Center) * 10;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot2"));
                int count = 64;
                for (int k = 0; k < count; k++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(npc.Center, 0, 0, DustID.Water, newVelocity.X, newVelocity.Y );
                }

                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity,
                    ModContent.ProjectileType<WaterBomb>(), 60, 0f, Owner: Main.myPlayer);
                }

                ai_Counter = 0;
                attack_Count++;
            }

            if(attack_Count > 5)
            {
                NPC.Kill();
            }
        }
    }
}
