using System;
using System.Collections.Generic;

namespace SessionTask.DataAccess.Entities
{
    public partial class FeatureUserXref
    {
        public int FeatureUserXrefId { get; set; }
        public int FeatureId { get; set; }
        public int UserId { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual User User { get; set; }
    }
}
