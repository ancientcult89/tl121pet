namespace tl121pet.Entities.Models
{
    public class TeamMember
    {
        public long TeamMemberId { get; set; }
        public long PersonId { get; set; }
        public Person Person { get; set; } = new Person();
        public long ProjectTeamId { get; set; }
        public ProjectTeam? ProjectTeam { get; set; }

        public float Participation { get; set; }
    }
}
