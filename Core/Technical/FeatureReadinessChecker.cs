using System.Collections.Generic;
public static class FeatureReadinessChecker
{
	public static FeatureReadinessReport BuildReport(IEnumerable<FeatureReadinessItem> items) { FeatureReadinessReport report = new(); if (items == null) return report; foreach (FeatureReadinessItem item in items) report.Add(item); return report; }
	public static FeatureReadinessItem Require(string id, string title, bool condition, int weight = 1) => new(id, title, condition, weight);
}
