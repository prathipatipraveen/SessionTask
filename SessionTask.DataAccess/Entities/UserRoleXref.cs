﻿using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class UserRoleXref
    {
        public int UserRoleXrefId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
