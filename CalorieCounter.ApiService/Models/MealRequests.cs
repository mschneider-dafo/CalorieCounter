public record MealForUsers(Guid Id, string FoodName,double Calories, double Protein, double Carbs, double Fat, int AmountInGramm, DateTimeOffset EatenAt);

public record MealRequest(int AmountInGramm, Guid FoodItem, DateTimeOffset? EatenAt);
