using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content;
using Stellamod.Core.XixianFlaskSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources
{
    public class WindingInsourcePlayer : ModPlayer
    {
        public int stacks;
        public Asset<Texture2D> BubbleTextureAsset;
        public override void PostUpdateBuffs()
        {
            base.PostUpdateBuffs();
            if (stacks > 0)
            {
                Player.AddBuff(ModContent.BuffType<WindingInsourceBuff>(), 2);
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
            if (stacks > 0)
            {
                int npcIndex = info.DamageSource.SourceNPCIndex;
                if (npcIndex != -1)
                {
                    while (stacks > 0)
                    {
                        NPC target = Main.npc[npcIndex];
                        NPC.HitInfo hitInfo = target.CalculateHitInfo((int)(info.Damage * 5), 1, true, 0, DamageClass.Generic);
                        target.StrikeNPC(hitInfo);
                        NetMessage.SendStrikeNPC(target, hitInfo, Main.myPlayer);
                        stacks--;
                    }
                }
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);

        }


        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            if (stacks > 0 && drawInfo.shadow == 0f)
            {
                string path = this.GetType().DirectoryHere() + "/StrengthInsourceBubble";
                BubbleTextureAsset ??= ModContent.Request<Texture2D>(path);
                SpriteBatch spriteBatch = Main.spriteBatch;

                Vector2 drawPosition = Player.Center - Main.screenPosition;
                Point tilePosition = Player.position.ToTileCoordinates();
                Color lightingColor = Lighting.GetColor(tilePosition.X, tilePosition.Y);
                Color drawColor = Color.Tan.MultiplyRGB(lightingColor);
                Vector2 drawOrigin = BubbleTextureAsset.Size() / 2f;
                Vector2 drawScale = Vector2.Lerp(new Vector2(0.65f), new Vector2(0.75f), ExtraMath.Osc(0f, 1f, speed: 12));
                spriteBatch.Restart(blendState: BlendState.Additive);
                spriteBatch.Draw(BubbleTextureAsset.Value, drawPosition, null, drawColor, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
                spriteBatch.Draw(BubbleTextureAsset.Value, drawPosition, null, drawColor, 0, drawOrigin, drawScale * 0.6f, SpriteEffects.None, 0);
                spriteBatch.RestartDefaults();

            }
        }
    }

    public class WindingInsourceBuff : ModBuff
    {

    }

    public class WindingInsource : InsourceItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<ShopRarity>();
        }

        public override int GetAddedTime()
        {
            return 60 * 30;
        }

        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            WindingInsourcePlayer insourcePlayer = flaskPlayer.Player.GetModPlayer<WindingInsourcePlayer>();
            insourcePlayer.stacks++;

            SoundStyle useSound = new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram");
            useSound.PitchVariance = 0.1f;
            SoundEngine.PlaySound(useSound, flaskPlayer.Player.position);

            FXUtil.ShakeCamera(flaskPlayer.Player.Center, 1024, 8);
            FXUtil.GlowCircleBoom(flaskPlayer.Player.Center,
                innerColor: Color.Tan,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Black, duration: 25, baseSize: 0.28f);
        }
    }
}
