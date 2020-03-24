using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class FeatureRolePermissionXref
    {
        public int FeatureRolePermissionXrefId { get; set; }
        public int FeatureId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual Permission Permission { get; set; }
        public virtual Role Role { get; set; }
    }
}
