using System.ComponentModel.DataAnnotations;

//грейд, имя, команда
namespace tl121pet.Entities.Models
{
    public class Person
    {
        [Key, Required]
        public long PersonId { get; set; } 
        public string FirstName { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long GradeId { get; set; }
        public Grade? Grade { get; set; }
    }
}
