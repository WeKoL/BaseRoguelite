public sealed class FeatureReadinessItem
{
	public string Id { get; }
	public string Title { get; }
	public bool IsDone { get; }
	public int Weight { get; }
	public FeatureReadinessItem(string id, string title, bool isDone, int weight = 1) { Id = id ?? string.Empty; Title = string.IsNullOrWhiteSpace(title) ? Id : title; IsDone = isDone; Weight = weight <= 0 ? 1 : weight; }
}
