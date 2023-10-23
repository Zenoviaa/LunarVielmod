using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.Utilities;
using Stellamod.Items.Consumables;
using Stellamod.Dusts;

namespace Stellamod.NPCs
{
    internal class DiceinNPC : ModNPC
    {

        float Frame = 0.15f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cinder Bat");

        }
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += Frame;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void SetDefaults()
        {
            Main.npcFrameCount[NPC.type] = 30;
            NPC.lavaImmune = true;
            NPC.width = 45;
            NPC.height = 45;
            NPC.damage = 0;
            NPC.defense = 8;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            NPC.value = 30f;
            NPC.alpha = 60;
            NPC.knockBackResist = .75f;
            NPC.alpha = 0;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }


        public override void AI()
        {


            Player player = Main.player[NPC.target];
            NPC.spriteDirection = -NPC.direction;
            NPC.ai[0]++;

            Frame += 0.005f;
            if (NPC.ai[0] == 1)
            {
                NPC.velocity.Y -= 7;
            }
            if (NPC.ai[0] >= 1)
            {
                NPC.velocity.Y *= 0.97f;
            }
            if (NPC.ai[0] >= 100)
            {
                SoundEngine.PlaySound(SoundID.NPCHit25, NPC.position);
                SoundEngine.PlaySound(SoundID.NPCDeath28, NPC.position);
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int k = 0; k < 50; k++)
                    {
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Sparkle>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(7), 0, default, 0.95f);
                        d.noGravity = true;
                    }
                }
                var entitySource = player.GetSource_OpenItem(Type);
                switch (Main.rand.Next(5))
                {

                    case 0:
                        CombatText.NewText(NPC.getRect(), Color.YellowGreen, "Wohooo", true, false);
                        Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(), ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 1));
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));
                        break;
                    case 1:
                        CombatText.NewText(NPC.getRect(), Color.YellowGreen, "Omg, its something!", true, false);
                        Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(), ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 2));
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));
                        break;
                    case 2:
                        CombatText.NewText(NPC.getRect(), Color.YellowGreen, "Are you disappointed? You should be.", true, false);
                        Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(), ModContent.ItemType<GildedBag1>(), Main.rand.Next(0, 1));
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));
                        break;
                    case 3:
                        CombatText.NewText(NPC.getRect(), Color.YellowGreen, "Wow, you have no maidens and no luck..", true, false);
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));
                        break;
                    case 4:
                        CombatText.NewText(NPC.getRect(), Color.YellowGreen, "Sooo lucky!", true, false);
                        Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(), ModContent.ItemType<GildedBag1>(), Main.rand.Next(2, 2));
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));
                        break;
                }
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 16f);
                NPC.active = false;

            }
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.LightBlue * num107 * .8f;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(
                GlowTexture,
                NPC.Center - Main.screenPosition + Drawoffset,
                NPC.frame,
                color1,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                effects,
                0
            );
            SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.MediumPurple);
            for (int num103 = 0; num103 < 4; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + ((float)num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * (float)num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }

    }
}