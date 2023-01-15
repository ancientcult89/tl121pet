using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.DTO
{
    //TODO: сомнительное присутсвие данного класса в разделе ДТО, но перемещать в модель - идея мне тоже не нравится. Подумать куда определить данный класс
    public class OneToOneDeadline
    {
        public Person Person { get; set; } = new Person();
        public Meeting LastMeetingOneToOne { get; set; } = new Meeting();
        public DateTime LastOneToOneMeetingDate { get; set; } = DateTime.Now;
        public AlertLevel AlertLVL { get; set; } = AlertLevel.None;
        public int DayToDeadline = 0;
    }
}
