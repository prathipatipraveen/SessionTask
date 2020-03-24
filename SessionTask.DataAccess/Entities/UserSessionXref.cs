using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class UserSessionXref
    {
        public int UserSessionXrefId { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public bool IsApproved { get; set; }

        public virtual Session Session { get; set; }
        public virtual User User { get; set; }
    }
}
