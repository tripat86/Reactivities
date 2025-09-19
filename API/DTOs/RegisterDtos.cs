using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDtos
    {
        [Required]
        public string DisplayName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        //[Required] - Here we dont need [Required] because Identity will enforce complex password rules
        public string  Password { get; set; } = "";
    }
}