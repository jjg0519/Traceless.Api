using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Traceless.AuthWithASPIdentity
{
    public partial class authSqlContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=yourserver;Database=yourdb;User Id=yourid;Password=yourpwd");
                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {}
    }
}
