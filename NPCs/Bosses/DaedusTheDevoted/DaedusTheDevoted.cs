using CosmicVoid.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.DaedusRework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted
{
    internal class BaseDaedusSegment
    {
        public BaseDaedusSegment(NPC npc)
        {
            NPC = npc;
        }

        public float frameCounter;
        public int frame;
        public NPC NPC { get; init; }
        public string BaseTexturePath => "Stellamod/NPCs/Bosses/DaedusTheDevoted/";
        public virtual void AI() { }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { }
    }

    internal class DaedusTopSegment : BaseDaedusSegment
    {
        public DaedusTopSegment(NPC npc) : base(npc) { }
        public Rectangle AnimationFrame { get; set; }
        public override void AI()
        {
            base.AI();
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusTop").Value;
            frameCounter += 0.5f;
            if(frameCounter >= 1f)
            {
                frame++;
                if(frame >= 60)
                {
                    frame = 0;
                }
            }

            AnimationFrame = texture.GetFrame(frame, totalFrameCount: 60);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusTop").Value;
            Vector2 drawPos = NPC.Center - screenPos;

            Vector2 drawOrigin = AnimationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, AnimationFrame, drawColor, 0f, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    internal class DaedusFaceSegment : BaseDaedusSegment
    {
        public enum AnimationState
        {
            Laughing,
            Smile,
            Scared
        }

        public DaedusFaceSegment(NPC npc) : base(npc) { Animation = AnimationState.Smile; }
        public AnimationState Animation { get; set; }
        public Rectangle AnimationFrame { get; set; }
        public override void AI()
        {
            base.AI();
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusFace").Value;
            frameCounter += 0.5f;
            if (frameCounter >= 1f)
            {
                frame++;
            }


            switch (Animation)
            {
                default:
                case AnimationState.Laughing:
                    if (frame >= 3)
                    {
                        frame = 0;
                    }
                    break;
                case AnimationState.Smile:
                    frame = 3;
                    break;
                case AnimationState.Scared:
                    if(frame < 4 || frame > 6)
                    {
                        frame = 4;
                    }
                    break;
            }

            AnimationFrame = texture.GetFrame(frame, totalFrameCount: 6);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusFace").Value;
            Vector2 drawPos = NPC.Center - screenPos;


            Vector2 drawOrigin = AnimationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, AnimationFrame, drawColor, 0f, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    internal class DaedusArmSegment : BaseDaedusSegment
    {
        public enum AnimationState
        {
            Raise,
            Hold_Up,
            Lower,
            Hold_Down
        }

        public DaedusArmSegment(NPC npc) : base(npc) { Animation = AnimationState.Hold_Down; }
        public AnimationState Animation { get; set; }
        public Rectangle AnimationFrame { get; set; }
        public override void AI()
        {
            base.AI();
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusArms").Value;
            frameCounter += 0.125f;
            if (frameCounter >= 1f)
            {
                frame++;
            }

            switch (Animation)
            {
                default:
                case AnimationState.Raise:
                    if(frame > 9)
                    {
                        Animation = AnimationState.Hold_Up;
                    }
                    break;
                case AnimationState.Hold_Up:
                    frame = 10;
                    break;
                case AnimationState.Lower:
                    if(frame < 11)
                    {
                        frame = 11;
                    }
                    if(frame > 17)
                    {
                        frame = 0;
                        Animation = AnimationState.Hold_Down;
                    }
                    break;
                case AnimationState.Hold_Down:
                    frame = 0;
                    break;
            }

            AnimationFrame = texture.GetFrame(frame, totalFrameCount: 17);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusArms").Value;
            Vector2 drawPos = NPC.Center - screenPos;

            Vector2 drawOrigin = AnimationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, AnimationFrame, drawColor, 0f, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    internal class DaedusBackSegment : BaseDaedusSegment
    {
        public DaedusBackSegment(NPC npc) : base(npc) { }
        public Rectangle AnimationFrame { get; set; }
        public override void AI()
        {
            base.AI();
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusBack").Value;
            frameCounter += 0.5f;
            if (frameCounter >= 1f)
            {
                frame++;
                if (frame >= 60)
                {
                    frame = 0;
                }
            }

            AnimationFrame = texture.GetFrame(frame, totalFrameCount: 60);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusBack").Value;
            Vector2 drawPos = NPC.Center - screenPos;

            Vector2 drawOrigin = AnimationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, AnimationFrame, drawColor, 0f, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    internal class DaedusRobeSegment : BaseDaedusSegment
    {
        public DaedusRobeSegment(NPC npc) : base(npc) { }
        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusRobe").Value;
            MiscShaderData shaderData = GameShaders.Misc["LunarVeil:DaedusRobe"];
            shaderData.Shader.Parameters["windNoiseTexture"].SetValue(TextureRegistry.CloudNoise.Value);

            float speed = 1;
            shaderData.Shader.Parameters["uImageSize0"].SetValue(texture.Size());
            shaderData.Shader.Parameters["startPixel"].SetValue(60);
            shaderData.Shader.Parameters["endPixel"].SetValue(115);
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * speed);
            shaderData.Shader.Parameters["distortionStrength"].SetValue(0.075f);


            Vector2 vel = -NPC.velocity * 0.1f;
            vel.Y *= 0.25f;
            shaderData.Shader.Parameters["movementVelocity"].SetValue(vel);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, shaderData.Shader, Main.GameViewMatrix.TransformationMatrix);

         
            Vector2 drawPos = NPC.Center - screenPos;

            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, null, drawColor, 0f, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    internal class DaedusTheDevoted : ModNPC
    {
        public DaedusTopSegment TopSegment { get; set; }
        public DaedusFaceSegment FaceSegment { get; set; }
        public DaedusBackSegment BackSegment { get; set; }
        public DaedusArmSegment ArmSegment { get; set; }
        public DaedusRobeSegment RobeSegment { get; set; }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            Main.npcFrameCount[NPC.type] = 46;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Stellamod/NPCs/Bosses/DaedusRework/DaedusBestiary",
                PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();


            NPC.width = 256;
            NPC.height = 256;
            NPC.damage = 14;
            NPC.defense = 10;
            NPC.lifeMax = 2600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.takenDamageMultiplier = 0.9f;
            NPC.BossBar = ModContent.GetInstance<DaedusBossBar>();

            NPC.aiStyle = 0;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Daedus");
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();

            //Animations
            TopSegment ??= new DaedusTopSegment(NPC);
            TopSegment.AI();

            FaceSegment ??= new DaedusFaceSegment(NPC);
            FaceSegment.AI();

            BackSegment ??= new DaedusBackSegment(NPC);
            BackSegment.AI();

            ArmSegment ??= new DaedusArmSegment(NPC);
            ArmSegment.AI();

            RobeSegment ??= new DaedusRobeSegment(NPC);
            RobeSegment.AI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw the segments
            BackSegment.Draw(spriteBatch, screenPos, drawColor);
            ArmSegment.Draw(spriteBatch, screenPos, drawColor);
            TopSegment.Draw(spriteBatch, screenPos, drawColor);
            RobeSegment.Draw(spriteBatch, screenPos, drawColor);
            FaceSegment.Draw(spriteBatch, screenPos, drawColor);
            return false;
        }
    }
}
