using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class FeatureRoleXref
    {
        public int FeatureRoleXrefId { get; set; }
        public int FeatureId { get; set; }
        public int RoleId { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual Role Role { get; set; }
    }
}
