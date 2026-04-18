using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class MagicWitchCauldron : ModNPC
{
    private int _frame;
    private ref float Timer => ref NPC.ai[0];
    private ref float CraftTimer => ref NPC.ai[1];
    private float DrinkEaseTime => 45;
    private float CraftEaseTime => 20;
    private Cauldron Cauldron => ModContent.GetInstance<Cauldron>();
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 60;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 80;
        NPC.height = 48;
        NPC.friendly = true;
        NPC.lifeMax = 20;
        NPC.defense = 1;
        NPC.damage = 1;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.townNPC = true;
    }
    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.5f;
        if (NPC.frameCounter >= 1f)
        {
            NPC.frameCounter = 0;
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }

        NPC.frame = TextureAssets.Npc[Type].Value.GetFrame(_frame, 10, 6);
    }
    private void Spew()
    {
        for (float f = 0; f < 6; f++)
        {
            Vector2 upwardVelocity = -Vector2.UnitY * 5;
            upwardVelocity *= Main.rand.NextFloat(0.5f, 1.5f);
            upwardVelocity = upwardVelocity.RotatedByRandom(MathHelper.ToRadians(65));

            Vector2 pos = NPC.position;
            pos.X += Main.rand.Next(0, NPC.width);

            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            spawnParams.outerColor = Color.Red;
            spawnParams.scaleRange *= 0.5f;
            DustParticle.Spawn(pos, upwardVelocity, spawnParams);
        }
    }
    public override void AI()
    {
        base.AI();
        if(Main.rand.NextBool(24))
        {
            Vector2 pos = NPC.position;
            pos.X += Main.rand.Next(0, NPC.width);
            BubbleParticle bp = BubbleParticle.Spawn(pos, -Vector2.UnitY, Color.White, Main.rand.NextFloat(0.25f, 0.5f));
            bp.gravity = 0;
        }

        Rectangle npcRect = NPC.getRect();
        foreach (Item item in Main.ActiveItems)
        {
            Rectangle itemRect = item.getRect();
            if (!npcRect.Intersects(itemRect))
                continue;

            if (!Cauldron.IsMaterial(item.type) && !Cauldron.IsMold(item.type))
                continue;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/CauldronCraft");
            soundStyle.PitchVariance = 0.15f;
            SoundEngine.PlaySound(soundStyle, NPC.position);

            Spew();
            Timer = DrinkEaseTime;
            Cauldron.AddToBrew(item.type, item.stack);
            item.active = false;
        }

        if(Cauldron.Results.Count > 0 && CraftTimer <= 0)
        {
            Timer = 0;
            CraftTimer = CraftEaseTime;
            int result = Cauldron.Results.Dequeue();
            if (MultiplayerHelper.IsHost)
            {
                int itemIndex = Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(),
                    result, Main.rand.Next(1, 1));
                Main.item[itemIndex].shimmered = true;
                Main.item[itemIndex].velocity = -Vector2.UnitY * 15;
                Main.item[itemIndex].velocity = Main.item[itemIndex].velocity.RotatedByRandom(MathHelper.ToRadians(65));
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
            }
            Spew();
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/CauldronCraft");
            soundStyle.PitchVariance = 0.15f;
            SoundEngine.PlaySound(soundStyle, NPC.position);
        }

        if (CraftTimer > 0)
        {
            CraftTimer--;
        }

        if (Timer > 0)
        {
            Timer--;
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromNPC(NPC);
        sbDrawer.worldPosition.Y += ExtraMath.Osc(0f, 4f, speed: 1);

        float range = MathHelper.ToRadians(2);
        float radians = MathHelper.Lerp(-range, range, ExtraMath.Osc(0f, 1f, speed: 2));
        sbDrawer.rotation = radians;
        sbDrawer.scale = Vector2.One * 2;

        float ease = EasingFunction.QuadraticBump(Timer / DrinkEaseTime);
        float ease2 = EasingFunction.QuadraticBump(CraftTimer / CraftEaseTime);
        Vector2 drinkScale = Vector2.Lerp(Vector2.One, new Vector2(0.75f, 1.25f), MathHelper.Clamp(ease + ease2, 0f, 1f));
        sbDrawer.scale *= drinkScale;

        sbDrawer.BottomCenterOrigin();
        sbDrawer.drawOrigin.Y -= 32;
        spriteBatch.Draw(sbDrawer);

        //Flash
        if(Timer > 0 || CraftTimer > 0)
        {
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
            spriteBatch.Restart(effect: whiteShader.Effect);
            sbDrawer.color = Color.Lerp(Color.Transparent, Color.LightGreen * 0.75f, MathHelper.Clamp(ease + ease2, 0f, 1f));
            spriteBatch.Draw(sbDrawer);
            spriteBatch.RestartDefaults();

        }

        //Godrays here

        float scale = 1f;
        Vector2 offset = new Vector2();
        offset.Y -= 96;
        int index = 0;
        foreach(var sbm in Cauldron.InsideCauldron)
        {
            Vector2 pos = NPC.Center;
            pos.Y += ExtraMath.Osc(0f, 4f, speed: 2, offset: index);
            ModItem modItem = ModContent.GetModItem(sbm.item);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, $"x{sbm.stack} {modItem.DisplayName.Value}",
                pos - screenPos + offset, Color.White, 0f, Vector2.Zero, new Vector2(scale), -1, scale);
            offset.Y -= 24;
        }
        return false;
    }
    public override void OnKill()
    {
        base.OnKill();
    }
}
