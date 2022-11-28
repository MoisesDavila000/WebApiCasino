using System.ComponentModel.DataAnnotations;

namespace WebApiCasino.DTOs
{
    public class EditarAdmin
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
