using Microsoft.AspNetCore.Mvc;

namespace ASP1.Models.Home.SignUp
{
    public class SignUpFormModel
    {
        [FromForm(Name = "signup-username")]
        public String UserName { get; set; } = null!;
        [FromForm(Name = "signup-email")]
        public String UserEmail { get; set; } = null!;

        [FromForm(Name = "signup-password")]
        public String Password { get; set; } = null!;

        [FromForm(Name = "signup-repeat")]
        public String PasswordRepeat { get; set; }= null!;

        [FromForm(Name = "signup-avatar")]
        public IFormFile AvatarFile { get; set; } = null!;

        [FromForm(Name = "signup-birthdate")]
        public DateTime? Birthdate { get; set; }=null!;

        [FromForm(Name = "signup-confirm")]
        public bool Confirm { get; set; }

        //дополнительное поле (не из формы) для имени сохраненного  файла
        public String? SavedFileName {  get; set; }
    }
}
