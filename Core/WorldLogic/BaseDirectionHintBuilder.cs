public static class BaseDirectionHintBuilder
{
	public static BaseDirectionHint Build(float playerX, float playerY, float baseX, float baseY)
	{
		float dx = baseX - playerX; float dy = baseY - playerY; float distance = System.MathF.Sqrt(dx * dx + dy * dy); string h = dx < -1f ? "запад" : dx > 1f ? "восток" : string.Empty; string v = dy < -1f ? "север" : dy > 1f ? "юг" : string.Empty; string text = string.IsNullOrWhiteSpace(h) ? v : string.IsNullOrWhiteSpace(v) ? h : v + "-" + h; if (string.IsNullOrWhiteSpace(text)) text = "рядом"; return new BaseDirectionHint(dx, dy, distance, text);
	}
}
