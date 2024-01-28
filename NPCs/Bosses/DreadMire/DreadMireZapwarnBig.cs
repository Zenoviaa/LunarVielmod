using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.DreadMire
{
    internal class DreadMireZapwarnBig : ModNPC
    {
        public bool Down;
        public bool Lightning;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (GlowTexture is not null)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Vector2 halfSize = new Vector2(GlowTexture.Width / 2, GlowTexture.Height / Main.npcFrameCount[NPC.type] / 2);
                spriteBatch.Draw(
                    GlowTexture,
                    new Vector2(NPC.position.X - screenPos.X + NPC.width / 2 - GlowTexture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - GlowTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + Main.NPCAddHeight(NPC) + NPC.gfxOffY),
                    NPC.frame,
                    Color.White,
                    NPC.rotation,
                    halfSize,
                    NPC.scale,
                    spriteEffects,
                0);
            }
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
            NPC.alpha = 255;
            NPC.width = 0;
            NPC.height = 0;
            NPC.damage = 0;
            NPC.defense = 8;
            NPC.lifeMax = 156;
            NPC.value = 30f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.friendly = true;
            NPC.dontCountMe = true;
        }

        float alphaCounter = 0;
        float counter = 8;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(15f * alphaCounter), (int)(25f * alphaCounter), 0), 0, new Vector2(30 / 4, 1028 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(15f * alphaCounter), (int)(25f * alphaCounter), 0), 0, new Vector2(30 / 4, 1028 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(15f * alphaCounter), (int)(25f * alphaCounter), 0), 0, new Vector2(30 / 4, 1028 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            return true;

        }
        public override void AI()
        {   
            if (!Down)
            {
                alphaCounter += 0.04f;
                if (alphaCounter >= 4)
                {
                    Down = true;
                }
            }
            else
            {
                NPC.ai[0]++;
                if (NPC.ai[0] == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap"), NPC.position);
                    Vector2 LightPos;
                    int Sound = Main.rand.Next(1, 4);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain1"), NPC.position);
                    }
                    if (Sound == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain2"), NPC.position);
                    }
                    if (Sound == 3)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain3"), NPC.position);
                    }
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 32f);
                    LightPos.X = NPC.position.X;
                    LightPos.Y = NPC.position.Y - 500;
                    if (StellaMultiplayer.IsHost)
                    {
                        var entitySource = NPC.GetSource_FromThis();
                        Projectile.NewProjectile(entitySource, LightPos.X, LightPos.Y, 0, 0,
                            ModContent.ProjectileType<DreadlightingBig>(), 50, 1, Owner: Main.myPlayer);
                        Projectile.NewProjectile(entitySource, LightPos.X, LightPos.Y, 0, 0,
                            ModContent.ProjectileType<DreadSpawnEffect>(), 40, 1, Owner: Main.myPlayer);
                    }
                }
                if (NPC.ai[0] >= 10)
                {
                    if (alphaCounter <= 0)
                    {
                        NPC.active = false;
                    }
                    alphaCounter -= 0.29f;
                }
            }

            if (!Lightning)
            {
                Lightning = true;
            }
        }
    }
}

