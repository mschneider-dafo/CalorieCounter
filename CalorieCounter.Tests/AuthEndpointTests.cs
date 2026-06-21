namespace CalorieCounter.Tests;

public class AuthEndpointTests
{
    private CalorieWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void StartUp(){
        _factory = new CalorieWebApplicationFactory<Program>();
    }

    [OneTimeTearDown]
    public void End(){
        _factory.Dispose();
    }

    [SetUp]
    public void Setup(){
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void Teardown(){
        _client.Dispose();
    }

    [Test]
    [TestCase("alllowercase123!")]
    [TestCase("NOLOWER123!")]
    [TestCase("Sa12!")]
    [TestCase("NoSpecial1")]
    public async Task Register_BadRequest_InvalidPassword(string pw){
        var request = new { Username = "t", Email = "t@gmail.com", Password =pw};

        var resp = await _client.PostAsJsonAsync("/api/register", request);

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Register_ReturnsOk_WithValidCredentials(){
        var request = new { Username = "test", Email = "test@gmail.com", Password ="Aloha123!"};

        var resp = await _client.PostAsJsonAsync("/api/register", request);

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var body =  await resp.Content.ReadFromJsonAsync<CalorieCounter.ApiService.AuthOkResponse>();

        Assert.That(body?.Token, Is.Not.Null.Or.Empty);
        Assert.That(body?.ExpiresAt, Is.GreaterThan(DateTimeOffset.Now));
    }

    [Test]
    public async Task Register_UserTwice_SameUsername(){
        var request = new { Username = "test2", Email = "test2@gmail.com", Password ="Aloha123!"};

        var resp = await _client.PostAsJsonAsync("/api/register",request);

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        request = request with { Email = "test2+2@gmail.com"};

        var resp2 = await _client.PostAsJsonAsync("/api/register",request);

        Assert.That(resp2.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest),await resp2.Content.ReadAsStringAsync());
    }

    [Test]
    public async Task Register_UserTwice_SameEmail(){
        var request = new { Username = "Arthur", Email = "Arthur@gmail.com", Password ="Aloha123!"};

        var resp = await _client.PostAsJsonAsync("/api/register",request);

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        request = request with { Username = "Arthur+1"};

        var resp2 = await _client.PostAsJsonAsync("/api/register",request);

        Assert.That(resp2.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest),await resp2.Content.ReadAsStringAsync());
    }

    [Test]
    public async Task Login_Invalid_NotRegisteredUser(){
        var request = new  {Username="slut", Password ="abc"};

        var resp = await _client.PostAsJsonAsync("/api/login",request);

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Login_Invalid_WrongPassword(){
        var request = new { Username = "Susan", Email = "susan@gmail.com", Password ="Aloha123!"};

        var resp = await _client.PostAsJsonAsync("/api/register",request);

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        var logReq = new {Username="Susan", Password = "Strange!23"};

        var logResp = await _client.PostAsJsonAsync("/api/login", logReq);

        Assert.That(logResp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RegisterThenLogin_ValidReturnsCredentials(){
        var request = new { Username = "test3", Email = "test3@gmail.com", Password ="Aloha123!"};

        var resp = await _client.PostAsJsonAsync("/api/register",request);

        Assert.That(resp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));


        var logReq = new {Username = "test3", Password = "Aloha123!"};

        var logResp = await _client.PostAsJsonAsync("/api/login", logReq);

        Assert.That(logResp.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var body =  await logResp.Content.ReadFromJsonAsync<CalorieCounter.ApiService.AuthOkResponse>();

        Assert.That(body?.Token, Is.Not.Null.Or.Empty);
        Assert.That(body?.ExpiresAt, Is.GreaterThan(DateTimeOffset.Now));

    }
}
