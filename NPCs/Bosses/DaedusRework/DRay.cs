using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.DaedusRework
{
    internal class DRay : ModNPC
    {
        public bool Down;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Stalker Lighting");
        }

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
            NPC.alpha = 255;
            NPC.width = 0;
            NPC.height = 0;
            NPC.damage = 8;
            NPC.defense = 8;
            NPC.lifeMax = 156;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        private float alphaCounter = 0;
        private float counter = 6;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/NPCs/Bosses/DaedusRework/DRay").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(45f * alphaCounter), (int)(05f * alphaCounter), 0), -NPC.rotation, new Vector2(30, 122), 0.25f * (counter + 0.3f), SpriteEffects.None, 0f);
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GothivianFlames>(), 4 * 40);
        }
        public override void AI()
        {
            NPC.damage = 0;
            NPC.ai[0]++;
            if (NPC.ai[0] == 100)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                if (StellaMultiplayer.IsHost)
                {
                    float speedXa = -NPC.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                    float speedYa = -NPC.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + -10, NPC.position.Y + -30, speedXa * 0, speedYa * 0, ModContent.ProjectileType<DaedusBombExplosion>(), (int)(27 * 1.5f), 0f, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<DLantern>(), (int)(27 * 1.5f), 0f, Main.myPlayer);
                }
            
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.position);
            }

            if (!Down)
            {
                alphaCounter += 0.05f;
                if (alphaCounter >= 5)
                {
                    Down = true;
                }
            }
            else
            {
                if (alphaCounter <= 0)
                {
                    NPC.active = false;
                }
                alphaCounter -= 0.29f;
            }
        }
    }
}

