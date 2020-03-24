using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class User
    {
        public User()
        {
            FeatureUserPermissionXref = new HashSet<FeatureUserPermissionXref>();
            UserRoleXref = new HashSet<UserRoleXref>();
            UserSessionXref = new HashSet<UserSessionXref>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<FeatureUserPermissionXref> FeatureUserPermissionXref { get; set; }
        public virtual ICollection<UserRoleXref> UserRoleXref { get; set; }
        public virtual ICollection<UserSessionXref> UserSessionXref { get; set; }
    }
}
