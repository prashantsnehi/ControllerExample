using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace ControllerExample.Models;

public class Person
{
    public Guid PersonId { get; set; } = Guid.NewGuid();
     
    [Required(ErrorMessage = "{0} can't be null")]
    [Display(Name = "Person Name")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "{0} should be between {2} and {1}")]
    [RegularExpression("^[A-Za-z]{3,30} [A-Za-z]{3,30}$")]
    public string? PersonName { get; set; }

    // [Required(ErrorMessage = "{0} should be greater than or equal to 18")]
    // [Range(18, 60, ErrorMessage = "{0} should be between {1} and {2}")]
    // public int Age { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [EmailAddress(ErrorMessage = "Please enter valid {0}")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [Phone(ErrorMessage = "Enter Valid Phone Number")]
    [DataType(DataType.PhoneNumber)]
    public long Phone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "{0} should be between {2} and {1}")]
    [RegularExpression("^[A-Za-z0-9@#$%^&+=]*$", ErrorMessage = "{0} can only contain letters, numbers, and special characters @#$%^&+=")]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [Compare("Password", ErrorMessage = "{0} and {1} don't match")]
    [Display(Name = "Confirm Password")]
    public string? ConfirmPassword { get; set; }

    [ValidateNever]
    public string? Profession { get; set; }
}