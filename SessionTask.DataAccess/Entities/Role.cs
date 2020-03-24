using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class Role
    {
        public Role()
        {
            FeatureRolePermissionXref = new HashSet<FeatureRolePermissionXref>();
            UserRoleXref = new HashSet<UserRoleXref>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<FeatureRolePermissionXref> FeatureRolePermissionXref { get; set; }
        public virtual ICollection<UserRoleXref> UserRoleXref { get; set; }
    }
}
