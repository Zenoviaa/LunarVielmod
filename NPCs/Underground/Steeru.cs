using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Stellamod.Items.Materials.Tech;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader.Utilities;
using Stellamod.Assets.Biomes;

namespace Stellamod.NPCs.Underground
{
    internal class Steeru : ModNPC
    {
        public const int Steeru_Gear_Count = 7;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 54;

            int height = 0;
            for(int i = 0; i < Steeru_Gear_Count; i++)
            {
                float f = (float)i;
                float scale = MathHelper.Lerp(1f, 1 / 2f, f / Steeru_Gear_Count);
                height += (int)(42 * scale * 0.5f);
            }

            NPC.height = height;
            NPC.damage = 60;
            NPC.defense = 12;
            NPC.lifeMax = 190;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 563f;
            NPC.knockBackResist = .1f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.3f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player

            float distX = targetCenter.X - NPC.Center.X;
            if (NPC.Center.X < targetCenter.X && NPC.velocity.X < moveSpeed)
            {
                NPC.velocity.X += accel;
            }
            else if(NPC.velocity.X > -moveSpeed)
            {
                NPC.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - NPC.Center.Y;
            if (NPC.Center.Y < targetCenter.Y && NPC.velocity.Y < moveSpeed)
            {
                NPC.velocity.Y += accel;
            }
            else if (NPC.Center.Y > targetCenter.Y && NPC.velocity.Y > -moveSpeed)
            {
                NPC.velocity.Y -= accel;
            }
        }


        public override void AI()
        {
            NPC.TargetClosest();

            Player target = Main.player[NPC.target];
            if (NPC.HasValidTarget)
            {
                AI_Movement(target.Center - new Vector2(0, NPC.height/2), 2, 0.02f);
            } else
            {

            }
           
            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(80))
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 size = new Vector2(54, 42);
            Vector2 drawOrigin = size / 2;
            Vector2 drawPos = NPC.position + drawOrigin;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.velocity.X < 0)
                spriteEffects = SpriteEffects.FlipHorizontally;

            float offsetY = size.Y;
            float hoverRange = offsetY/2;
            float hoverOffset = offsetY/Steeru_Gear_Count;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 trailDrawPos = NPC.oldPos[k] - Main.screenPosition + size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(191, 165, 160), new Color(191, 59, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                for(int i = Steeru_Gear_Count - 1; i > -1; i--)
                {
                    float yHovering = VectorHelper.Osc(0, hoverRange, speed: 3, offset: i * hoverOffset);
                    float xHovering = VectorHelper.Osc(-hoverRange/2, hoverRange/2, speed: 3, offset: i * hoverOffset);

                    float f = (float)i;
                    float scale = MathHelper.Lerp(1, 1f/2f, f / Steeru_Gear_Count);
                    Vector2 offset = new Vector2(xHovering, i * offsetY + yHovering);
                    offset.Y += offsetY;
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, trailDrawPos + offset*scale * 0.5f, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, scale, spriteEffects, 0f);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = Steeru_Gear_Count - 1; i > -1; i--)
            {
                float f = (float)i;
                float scale = MathHelper.Lerp(1, 1f / 2f, f / Steeru_Gear_Count);

                float yHovering = VectorHelper.Osc(0, hoverRange, speed: 3, offset: i * hoverOffset);
                float xHovering = VectorHelper.Osc(-hoverRange/2, hoverRange/2, speed: 3, offset: i * hoverOffset);


                Vector2 offset = new Vector2(xHovering, i * offsetY + yHovering);
                offset.Y += offsetY;
                spriteBatch.Draw(texture, drawPos - screenPos + offset*scale*0.5f, NPC.frame, Color.White, NPC.rotation, drawOrigin, scale, spriteEffects, 0);
            }

            //Draw Eye
            texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Underground/SteeruEye").Value;
            drawOrigin = texture.Size() / 2;
            float yHoveringEye = VectorHelper.Osc(0, hoverRange, speed: 3);
            float xHoveringEye = VectorHelper.Osc(-hoverRange/2, hoverRange/2, speed: 3);
            Vector2 hoveringOffset = new Vector2(xHoveringEye, yHoveringEye);
            
            spriteBatch.Draw(texture, NPC.position - screenPos + hoveringOffset + new Vector2(29, 0), null, 
                Color.White, NPC.rotation, drawOrigin, 1, spriteEffects, 0);
            
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnRates.GetMechanicalEnemySpawnChance(spawnInfo);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrokenTech>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.IronOre, 1, 1, 5));
        }
    }
}
