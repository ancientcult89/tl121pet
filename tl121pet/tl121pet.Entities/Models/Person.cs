using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.Models
{
    public class Person
    {
        [Key, Required]
        public long PersonId { get; set; } 
        public string FirstName { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Range(1, long.MaxValue, ErrorMessage = "Required")]
        public long GradeId { get; set; }
        public Grade? Grade { get; set; }
    }
}
