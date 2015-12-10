using Microsoft.AspNet.Identity.EntityFramework;
using SoftuniTwitter.Data.Migrations;
using SoftuniTwitter.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftuniTwitter.Data
{
    public class SoftuniTwitterDbContext:IdentityDbContext<ApplicationUser>
    {
        public SoftuniTwitterDbContext()
            : base("SoftuniTwitter")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SoftuniTwitterDbContext, Configuration>());
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FilePath> FilePaths { get; set; }

        public static SoftuniTwitterDbContext Create()
        {
            return new SoftuniTwitterDbContext();
        }
    }
}