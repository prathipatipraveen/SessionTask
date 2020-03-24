using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class Session
    {
        public Session()
        {
            UserSessionXref = new HashSet<UserSessionXref>();
        }

        public int SessionId { get; set; }
        public int EventId { get; set; }
        public string SessionName { get; set; }
        public string Description { get; set; }
        public string HostName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? MaxCount { get; set; }

        public virtual ICollection<UserSessionXref> UserSessionXref { get; set; }
    }
}
