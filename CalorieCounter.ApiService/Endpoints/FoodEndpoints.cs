using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace CalorieCounter.ApiService;

public static class FoodEndpoints{

    public static void MapFoodEndpoints(this WebApplication app){
        app.MapGet("/api/foods", GetFoods)
           .WithName("GetFoods")
           .RequireAuthorization();

        app.MapGet("/api/foods/{id}", GetSpecificFood)
           .WithName("GetSpecificFood")
           .RequireAuthorization();


        app.MapPost("/api/foods", EnterFood)
           .WithName("EnterFood")
           .RequireAuthorization();

    }

   private static async Task<Results<Created<FoodItemForUser>, BadRequest<string>>> EnterFood(UserProvidedFoodItem request, CalorieDbContext db)
   {
       (bool valid, string reason) = ValidateFood(request);
       if(!valid)
          return TypedResults.BadRequest(reason);
       var res = db.FoodItems.Add(new FoodItem(){
                    Name = request.Name,
                    CaloriesPer100g = request.Calories,
                    Protein = request.Protein,
                    Carbohydrates = request.Carbs,
                    Fat = request.Fat,
                    LastModified = DateTimeOffset.UtcNow,
                    InternalId = Guid.NewGuid()
                    });


       var savedCount = await db.SaveChangesAsync();

       return TypedResults.Created($"/api/foods/{res.Entity.InternalId}", 
              new FoodItemForUser(res.Entity.Name,res.Entity.CaloriesPer100g,res.Entity.Protein,res.Entity.Carbohydrates,res.Entity.Fat, res.Entity.InternalId,res.Entity.LastModified));
   }

   private static async Task<Results<Ok<FoodItemForUser>, NotFound>> GetSpecificFood(Guid id, CalorieDbContext db)
   {
      var item = await db.FoodItems.FirstOrDefaultAsync(x => x.InternalId == id);
      if (item is null)
         return TypedResults.NotFound();

      return TypedResults.Ok(new FoodItemForUser(item.Name, item.CaloriesPer100g, item.Protein, item.Carbohydrates, item.Fat, item.InternalId, item.LastModified));
   }

    private static async Task<Ok<List<FoodItemForUser>>> GetFoods(string? name, CalorieDbContext db)
    {
      var query = string.IsNullOrWhiteSpace(name) ? db.FoodItems
                                                              : db.FoodItems.Where(x => x.Name.Contains(name));


      var items = await query.Select(item => new FoodItemForUser(item.Name, item.CaloriesPer100g, item.Protein, item.Carbohydrates, item.Fat, item.InternalId, item.LastModified))
                             .ToListAsync();

      return TypedResults.Ok(items);
   }

    private static (bool valid, string reason) ValidateFood(UserProvidedFoodItem item){
        if(item.Calories < 0 || item.Protein <0 || item.Carbs <0 || item.Fat <0)
            return (false, "Nutritional value must be non-negative");

        if(item.Calories +5 < ((item.Protein + item.Carbs)*4 + (item.Fat * 9)))
        {
            return (false, "Calories can't be less than calories from macros");
        }



        return (true,string.Empty);
    }
}

