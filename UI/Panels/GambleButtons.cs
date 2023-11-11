using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Items.Consumables;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Panels
{
    public class GambleButtons : UIElement
	{
		public static int selectedID = 0;
		static int idCount = 1;
		readonly int id = idCount++;

		const double dist = 60.0;
		public float scale = 1f;
		const float max_scale = 2f;

		public static UIText text = new UIText("")
		{
			HAlign = 0.5f
		};

		readonly Texture2D buttonTexture;

		readonly string buttonName;
		readonly int itemType;

		Color color;
		readonly double rotOff;
		double rot;

		public GambleButtons(string file, string name, Color clr, int type, double rotationOffset)
		{
			text = new UIText(name);
			buttonTexture = (Texture2D)ModContent.Request<Texture2D>(file, AssetRequestMode.ImmediateLoad);
			buttonName = name;
			color = clr;
			itemType = type;
			rotOff = rotationOffset;

		}
		public override void LeftClick(UIMouseEvent evt)
		{
			selectedID = id;
			if (selectedID == id)
			{
				text.TextColor = color;
				text.SetText(buttonName, Main.GameZoomTarget * 0.5f, true);
				Player player = Main.LocalPlayer;
				MyPlayer GamblePlayer = player.GetModPlayer<MyPlayer>();

				if (!GamblePlayer.Bossdeath)
				{
					for (int i = 0; i < player.inventory.Length; i++)
					{
						if (player.inventory[i].type == ModContent.ItemType<GambitToken>())
						{
							Item item = new Item();
							item.SetDefaults(itemType);
							player.inventory[i].TurnToAir();
							player.inventory[i] = item;
							SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


							Gamble.visible = false;
							break;


						}
					}

				}

			}

		}
		public override void Update(GameTime gameTime)
		{
			text.Left.Set((Main.LocalPlayer.Center.X - Main.screenPosition.X) - Main.screenWidth / 2, 0);
			text.Top.Set((Main.LocalPlayer.Center.Y - Main.screenPosition.Y - 112), 0);
			text.SetText(text.Text, Main.GameZoomTarget * 0.5f, true);

			if (selectedID == id)
			{
				if (scale < max_scale)
				{
					scale += 0.04f;
					if (scale > max_scale) scale = max_scale;
				}
			}
			else if (scale > 1f)
			{
				scale -= 0.04f;
				if (scale < 1f) scale = 1f;
			}
			rot = gameTime.TotalGameTime.TotalMilliseconds / 2000 + rotOff;

		}
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			
		
		double rot = (Main.GlobalTimeWrappedHourly / 1) + rotOff;

			Vector2 pos = Main.LocalPlayer.Center - Main.screenPosition;
			Vector2 mod = new((float)(pos.X + (Math.Cos(rot) * dist)) / Main.UIScale, (float)(pos.Y + (Math.Sin(rot) * dist)) / Main.UIScale);

			
			Left.Set(0, 0.5f);
			Top.Set(0, 0.5f);
			Width.Set(buttonTexture.Width, 0f);
			Height.Set(buttonTexture.Height, 0f);

			spriteBatch.Draw(buttonTexture, mod, buttonTexture.Bounds, new Color(255, 255, 255, 127), 0f, buttonTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			// new Rectangle((int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels)
		}
	
	}
	class Gamble : UIState
	{
		public static bool visible = false;

		public static GambleButtons Choice1;
		public static GambleButtons Choice2;
		public static GambleButtons Choice3;
		public static GambleButtons Choice4;
		public static GambleButtons Choice5;
		public static GambleButtons Choice6;

		public override void OnInitialize()
		{


			

			Choice1 = new GambleButtons("Stellamod/UI/Panels/Die1", "1", new Color(255, 111, 58, 255), ModContent.ItemType<GildedBag1>(), 0);
			Choice2 = new GambleButtons("Stellamod/UI/Panels/Die2", "2", new Color(255, 111, 58, 255), ModContent.ItemType<GildedBag1>(), MathHelper.ToRadians(60 * 1));
			Choice3 = new GambleButtons("Stellamod/UI/Panels/Die3", "3", new Color(44, 201, 122, 255), ModContent.ItemType<GildedBag1>(), MathHelper.ToRadians(60 * 2));
			Choice4 = new GambleButtons("Stellamod/UI/Panels/Die4", "4", new Color(209, 82, 234, 255), ModContent.ItemType<GildedBag1>(), MathHelper.ToRadians(60 * 3));
			Choice5 = new GambleButtons("Stellamod/UI/Panels/Die5", "5", new Color(37, 185, 234, 255), ModContent.ItemType<GildedBag1>(), MathHelper.ToRadians(60 * 4));
			Choice6 = new GambleButtons("Stellamod/UI/Panels/Die6", "6", new Color(255, 61, 116, 255), ModContent.ItemType<GildedBag1>(), MathHelper.ToRadians(60 * 5));

			
			
			Choice1.OnLeftClick += OnClick1;

			Append(Choice1);
			Append(Choice2);
			Append(Choice3);
			Append(Choice4);
			Append(Choice5);
			Append(Choice6);
			Append(GambleButtons.text);
		}

		public void OnClick1(UIMouseEvent evt, UIElement listeningElement)
		{
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/clickk"));
				
			
		}
	}
}