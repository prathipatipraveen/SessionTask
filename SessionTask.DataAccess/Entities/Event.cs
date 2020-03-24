using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? MaxCount { get; set; }
    }
}
