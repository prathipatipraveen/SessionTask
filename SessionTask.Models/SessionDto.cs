using System;

namespace SessionTask.Models
{
    public class SessionDto
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string Description { get; set; }
        public string HostName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EventId { get; set; }
        public string MaxCount { get; set; }
        public bool IsEnrolled { get; set; }
        public bool IsApproved { get; set; }
    }
}
