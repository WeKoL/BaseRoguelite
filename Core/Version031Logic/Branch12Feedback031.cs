using System;
using System.Collections.Generic;
using System.Linq;

public sealed class JuiceProfile031
{
	public float CameraShake { get; }
	public float HitPause { get; }
	public float SoundVolume { get; }
	public bool HasStrongFeedback => CameraShake >= 0.5f || HitPause >= 0.08f || SoundVolume >= 0.8f;

	public JuiceProfile031(float cameraShake, float hitPause, float soundVolume)
	{
		CameraShake = Math.Clamp(cameraShake, 0f, 1f);
		HitPause = Math.Clamp(hitPause, 0f, 0.25f);
		SoundVolume = Math.Clamp(soundVolume, 0f, 1f);
	}
}

public static class FeedbackIntensityPlanner031
{
	public static JuiceProfile031 ForDamage(int damage, bool critical)
	{
		float baseValue = Math.Clamp(damage / 50f, 0.1f, 1f);
		return new JuiceProfile031(critical ? Math.Max(0.7f, baseValue) : baseValue, critical ? 0.12f : 0.04f, critical ? 1f : 0.65f);
	}
}

public sealed class FeedbackEventBatch031
{
	private readonly List<string> _events = new();
	public IReadOnlyList<string> Events => _events;
	public void Add(string eventId)
	{
		if (!string.IsNullOrWhiteSpace(eventId)) _events.Add(eventId);
	}
	public IReadOnlyList<string> DrainDistinct()
	{
		var result = _events.Distinct().ToList();
		_events.Clear();
		return result;
	}
}
