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
            Assert.Fail(resp.StatusCode.ToString() + await resp.Content.ReadAsStringAsync());
        }
        UserProvidedFoodItem req2 = new("Spaghetti",359,13.5,70.2,2); 
        var resp2 = await client.PostAsJsonAsync("api/enterfood", req2);
        if(resp2.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(resp2.StatusCode.ToString() + await resp2.Content.ReadAsStringAsync());
        }
        UserProvidedFoodItem req3 = new("Olive Oil",857,0,0,91.5);
        var resp3 =await client.PostAsJsonAsync("api/enterfood", req3);
        if(resp3.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(resp3.StatusCode.ToString() + await resp3.Content.ReadAsStringAsync());
        }
        UserProvidedFoodItem req4 = new("Pizza Margherita",231,9,35,5.5);
        var resp4 = await client.PostAsJsonAsync("api/enterfood", req4);
        if(resp4.StatusCode != System.Net.HttpStatusCode.Created){
            Assert.Fail(resp4.StatusCode.ToString() + await resp4.Content.ReadAsStringAsync());
        }

        FoodItemForUser? food = await resp4.Content.ReadFromJsonAsync<FoodItemForUser>();

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
    public async Task FoodWithStringQuerySuccess(){
        var resp = await _client.GetAsync("/api/foods?name=Spag");
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        var cont = await resp.Content.ReadFromJsonAsync<FoodItemForUser[]>();
        if(cont is null)
        {
            Assert.Fail();
            return;
        }
        
        Assert.That(cont.Length, Is.EqualTo(1));
        Assert.That(cont[0].Name, Is.EqualTo("Spaghetti"));
        Assert.That(cont[0].Calories, Is.EqualTo(359));
    }

    [Test]
    public async Task FoodWithStringQueryEmpty(){
        var resp = await _client.GetAsync("/api/foods?name=Alphanumeric");

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        var cont = await resp.Content.ReadFromJsonAsync<FoodItemForUser[]>();
        if(cont is null)
        {
            Assert.Fail();
            return;
        }
        
        Assert.That(cont.Length, Is.EqualTo(0));
    }

    [Test]
    public async Task EnterFoodSuccess(){
        var req = new UserProvidedFoodItem("TestFood", 300, 30,10,10);
        var resp = await _client.PostAsJsonAsync("/api/enterfood", req);

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));
        FoodItemForUser? food = await resp.Content.ReadFromJsonAsync<FoodItemForUser>();
        Assert.That(food, Is.Not.Null);

        
        Assert.Multiple(()=> {
                Assert.That(food!.Name, Is.EqualTo(req.Name));
                Assert.That(food.Calories, Is.EqualTo(req.Calories));
                Assert.That(food.Protein, Is.EqualTo(req.Protein));
                Assert.That(food.Fat, Is.EqualTo(req.Fat));
                Assert.That(food.Carbs, Is.EqualTo(req.Carbs));
                Assert.That(food.Identification, Is.Not.EqualTo(Guid.Empty));
                Assert.That(food.LastModified, Is.LessThan(DateTimeOffset.Now));
                });

        var uri = resp.Headers.Location;

        Assert.That(uri, Is.Not.Null);

        var get = await _client.GetAsync(uri);
        Assert.That(get.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        var food2 = await get.Content.ReadFromJsonAsync<FoodItemForUser>();
        
        Assert.That(food2, Is.EqualTo(food));
    }

    [Test]
    public async Task GetFoods(){
        FoodItemForUser[]? resp = await _client.GetFromJsonAsync<FoodItemForUser[]>("/api/foods");
        
        Assert.That(resp, Is.Not.Null);
        if(resp is null)
            return;
        Assert.That(resp, Has.Length.AtLeast(3));
        Assert.That(resp, Has.One.Matches<FoodItemForUser>(f=> f.Name == "Spaghetti" && f.Carbs == 70.2));
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
        var err = await resp.Content.ReadFromJsonAsync<string>();

        Assert.That(err, Is.EqualTo("Nutritional value must be non-negative"), err);
    }

    [Test]
    public async Task TryEnterExactCalories(){
        UserProvidedFoodItem food = new("Test", 50,10,2.5,0);
        var resp = await _client.PostAsJsonAsync("/api/enterfood", food);
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));
    }

    [Test]
    public async Task TryEnterTooLittleCalories(){
        UserProvidedFoodItem food = new("Test", 500,10,10,59);
        var resp = await _client.PostAsJsonAsync("/api/enterfood", food);
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        var err = await resp.Content.ReadFromJsonAsync<string>();

        Assert.That(err, Is.EqualTo("Calories can't be less than calories from macros"), err);
    }

    [Test]
    public async Task TryEnterMacrosExact100(){
        UserProvidedFoodItem food = new("Test", 9999,40,30,30);
        var resp = await _client.PostAsJsonAsync("/api/enterfood", food);
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));

    }

    [Test]
    public async Task TryEnterMacrosGreater100(){
        UserProvidedFoodItem food = new("Test", 9999,40,40,40);
        var resp = await _client.PostAsJsonAsync("/api/enterfood", food);
        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        var err = await resp.Content.ReadFromJsonAsync<string>();

        Assert.That(err, Is.EqualTo("Protein, Carbohydrates and Fat can't be more than 100gram per 100 gram"),err);
    }
}
