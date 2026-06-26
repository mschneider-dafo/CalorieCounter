using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq.Expressions;
namespace CalorieCounter.ApiService;

public static class MealEndpoints{
    public static void MapMealEndpoints(this WebApplication app){
        app.MapGet("/api/foodentries", GetFoodEntries)
            .WithName("Get Meals")
            .RequireAuthorization();

        app.MapGet("/api/foodentry/{id}", GetFoodEntry)
            .WithName("Get Meal")
            .RequireAuthorization();

        app.MapDelete("/api/foodentry/{id}", DeleteFoodEntry)
            .WithName("Delete Meal")
            .RequireAuthorization();

        app.MapPost("/api/foodentry", EnterFoodEntry)
            .WithName("Enter Meal")
            .RequireAuthorization();

    }

    public static Expression<Func<FoodEntry,MealForUsers>> ToMealExpression = item => new MealForUsers(item.PublicIdentifier,
                item.FoodItem!.Name,
                item.FoodItem.CaloriesPer100g * item.AmountInGramm,
                item.FoodItem.Protein * item.AmountInGramm, 
                item.FoodItem.Carbohydrates * item.AmountInGramm, 
                item.FoodItem.Fat * item.AmountInGramm, 
                item.AmountInGramm,
                item.EatenAt);

    private static MealForUsers ToMeal(FoodEntry item){
        return new MealForUsers(item.PublicIdentifier,
                item.FoodItem?.Name ?? "",
                (item.FoodItem?.CaloriesPer100g ?? 0)*item.AmountInGramm,
                (item.FoodItem?.Protein ?? 0)* item.AmountInGramm, 
                (item.FoodItem?.Carbohydrates ?? 0)*item.AmountInGramm, 
                (item.FoodItem?.Fat ?? 0)* item.AmountInGramm, 
                item.AmountInGramm,
                item.EatenAt);
    }
    
    private static async Task<Results<Created<MealForUsers>,BadRequest<string>>>  EnterFoodEntry(MealRequest request, UserManager<User> manager, CalorieDbContext db, HttpContext context){
        var user = await manager.GetUserAsync(context.User);
        if(user is null)
            return TypedResults.BadRequest("User does not exist");

        var foodItem =  await db.FoodItems.FirstOrDefaultAsync(x=> x.InternalId == request.FoodItem);

        if(foodItem is null)
            return TypedResults.BadRequest("FoodItem Not Found");

        FoodEntry entry = new FoodEntry(){
            PublicIdentifier = Guid.NewGuid(),
            UserId = user.Id ,
            User = user,
            FoodItemId = foodItem.InternalId,
            FoodItem = foodItem,
            AmountInGramm = request.AmountInGramm,
            EatenAt = request.EatenAt ?? DateTimeOffset.UtcNow,
        };

        db.FoodEntries.Add(entry);

        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/foodentry/{entry.PublicIdentifier}", ToMeal(entry));
    }

    private static async Task<Results<Ok<List<MealForUsers>>,BadRequest<string>>> GetFoodEntries(DateTimeOffset? after, DateTimeOffset? before, UserManager<User> manager, CalorieDbContext db, HttpContext context){
        var user = await manager.GetUserAsync(context.User);

        DateTimeOffset s = after ?? DateTimeOffset.MinValue;
        DateTimeOffset e = before ?? DateTimeOffset.MaxValue;

        if(s > e)
            return TypedResults.BadRequest("Start time is after end time");

        return TypedResults.Ok(await db.FoodEntries.Where(x=> x.User == user && x.EatenAt >=s && x.EatenAt <= e).Select(ToMealExpression).ToListAsync());
    }

    private static async Task<Results<Ok<MealForUsers>,NotFound>> GetFoodEntry(Guid id, UserManager<User> manager, CalorieDbContext db, HttpContext context){
        var user = await manager.GetUserAsync(context.User);

        var res = await db.FoodEntries.FirstOrDefaultAsync(x=> x.User == user && x.PublicIdentifier == id);
        if(res is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(ToMeal(res));
    }

    private static async Task<Results<Ok,NotFound>> DeleteFoodEntry(Guid id, UserManager<User> manager, CalorieDbContext db, HttpContext context){
        var user = await manager.GetUserAsync(context.User);

        var res = await db.FoodEntries.FirstOrDefaultAsync(x=> x.User == user && x.PublicIdentifier == id);
        if(res is null)
            return TypedResults.NotFound();

        db.FoodEntries.Remove(res);

        await db.SaveChangesAsync();

        return TypedResults.Ok();
    }

}
