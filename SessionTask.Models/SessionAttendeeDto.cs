using System;
using System.Collections.Generic;
using System.Text;

namespace SessionTask.Models
{
    public class SessionAttendeeDto
    {
        public int UserId { get; set; }
        public string AttendeeName { get; set; }
        public string SessionName { get; set; }
        public string EventName { get; set; }
        public string HostName { get; set; }
        public bool IsApproved { get; set; }
    }
}
