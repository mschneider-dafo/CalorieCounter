using Microsoft.EntityFrameworkCore;
namespace CalorieCounter.ApiService;

public static class FoodEndpoints{

    public static void MapFoodEndpoints(this WebApplication app){
        app.MapGet("/api/foods", async (string? name,CalorieDbContext db) =>{
            var query = string.IsNullOrWhiteSpace(name) ? db.FoodItems
                                                        : db.FoodItems.Where(x=> x.Name.Contains(name));


            var items = await query.Select(item=> new FoodItemForUser(item.Name,item.CaloriesPer100g,item.Protein,item.Carbohydrates, item.Fat, item.InternalId, item.LastModified))
                                   .ToListAsync();

            return Results.Ok(items);
        })
        .RequireAuthorization();

        app.MapGet("/api/foods/{id}", async (Guid id,CalorieDbContext db) =>{
            var item = await db.FoodItems.FirstOrDefaultAsync(x=> x.InternalId == id);
            if(item is null)
                return Results.NotFound();

            return Results.Ok(new FoodItemForUser(item.Name,item.CaloriesPer100g,item.Protein,item.Carbohydrates, item.Fat, item.InternalId, item.LastModified));
        })
        .RequireAuthorization();


        app.MapPost("/api/foods", async(UserProvidedFoodItem request, CalorieDbContext db) =>{
            (bool valid, string reason) = ValidateFood(request);
            if(!valid)
                return Results.BadRequest(reason);

            var res = await db.FoodItems.AddAsync(new FoodItem(){
                    Name = request.Name,
                    CaloriesPer100g = request.Calories,
                    Protein = request.Protein,
                    Carbohydrates = request.Carbs,
                    Fat = request.Fat,
                    LastModified = DateTimeOffset.UtcNow,
                    InternalId = Guid.NewGuid()
                    });


            var savedCount = await db.SaveChangesAsync();
            if(savedCount ==0) return Results.Problem("Failed to save");

            return Results.Created($"/api/foods/{res.Entity.InternalId}", 
                    new FoodItemForUser(res.Entity.Name,res.Entity.CaloriesPer100g,res.Entity.Protein,res.Entity.Carbohydrates,res.Entity.Fat, res.Entity.InternalId,res.Entity.LastModified));

        }).RequireAuthorization();

    }

    private static (bool valid, string reason) ValidateFood(UserProvidedFoodItem item){
        if(item.Calories < 0 || item.Protein <0 || item.Carbs <0 || item.Fat <0)
            return (false, "Nutritional value must be non-negative");

        var diff = item.Calories - ((item.Protein + item.Carbs)*4 + (item.Fat*9));

        if (Math.Abs(diff)>=1)
            return (false, "Protein, Carbohydrates and Fat do not add up to calories");

        return (true,string.Empty);
    }
}

