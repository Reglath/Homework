using Homework.Contexts;
using Homework.Models.DTOs;
using HomeworkTest.Helpers;
using HomeworkTest.Seeding;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace HomeworkTest.IntegrationTests
{
    public class LoginEndpointTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory factory;
        private readonly ApplicationDbContext context;
        private readonly HttpClient client;
        private readonly JwtHelper jwtHelper;

        public LoginEndpointTest(CustomWebApplicationFactory customWebApplicationFactory)
        {
            factory = customWebApplicationFactory;
            client = factory.CreateClient();
            context = ContextProvider.CreateContextFromFactory(factory);
            DBSeed.Seed(context);
            var configuration = new ConfigurationBuilder().AddUserSecrets<LoginEndpointTest>().Build();
            jwtHelper = new JwtHelper(configuration);
            var token = jwtHelper.GenerateJwtToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Theory]
        [InlineData("/login", "username1", "password1")]

        public async Task HttpStatusOk(string url, string username, string password)
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var dto = new LoginDTO()
            {
                Username = username,
                Password = password,
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/register", "", "password5")]
        [InlineData("/register", "username5", "")]
        [InlineData("/register", "username1", "password1")]
        public async Task HttpStatusNotOk(string url, string username, string password)
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var registerDTO = new RegisterDTO()
            {
                Username = username,
                Password = password,
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDTO), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
