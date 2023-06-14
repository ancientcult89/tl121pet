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
        public string ShortName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Range(1, long.MaxValue, ErrorMessage = "Required")]
        public long GradeId { get; set; }
        public Grade? Grade { get; set; }
    }

    public class PersonComparer : EqualityComparer<Person>
    {
        public override bool Equals(Person p1, Person p2)
        {
            if (p1 == p2)
                return true;

            if ((p1 == null && p2 != null) || (p1 != null && p2 == null))
                return false;

            return p1.PersonId == p2.PersonId;
        }

        public override int GetHashCode(Person p1) => p1 != null && p1.PersonId != null ? p1.PersonId.GetHashCode() : 0;
    }
}
