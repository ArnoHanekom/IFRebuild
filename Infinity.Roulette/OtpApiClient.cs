using DocumentFormat.OpenXml.Spreadsheet;
using System.Net.Http;
using Newtonsoft.Json;

namespace Infinity.Roulette;

public class OtpApiClient(string apiKey, string apiBaseUrl)
{
    private HttpClient _otpApi { get; set; } = new HttpClient();

    public async Task<string> GenerateOtpAsync(string phoneNumber)
    {
        _otpApi.DefaultRequestHeaders.Add("API-Key", apiKey);
        var requestData = new
        {
            phoneNumber
        };
        var content = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");
        var response = await _otpApi.PostAsync($"{apiBaseUrl}/generate-otp", content);

        if (!response.IsSuccessStatusCode) throw new Exception($"OTP generation failed. StatusCode: {response.StatusCode}");

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<dynamic>(responseContent)!["otp"];
    }
}