using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos.Finance
{
    public class TransactionHistoryRequest
    {
        [FromQuery]
        [Required(ErrorMessage = "Поле Token обязательно")]
        public string Token { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Поле Limit должно быть не меньше 1")]
        public int Limit { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Поле Offset не может быть отрицательным")]
        public int Offset { get; set; }
    }
}
