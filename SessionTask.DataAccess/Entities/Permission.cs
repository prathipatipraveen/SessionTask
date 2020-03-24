using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class Permission
    {
        public Permission()
        {
            FeatureRolePermissionXref = new HashSet<FeatureRolePermissionXref>();
            FeatureUserPermissionXref = new HashSet<FeatureUserPermissionXref>();
        }

        public int PermissionId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FeatureRolePermissionXref> FeatureRolePermissionXref { get; set; }
        public virtual ICollection<FeatureUserPermissionXref> FeatureUserPermissionXref { get; set; }
    }
}
