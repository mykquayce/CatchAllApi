using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace CatchAllApi.WebApi.Tests;

public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _httpClient;
	private readonly CollectionLoggerProvider _loggerProvider;

	public UnitTest1(WebApplicationFactory<Program> factory)
	{
		_loggerProvider = new CollectionLoggerProvider();
		var options = new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false, };

		_httpClient = factory
			.WithWebHostBuilder(builder =>
			{
				builder.ConfigureLogging(b =>
				{
					b.ClearProviders()
						.AddProvider(_loggerProvider);
				});
			})
			.CreateClient(options);
	}

	[Theory]
	[InlineData("get", "")]
	public async Task SimpleRequestTests(string method, string requestUri)
	{
		// Arrange
		var request = new HttpRequestMessage(HttpMethod.Parse(method), requestUri);

		// Act
		var response = await _httpClient.SendAsync(request);

		// Assert
		Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);
		Assert.NotEmpty(_loggerProvider.Entries);
		Assert.DoesNotContain(default, _loggerProvider.Entries);
	}

	[Theory]
	[InlineData("client-id", "client-secret")]
	public async Task BearerTokenTests(string clientId, string clientSecret)
	{
		// Arrange
		var request = new HttpRequestMessage(HttpMethod.Post, requestUri: "oauth2/token")
		{
			Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				["client_id"] = clientId,
				["client_secret"] = clientSecret,
				["grant_type"] = "client_credentials",
			}),
		};

		// Act
		var response = await _httpClient.SendAsync(request);

		// Assert
		Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);
		Assert.NotEmpty(_loggerProvider.Entries);
		Assert.DoesNotContain(default, _loggerProvider.Entries);
	}

	[Theory]
	[InlineData("client-id", "client-secret")]
	public async Task BasicAuthTokenTests(string clientId, string clientSecret)
	{
		// Arrange
		var authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
		var request = new HttpRequestMessage(HttpMethod.Post, requestUri: "oauth2/token")
		{
			Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				["grant_type"] = "client_credentials",
			}),
			Headers =
			{
				Authorization = new AuthenticationHeaderValue("Basic", authorization),
			}
		};

		// Act
		var response = await _httpClient.SendAsync(request);

		// Assert
		Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);
		Assert.NotEmpty(_loggerProvider.Entries);
		Assert.DoesNotContain(default, _loggerProvider.Entries);
	}
}
