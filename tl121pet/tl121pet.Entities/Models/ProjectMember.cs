namespace tl121pet.Entities.Models
{
    /// <summary>
    /// Person project membership
    /// </summary>
    public class ProjectMember
    {
        public long ProjectMemberId { get; set; }
        public long PersonId { get; set; }
        public Person? Person { get; set; }
        public long ProjectTeamId { get; set; }
        public ProjectTeam? ProjectTeam { get; set; }
    }
}
