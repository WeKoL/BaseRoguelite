#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class MenuFlowStep031
{
	public string Id { get; }
	public string Label { get; }
	public bool BlocksPlayerInput { get; }
	public MenuFlowStep031(string id, string label, bool blocksPlayerInput)
	{
		Id = id ?? string.Empty;
		Label = label ?? string.Empty;
		BlocksPlayerInput = blocksPlayerInput;
	}
}

public sealed class MenuFlowNavigator031
{
	private readonly Stack<MenuFlowStep031> _stack = new();
	public MenuFlowStep031? Current => _stack.Count == 0 ? null : _stack.Peek();
	public int Depth => _stack.Count;
	public void Push(MenuFlowStep031 step)
	{
		if (step != null) _stack.Push(step);
	}
	public MenuFlowStep031? Back()
	{
		return _stack.Count == 0 ? null : _stack.Pop();
	}
}

public sealed class TooltipPriority031
{
	public string Text { get; }
	public int Priority { get; }
	public TooltipPriority031(string text, int priority)
	{
		Text = text ?? string.Empty;
		Priority = priority;
	}
	public static string PickBest(IEnumerable<TooltipPriority031> tips)
	{
		return (tips ?? Array.Empty<TooltipPriority031>()).OrderByDescending(x => x.Priority).FirstOrDefault()?.Text ?? string.Empty;
	}
}
