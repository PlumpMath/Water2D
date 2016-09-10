half4 GetFalloff(half4 targetColor, float v, float height, float falloff)
{
	float t = (v - height) / falloff;
	t = clamp(t, 0.0, 1.0);
	return lerp(half4(0.0, 0.0, 0.0, 0.0), targetColor, t);
}