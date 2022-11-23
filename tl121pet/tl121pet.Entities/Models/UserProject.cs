namespace tl121pet.Entities.Models
{
    public class UserProject
    {
        public long UserProjectId { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public long ProjectTeamId { get; set; }
        public ProjectTeam? ProjectTeam { get; set; }
    }
}
