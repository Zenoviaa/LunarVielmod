using Stellamod.Core;
using Stellamod.NPCs.Bosses.Verlia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Core.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Trails;

namespace Stellamod.Content.Areas.Abyss.BossesAB.VerlianSingularity
{
    public class VerlianSingularity : ScarletBoss
    {
        private float _spinTimer;
        private enum AIState
        {
            Spawn,
            Idle
        }
        private ref float Timer => ref NPC.ai[0];
       private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 2;
            NPC.defense = 11;
            NPC.lifeMax = 4500;
            NPC.scale = 1f;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;

            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SingularityFragment");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
        }

        public override void AI()
        {
            base.AI();
            _spinTimer++;
            if(_spinTimer % 7 == 0)
            {
                float radius = 800;
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                Vector2 velocity = NPC.Center - spawnPos;
                velocity = velocity.SafeNormalize(Vector2.Zero);
                velocity *= Main.rand.NextFloat(16, 64);
                FXUtil.GlowStretch(spawnPos, velocity);
            }
            float startRadians = -MathHelper.ToRadians(22);
            float endRadians = startRadians + MathHelper.ToRadians(2);
            float interpolant = ExtraMath.Osc(0f, 1f, speed: 1);
            NPC.rotation = MathHelper.Lerp(startRadians, endRadians, interpolant);
            NPC.velocity = Vector2.UnitY.RotatedBy(_spinTimer * 0.02f) * 0.5f;
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
            }
        }
        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }
        private void AI_Spawn()
        {

        }
        private void AI_Idle()
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = NPC.scale * Vector2.One;


            float spinRotOffset = _spinTimer * -0.01f;
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Main.DiscoColor;
            sparkyShader.Distortion = -0.15f;
            sparkyShader.Time = -Main.GlobalTimeWrappedHourly * 40;
            sparkyShader.Tiling = Vector2.One * 2;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: sparkyShader.Effect);


            var lightTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 lightDrawOrigin = lightTexture.Size() / 2f;

            float sparkyRot = NPC.rotation + spinRotOffset;
            float scaleOsc2 = ExtraMath.Osc(0.4f, 0.5f, speed: 1);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.White * 0.75f, sparkyRot, lightDrawOrigin, drawScale * 3 * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.White * 0.25f, sparkyRot + 0.2f, lightDrawOrigin, drawScale * 8 * scaleOsc2, SpriteEffects.None, 0);


            var shader = SingularityShader.Instance;
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawPosition, null, Color.White, NPC.rotation, drawOrigin, drawScale * 1.5f * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();

            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 2));
            diskDrawColor.A = 0;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.65f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc, SpriteEffects.None, 0);


            for(float f = 0; f < 4; f++)
            {
              
                spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(1.5f, 0.2f) , SpriteEffects.None, 0);
            }
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(3.5f, 0.2f), SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(7.5f, 0.2f), SpriteEffects.None, 0);






            Texture2D extra67 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_67").Value;
            Vector2 extra67DrawOrigin = extra67.Size() / 2f;
            Color extra67DrawColor = Color.Lerp(Color.White, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 2));
            extra67DrawColor.A = 0;
            spriteBatch.Draw(extra67, drawPosition, null, extra67DrawColor, NPC.rotation, extra67DrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);



            return false;
        }
    }
}
