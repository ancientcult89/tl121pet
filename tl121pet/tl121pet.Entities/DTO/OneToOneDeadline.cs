using tl121pet.Entities.Models;

namespace tl121pet.Entities.DTO
{
    public class OneToOneDeadline
    {
        public Person Person { get; set; } = new Person();
        public Meeting LastMeetingOneToOne { get; set; } = new Meeting();
    }
}
