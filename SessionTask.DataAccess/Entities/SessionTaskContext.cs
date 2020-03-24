using System;
using Microsoft.EntityFrameworkCore;

namespace SessionTask.DataAccess.Entities
{
    public partial class SessionTaskContext : DbContext
    {
        public SessionTaskContext()
        {
        }

        public SessionTaskContext(DbContextOptions<SessionTaskContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Feature> Feature { get; set; }
        public virtual DbSet<FeatureRolePermissionXref> FeatureRolePermissionXref { get; set; }
        public virtual DbSet<FeatureUserPermissionXref> FeatureUserPermissionXref { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRoleXref> UserRoleXref { get; set; }
        public virtual DbSet<UserSessionXref> UserSessionXref { get; set; }

    }
}
