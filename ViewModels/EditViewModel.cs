﻿using System.ComponentModel.DataAnnotations;

namespace ProjectOneMil.ViewModels
{
    public class EditViewModel
    {
        
        public string? Id { get; set; } 

		public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        
        [DataType(DataType.Password)]
        public string? Password { get; set; } 
        
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; } 

		
	}
}
