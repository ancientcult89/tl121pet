﻿using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Aggregate
{
    public class OneToOneDeadline
    {
        public Person Person { get; set; } = new Person();
        public Meeting LastMeetingOneToOne { get; set; } = new Meeting();
        public DateTime? LastOneToOneMeetingDate { get; set; } = DateTime.Now;
        public DateTime DeadLine { get; set; }
        public int DayToDeadline { get; set; } = 0;
    }
}
