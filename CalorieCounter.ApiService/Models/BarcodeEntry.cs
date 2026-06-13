namespace CalorieCounter.ApiService;

public class BarcodeEntry
{
    public int Id{get;set;}
    public required string Code{get;set;}
    public Guid FoodItemId{get;set;}
    public FoodItem? FoodItem{get;set;}
}
