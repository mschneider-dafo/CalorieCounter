using CalorieCounter.ApiService;
public class MealEndpointTests
{
    private CalorieWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _token ="";
    private Guid m1g,m2g,m3g;

    [OneTimeSetUp]
    public async Task StartUp(){
        _factory = new CalorieWebApplicationFactory<Program>();
        var client = _factory.CreateClient();
        var request = new { Username = "MealTester", Email = "Meal@gmail.com", Password ="Aloha123!"};
        var resp = await client.PostAsJsonAsync("/api/register", request);
        if(resp.StatusCode != System.Net.HttpStatusCode.OK){
            Assert.Fail("Creating of Test user failed");
        }
        var content  = await resp.Content.ReadFromJsonAsync<AuthOkResponse>();
        if(content is not null)
            _token = content.Token;
        else
            Assert.Fail();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        await SeedMeals(client);
    }

    [SetUp]
    public void Setup(){
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    [TearDown]
    public void TearDown() {
        _client.Dispose();
        _client = null!;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown(){
        _factory.Dispose();
        _factory = null!;
    }

    private async Task SeedMeals(HttpClient client){
        UserProvidedFoodItem f1 = new ("Skyr", 70,10,4,3);
        var resp = await client.PostAsJsonAsync("api/enterfood", f1);
        if(resp.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(resp.StatusCode.ToString() + await resp.Content.ReadAsStringAsync());
        }

        FoodItemForUser? rf = await resp.Content.ReadFromJsonAsync<FoodItemForUser>();

        if(rf is null){
            Assert.Fail("Result is null");
            return;
        }

        MealRequest m1 = new(200,rf.Identification,null);
        MealRequest m2 = new(450,rf.Identification,new DateTimeOffset(2025,10,31,10,15,15, TimeSpan.FromHours(1)));
        MealRequest m3 = new(450,rf.Identification,new DateTimeOffset(2026,10,31,10,15,15,TimeSpan.FromHours(1)));

        var rm1 = await client.PostAsJsonAsync("api/foodentry",m1);
        if(rm1.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(rm1.StatusCode.ToString()+ await rm1.Content.ReadAsStringAsync());
        }
        m1g= (await rm1.Content.ReadFromJsonAsync<MealForUsers>())?.Id ?? Guid.Empty;
        var rm2 = await client.PostAsJsonAsync("api/foodentry",m2);
        if(rm2.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(rm2.StatusCode.ToString()+ await rm2.Content.ReadAsStringAsync());
        }
        m2g= (await rm2.Content.ReadFromJsonAsync<MealForUsers>())?.Id ?? Guid.Empty;
        var rm3 = await client.PostAsJsonAsync("api/foodentry",m3);
        if(rm3.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(rm3.StatusCode.ToString()+ await rm3.Content.ReadAsStringAsync());
        }
        m3g= (await rm3.Content.ReadFromJsonAsync<MealForUsers>())?.Id ?? Guid.Empty;
    }
}
