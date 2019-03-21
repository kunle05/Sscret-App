using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Secrets.Models
{
    public class User
    {
       [Key]
       public int UserId {get; set;}

       [Required]
       [MinLength(2, ErrorMessage="First name must be atleast 2 characters")]
       [Display(Name = "First Name")]
       public string FirstName {get; set;}

       [Required]
       [MinLength(2, ErrorMessage="Last name must be atleast 2 characters")]
       [Display(Name = "Last Name")]
       public string LastName {get; set;}

       [Required]
       [DataType(DataType.EmailAddress, ErrorMessage="Invalid Email Address")]
       public string Email {get; set;}

       [Required]
       [MinLength(8, ErrorMessage="Password must be atleast 8 character long")]
       [DataType(DataType.Password)]
       public string Password {get; set;}

       public DateTime CreatedAt {get; set;} = DateTime.Now;
       public DateTime UpdatedAt {get; set;} = DateTime.Now;
       public List<Secret> PostedSecrets {get; set;}
       public List<Like> LikedPost {get; set;}

       [NotMapped]
       [Compare("Password", ErrorMessage="Passwords does not match")]
       [DataType(DataType.Password)]
       [Display(Name = "Confirm Password")]
       public string Confirm {get; set;}       
    }

    public class myUser
    {
       [Required]
       [DataType(DataType.EmailAddress, ErrorMessage="Invalid Email Address")]
       public string Email {get; set;}

       [Required]
       [MinLength(8, ErrorMessage="Password must be atleast 8 character long")]
       [DataType(DataType.Password)]
       public string Password {get; set;}
    }

    public class LoginRegistration
    {
        public myUser myUser {get; set;}
        public User User {get; set;}
    }
}