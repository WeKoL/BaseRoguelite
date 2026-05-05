using Xunit;

namespace BaseRoguelite.Tests.SafeZone;

public class SafeZoneUnloadEntryNotificationTextBuilderTests
{
	[Fact]
	public void Build_UsesResolvedDisplayName_AndFinalStorageAmount()
	{
		var entry = new SafeZoneUnloadEntry("wooden_plank", 15, 15, 21);

		string text = SafeZoneUnloadEntryNotificationTextBuilder.Build(entry, itemId => itemId switch
		{
			"wooden_plank" => "Доски",
			_ => itemId
		});

		Assert.Equal("+15 Доски (21)", text);
	}

	[Fact]
	public void Build_FallsBackToItemId_WhenResolverReturnsEmptyName()
	{
		var entry = new SafeZoneUnloadEntry("metal", 7, 7, 12);

		string text = SafeZoneUnloadEntryNotificationTextBuilder.Build(entry, _ => "");

		Assert.Equal("+7 metal (12)", text);
	}
}
