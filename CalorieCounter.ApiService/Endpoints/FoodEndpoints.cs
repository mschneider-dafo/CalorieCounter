namespace CalorieCounter.ApiService;

public static class FoodEndpoints{

    public static void MapFoodEndpoints(this WebApplication app){
        app.MapPost("/api/foods", async ( UserProvidedFoodItem food, CalorieDbContext db) => {
            })
            .RequireAuthorization();
    }

}


