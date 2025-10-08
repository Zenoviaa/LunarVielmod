using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.XixianFlaskSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources
{
    public class StrengthInsourcePlayer : ModPlayer
    {
        public int stacks;
        public Asset<Texture2D> BubbleTextureAsset;

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (stacks > 0)
            {
                Player.GetDamage(DamageClass.Generic) += 0.25f;
            }
        }
        public override void PostUpdateBuffs()
        {
            base.PostUpdateBuffs();
            if (stacks > 0)
            {
                Player.AddBuff(ModContent.BuffType<StrengthInsourceBuff>(), 2);
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
            
            stacks=0;
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
                Color drawColor = Color.Red.MultiplyRGB(lightingColor);
                Vector2 drawOrigin = BubbleTextureAsset.Size() / 2f;
                Vector2 drawScale = Vector2.Lerp(new Vector2(0.65f), new Vector2(0.75f), ExtraMath.Osc(0f, 1f, speed: 12));
                spriteBatch.Restart(blendState: BlendState.Additive);
                spriteBatch.Draw(BubbleTextureAsset.Value, drawPosition, null, drawColor, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
                spriteBatch.Draw(BubbleTextureAsset.Value, drawPosition, null, drawColor, 0, drawOrigin, drawScale * 0.6f, SpriteEffects.None, 0);
                spriteBatch.RestartDefaults();
             
            }
        }
    }
    public class StrengthInsourceBuff : ModBuff
    {

    }

    public class StrengthInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 30;
        }

        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            StrengthInsourcePlayer strengthPlayer = flaskPlayer.Player.GetModPlayer<StrengthInsourcePlayer>();
            strengthPlayer.stacks++;

            SoundStyle useSound = new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram");
            useSound.PitchVariance = 0.1f;
            SoundEngine.PlaySound(useSound, flaskPlayer.Player.position);



            FXUtil.ShakeCamera(flaskPlayer.Player.Center, 1024, 8);
            FXUtil.GlowCircleBoom(flaskPlayer.Player.Center,
                innerColor: Color.Red,
                glowColor: Color.DarkRed,
                outerGlowColor: Color.Black, duration: 25, baseSize: 0.28f);
        }
    }
}
