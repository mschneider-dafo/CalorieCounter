namespace CalorieCounter.ApiService;

public class FoodItem
{
    public int Id{get;set;}
    public required string Name{get;set;}
    public Guid InternalId{get;set;}
    public double CaloriesPer100g {get;set;}
    public double Protein{get;set;}
    public double Carbohydrates{get;set;}
    public double Fat{get;set;}
    public DateTimeOffset LastModified{get;set;}
}
