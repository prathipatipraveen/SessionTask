using System.Collections.Generic;

namespace SessionTask.Models
{
    public class ApproveAttendeesDto
    {
        public List<int> UserIds { get; set; }
        public int SessionId { get; set; }
    }
}
