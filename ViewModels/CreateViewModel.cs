using System.ComponentModel.DataAnnotations;

namespace ProjectOneMil.ViewModels
{
    public class CreateViewModel
    {
        [Required]
		public string FullName { get; set; } = String.Empty;

        [Required]
        public string UserName { get; set; } = String.Empty;
		
        [Required]
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = String.Empty;
        
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = String.Empty;


		
	}
}
