using System.Collections.Generic;

public sealed class AudioMixPlan030
{
	private readonly Dictionary<AudioCueKind, float> _volumes = new();
	public IReadOnlyDictionary<AudioCueKind, float> Volumes => _volumes;
	public void SetVolume(AudioCueKind cue, float volume) => _volumes[cue] = System.Math.Clamp(volume, 0f, 1f);
	public float GetVolume(AudioCueKind cue) => _volumes.TryGetValue(cue, out float v) ? v : 1f;
}
