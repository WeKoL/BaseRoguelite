using Xunit;

namespace BaseRoguelite.Tests.Base;

public class StorageStateTests
{
	[Fact]
	public void NewStorage_HasNoEntries()
	{
		var storage = new StorageState();

		Assert.Empty(storage.Entries);
		Assert.Equal(0, storage.GetTotalAmount("rock"));
	}

	[Fact]
	public void AddItem_AddsNewEntry()
	{
		var storage = new StorageState();

		storage.AddItem("rock", 5, 99);

		Assert.Single(storage.Entries);
		Assert.Equal(5, storage.Entries[0].Amount);
		Assert.Equal("rock", storage.Entries[0].ItemId);
		Assert.Equal(5, storage.GetTotalAmount("rock"));
	}

	[Fact]
	public void AddItem_StacksIntoExistingEntry()
	{
		var storage = new StorageState();

		storage.AddItem("rock", 4, 99);
		storage.AddItem("rock", 7, 99);

		Assert.Single(storage.Entries);
		Assert.Equal(11, storage.Entries[0].Amount);
		Assert.Equal(11, storage.GetTotalAmount("rock"));
	}

	[Fact]
	public void AddItem_SplitsIntoSeveralStacks_WhenMaxStackExceeded()
	{
		var storage = new StorageState();

		storage.AddItem("rock", 23, 10);

		Assert.Equal(3, storage.Entries.Count);
		Assert.Equal(10, storage.Entries[0].Amount);
		Assert.Equal(10, storage.Entries[1].Amount);
		Assert.Equal(3, storage.Entries[2].Amount);
		Assert.Equal(23, storage.GetTotalAmount("rock"));
	}

	[Fact]
	public void AddItem_KeepsDifferentItemsSeparate()
	{
		var storage = new StorageState();

		storage.AddItem("rock", 6, 99);
		storage.AddItem("metal", 3, 99);

		Assert.Equal(2, storage.Entries.Count);
		Assert.Equal(6, storage.GetTotalAmount("rock"));
		Assert.Equal(3, storage.GetTotalAmount("metal"));
	}

	[Fact]
	public void Clear_RemovesAllEntries()
	{
		var storage = new StorageState();

		storage.AddItem("rock", 8, 99);
		storage.Clear();

		Assert.Empty(storage.Entries);
		Assert.Equal(0, storage.GetTotalAmount("rock"));
	}

	[Fact]
	public void AddItem_IgnoresInvalidInput()
	{
		var storage = new StorageState();

		storage.AddItem("", 5, 99);
		storage.AddItem("rock", 0, 99);
		storage.AddItem("rock", -2, 99);
		storage.AddItem("rock", 5, 0);

		Assert.Empty(storage.Entries);
	}
}
