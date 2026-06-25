using CalorieCounter.ApiService;
public class FoodEndpointTests {
    private CalorieWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _token ="";
    private Guid pizzaGuid;

    [OneTimeSetUp]
    public async Task StartUp(){
        _factory = new CalorieWebApplicationFactory<Program>();
        var client = _factory.CreateClient();
        var request = new { Username = "FoodTester", Email = "food@gmail.com", Password ="Aloha123!"};
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

        await SeedFood(client);
    }

    [SetUp]
    public void Setup(){
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    private async Task SeedFood(HttpClient client){
        UserProvidedFoodItem req = new("Chicken Breast", 104, 22,0,1.8);
        var resp = await client.PostAsJsonAsync("api/enterfood", req);
        if(resp.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(resp.StatusCode.ToString());
        }
        req = new("Spaghetti",330,12.5,70.5,1.2); 
        resp = await client.PostAsJsonAsync("api/enterfood", req);
        if(resp.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(resp.StatusCode.ToString());
        }
        req = new("Olive Oil",857,0,0,91.5);
        resp =await client.PostAsJsonAsync("api/enterfood", req);
        if(resp.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail();
        }
        req = new("Pizza Margherita",231,9,35,5.5);
        resp = await client.PostAsJsonAsync("api/enterfood", req);
        if(resp.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail();
        }

        FoodItemForUser? food = await resp.Content.ReadFromJsonAsync<FoodItemForUser>();

        pizzaGuid = food?.Identification ?? pizzaGuid;
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


    [Test]
    public async Task GetPizzaById(){
        var resp = await _client.GetAsync($"/api/foods/{pizzaGuid}");

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), $"Get Failed with Status: {resp.StatusCode} and Content: ({await resp.Content.ReadAsStringAsync()})");

        FoodItemForUser? food = await resp.Content.ReadFromJsonAsync<FoodItemForUser>();

        Assert.That(food, Is.Not.Null);

        if(food is null)
            return;

        Assert.Multiple(()=> {
                Assert.That(food.Name, Is.EqualTo("Pizza Margherita"));
                Assert.That(food.Calories, Is.EqualTo(231));
                Assert.That(food.Protein, Is.EqualTo(9));
                Assert.That(food.Fat, Is.EqualTo(5.5));
                Assert.That(food.Carbs, Is.EqualTo(35));
                });

    }

    [Test]
    public async Task GetFoods(){
        FoodItemForUser[]? resp = await _client.GetFromJsonAsync<FoodItemForUser[]>("/api/foods");
        
        Assert.That(resp, Is.Not.Null);
        if(resp is null)
            return;
        Assert.That(resp, Has.Length.AtLeast(3));
        Assert.That(resp, Has.One.Matches<FoodItemForUser>(f=> f.Name == "Spaghetti" && f.Carbs == 70.5));
        Assert.That(resp, Has.One.Matches<FoodItemForUser>(f=> f.Name == "Chicken Breast" && f.Carbs == 0 && f.Protein == 22));
        Assert.That(resp, Has.One.Matches<FoodItemForUser>(f=> f.Name == "Olive Oil" && f.Calories == 857 && f.Fat== 91.5));
    }

    [Test]
    public async Task GetNonExistantFood(){
        var resp = await _client.GetAsync($"/api/foods/{Guid.Empty}");
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }

    [Test]
    public async Task TryEnterNegativeValue(){
        UserProvidedFoodItem food = new("Test", 500,-5,2,40);
        var resp = await _client.PostAsJsonAsync("/api/enterfood", food);
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        var err = await resp.Content.ReadAsStringAsync();

        Assert.That(err == "Nutritional value must be non-negative");
    }

    [Test]
    public async Task TryEnterTooLittleCalories(){
        UserProvidedFoodItem food = new("Test", 500,10,10,59);
        var resp = await _client.PostAsJsonAsync("/api/enterfood", food);
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        var err = await resp.Content.ReadAsStringAsync();

        Assert.That(err == "Calories can't be less than calories from macros");
    }

    [Test]
    public async Task TryEnterMacrosGreater100(){
        UserProvidedFoodItem food = new("Test", 9999,40,40,40);
        var resp = await _client.PostAsJsonAsync("/api/enterfood", food);
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        var err = await resp.Content.ReadAsStringAsync();

        Assert.That(err == "Protein, Carbohydrates and Fat can't be more than 100gram per 100 gram");
    }
}
