using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model.DTO
{
    public record RecaptchaResponseDTO
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("score")]
        public float Score { get; init; }
    }
}
