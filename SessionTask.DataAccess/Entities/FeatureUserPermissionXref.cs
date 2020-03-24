using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class FeatureUserPermissionXref
    {
        public int FeatureUserPermissionXrefId { get; set; }
        public int FeatureId { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual Permission Permission { get; set; }
        public virtual User User { get; set; }
    }
}
