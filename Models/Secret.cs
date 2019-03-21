using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Secrets.Models
{
    public class Secret
    {
        [Key]
        public int SecretId {get; set;}

        [Required (ErrorMessage = "What's your secret?")]
        [MinLength(10, ErrorMessage="Post must be atleast 10 characters long")]
        public string Post {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
        public int UserId {get; set;}
        public User Poster {get; set;}
        public List <Like> Likes {get; set;}
    }

    public class Like
    {
        [Key]
        public int LikeId {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
        public int UserId {get; set;}
        public User User {get; set;}
        public int SecretId {get; set;}
        public Secret Secret {get; set;}
    }

    public class AllSecrets
    {
        public List<Secret> allSecrets {get; set;}
        public Secret newSecret {get; set;}
    }
}
