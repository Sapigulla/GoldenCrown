using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos.User
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Поле login обязательено")]
        [MinLength(3, ErrorMessage = "Минимальная длина логина от 3 символов")]
        public string Login { get; set; }


        [Required(ErrorMessage = "Поле name обязательено")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле password обязательено")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля от 6 символов")]
        public string Password { get; set; }
    }
}
