using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class Feature
    {
        public Feature()
        {
            FeatureRolePermissionXref = new HashSet<FeatureRolePermissionXref>();
            FeatureUserPermissionXref = new HashSet<FeatureUserPermissionXref>();
        }

        public int FeatureId { get; set; }
        public string FeatureName { get; set; }

        public virtual ICollection<FeatureRolePermissionXref> FeatureRolePermissionXref { get; set; }
        public virtual ICollection<FeatureUserPermissionXref> FeatureUserPermissionXref { get; set; }
    }
}
