﻿//грейд, имя, команда
namespace tl121pet.Entities.Models
{
    public class Person
    {
        public long PersonId { get; set; } 
        public string FirstName { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public long GradeId { get; set; }
        public long ProjectTeamId { get; set; }

        public Grade? Grade { get; set; }
        public ProjectTeam? ProjectTeam { get; set; }
    }
}
