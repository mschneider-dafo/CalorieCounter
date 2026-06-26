namespace CalorieCounter.ApiService;

public class FoodEntry
{
    public int Id{get;set;}
    public Guid PublicIdentifier{get;set;}
    public int UserId{get;set;}
    public User? User{get;set;}
    public Guid FoodItemId{get;set;}
    public FoodItem? FoodItem{get;set;}
    public int AmountInGramm{get;set;}
    public DateTimeOffset EatenAt {get;set;}
}
