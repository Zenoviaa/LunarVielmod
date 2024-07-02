
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;


//By Al0n37
namespace Stellamod.NPCs.Bosses.DaedusRework
{

    public class SolarSingularity : ModNPC
    {

        public int PrevAtack;
        int moveSpeed = 0;
        int moveSpeedY = 0;
        float DaedusDrug = 8;
        float HomeY = 330f;
        private bool p2 = false;
        bool Attack;
        bool Flying;
        Vector2 DaedusPos;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            // DisplayName.SetDefault("Jack");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + (NPC.scale * 4), NPC.height - NPC.frame.Height + 0);
            Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, DaedusDrug).RotatedBy(radians) * time, NPC.frame, new Color(234, 132, 54, 50), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, DaedusDrug * 2).RotatedBy(radians) * time, NPC.frame, new Color(254, 204, 72, 77), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            return true;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
            NPC.alpha = 0;
            NPC.width = 100;
            NPC.height = 60;
            NPC.damage = 100000;
            NPC.defense = 0;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = 10f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.npcSlots = 10f;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Daedus");
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Graveyard,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A small singularity granted to Daedus in exchange for where he lives and protects for.")),
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2048f, 128f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }
        public Vector2 DaedusPosAdd;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(DaedusPosAdd);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DaedusPosAdd = reader.ReadVector2();
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (Flying)
            {
                if (NPC.Center.X >= DaedusPosAdd.X && moveSpeed >= -120) // flies to players x position
                    moveSpeed--;
                else if (NPC.Center.X <= DaedusPosAdd.X && moveSpeed <= 120)
                    moveSpeed++;

                NPC.velocity.X = moveSpeed * 0.10f;

                if (NPC.Center.Y >= DaedusPosAdd.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
                {
                    moveSpeedY--;
                    HomeY = 0f;
                }
                else if (NPC.Center.Y <= DaedusPosAdd.Y - HomeY && moveSpeedY <= 20)
                {
                    moveSpeedY++;
                }

                NPC.velocity.Y = moveSpeedY * 0.13f;
            }

            if (Attack)
            {
                NPC.velocity *= 0.82f;
                if (DaedusDrug <= 18)
                {
                    DaedusDrug += 0.1f;
                }
            }
            else
            {
                if (DaedusDrug >= 8)
                {
                    DaedusDrug -= 0.1f;
                }
            }
            if (!NPC.HasPlayerTarget)
            {
                NPC.TargetClosest(false);
                Player player1 = Main.player[NPC.target];
                if (!NPC.HasPlayerTarget || NPC.Distance(player1.Center) > 3000f)
                {
                    return;
                }
            }

            Player playerT = Main.player[NPC.target];
            int distance = (int)(NPC.Center - playerT.Center).Length();
            if (distance > 3000f || playerT.dead)
            {
                NPC.ai[0] = 0;
                NPC.ai[3]++;
                NPC.position.Y = player.Center.Y + -800;
                if (NPC.ai[3] >= 80)
                {
                    NPC.active = false;
                }
            }
            if (NPC.ai[2] == 0)
            {
   
                NPC.ai[2] = 1;
            }
            p2 = NPC.life < NPC.lifeMax * 0.5f;

           
            if (NPC.ai[2] == 1)
            {
                switch (NPC.ai[1])
                {
                    case 0:
 
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 20)
                        {
                            DaedusPos = NPC.position;
                            if (StellaMultiplayer.IsHost)
                            {
                                DaedusPosAdd.X = DaedusPos.X + Main.rand.Next(-50, 50);
                                DaedusPosAdd.Y = DaedusPos.Y + Main.rand.Next(-50, 50);
                                NPC.netUpdate = true;
                            }

                            Flying = true;
                            NPC.ai[0] = 290;
                            NPC.ai[1] = 1;
                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 100)
                        {      
                            NPC.ai[1] = 2;
                        }

                        break;
                    case 2:
                        NPC.ai[0]++;

                        if (NPC.ai[0] >= 300)
                        {
                            NPC.ai[0] = 0;
                            if (StellaMultiplayer.IsHost)
                            {
                                DaedusPosAdd.X = DaedusPos.X + Main.rand.Next(-90, 90);
                                DaedusPosAdd.Y = DaedusPos.Y + Main.rand.Next(-50, 50);
                                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                                    ModContent.NPCType<SolarPortal>());
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                }
            }
        }
    }
}