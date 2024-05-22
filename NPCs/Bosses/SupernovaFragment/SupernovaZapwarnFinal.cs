using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.SupernovaFragment
{
    internal class SupernovaZapwarnFinal : ModNPC
    {
        public float RotSpeed = 10.4f;
        public bool Down;
        public bool Lightning;


        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
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

        float alphaCounter = 6;
        float counter = 8;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(75f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), NPC.rotation, new Vector2(30 / 4, 1028 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(75f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), NPC.rotation, new Vector2(30 / 4, 1028 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(75f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), NPC.rotation, new Vector2(30 / 4, 1028 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            return true;

        }
        public override void AI()
        {
            NPC.position = SupernovaFragment.SingularityPos;
            NPC.ai[0]++;
            NPC.rotation = RotSpeed + NPC.ai[1];
            alphaCounter = MathHelper.Lerp(alphaCounter, 0, 0.05f);
            if (RotSpeed >= 0)
            {
                RotSpeed = MathHelper.Lerp(RotSpeed, 0, 0.06f);
            }
            NPC.ai[2] = NPC.rotation;

            if (NPC.ai[0] == 75)
            {

                Vector2 LightPos;
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SupernovaFragment_EndLazer1"), NPC.position);
                }
                if (Sound == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SupernovaFragment_EndLazer2"), NPC.position);
                }

                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 64f);

                LightPos.X = NPC.position.X;
                LightPos.Y = NPC.position.Y - 500;
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero, ModContent.ProjectileType<SupernovaFinalExplosion>(), 800, 0f, Owner: Main.myPlayer);
                    Vector2 velocity = new Vector2(0, 10);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y) - NPC.rotation.ToRotationVector2() * 1200, velocity,
                                         ModContent.ProjectileType<SupernovaBeamFinal>(), 500, 0f, Owner: Main.myPlayer, ai1: NPC.whoAmI);

                }
            }
            if (NPC.ai[0] == 200)
            {
                NPC.active = false;
            }
        }
    }
}

