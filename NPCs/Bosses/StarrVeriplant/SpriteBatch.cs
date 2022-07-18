
/*Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);

// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)






Vector2 frameOrigin = NPC.frame.Size();
Vector2 offset = new Vector2(NPC.width - frameOrigin.X, NPC.height - NPC.frame.Height);
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

for (float i = 0f; i < 1f; i += 0.25f)
{
	float radians = (i + timer) * MathHelper.TwoPi;

	spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(90, 70, 255, 50), NPC.rotation, frameOrigin, NPC.scale, SpriteEffects.None, 0);
}

for (float i = 0f; i < 1f; i += 0.34f)
{
	float radians = (i + timer) * MathHelper.TwoPi;

	spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, NPC.frame, new Color(140, 120, 255, 77), NPC.rotation, frameOrigin, NPC.scale, SpriteEffects.None, 0);
}

// Using a rectangle to crop a texture can be imagined like this:
// Every rectangle has an X, a Y, a Width, and a Height
// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
// Our Width and Height values specify how big of an area we want to sample starting from X and Y
SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
Rectangle rect;
*/