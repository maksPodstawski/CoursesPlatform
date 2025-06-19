using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IBL;
using Microsoft.Extensions.Configuration;
using Model.DTO;

namespace BL.Services
{
    public class RecaptchaService: IRecaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public RecaptchaService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _secretKey = config["GoogleReCaptcha:SecretKey"] ?? throw new ArgumentNullException("Recaptcha:SecretKey is not configured.");
        }

        public async Task<bool> VerifyAsync(string token)
        {
            var response = await _httpClient.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", _secretKey),
                    new KeyValuePair<string, string>("response", token)
                })
            );

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"reCAPTCHA response: {json}");
            var result = JsonSerializer.Deserialize<RecaptchaResponseDTO>(json);
            return result.Success && result.Score >= 0.5;
        }
    }
}
