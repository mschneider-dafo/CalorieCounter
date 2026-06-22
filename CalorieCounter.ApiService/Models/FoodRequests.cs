public record UserProvidedFoodItem (string Name, double Calories, double Protein, double Carbs, double Fat);

public record FoodItemForUser(string Name, double Calories, double Protein, double Carbs, double Fat, Guid Identification, DateTimeOffset LastModified);
