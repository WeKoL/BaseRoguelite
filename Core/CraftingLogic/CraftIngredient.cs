public sealed class CraftIngredient { public string ItemId {get;} public int Amount {get;} public CraftIngredient(string itemId,int amount){ItemId=itemId??""; Amount=System.Math.Max(1,amount);} }
