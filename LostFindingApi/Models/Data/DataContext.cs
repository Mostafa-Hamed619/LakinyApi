using LostFindingApi.Models.Real_Time;
using LostFindingApi.Services.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LostFindingApi.Models.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        private readonly IhttpContextAccessor ihttpContext;

        public DataContext(DbContextOptions<DataContext> options, IhttpContextAccessor IhttpContext) : base(options)
        {
            ihttpContext = IhttpContext;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUserLogin<string>>().HasNoKey();
            builder.Entity<IdentityUserRole<string>>().HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Entity<IdentityUserToken<string>>()
                        .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });


            builder.Entity<Item>()
                .HasOne<Category>()
                .WithMany()
                .HasForeignKey(c => c.CategoryId);

            builder.Entity<Item>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(i => i.userId);

            builder.Entity<Card>()
                .HasKey(c => c.id);
        }

        public DbSet<Item> Item { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Chat> chats { get; set; }
        public DbSet<Card> cards { get; set; }
        public DbSet<Liscence> liscences { get; set; }
        public DbSet<AuditLogs> auditLogs { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var ModificationEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)
                .ToList();

            foreach(var entity in ModificationEntities)
            {
                var audit = new AuditLogs()
                {
                    Action = entity.State.ToString(),
                    EntityType = entity.Entity.GetType().Name,
                    TimeStamp = DateTime.Now,
                    Changes = GetUpdate(entity),
                    User = ihttpContext.getContext().HttpContext.User.FindFirst(ClaimTypes.GivenName)?.Value
                };
                audit.Id = new Guid();

                auditLogs.Add(audit);
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private static string GetUpdate(EntityEntry entry)
        {
            var sb = new StringBuilder();

            foreach(var prop in entry.OriginalValues.Properties)
            {
                var OriginalValue = entry.OriginalValues[prop];
                var CurrentValue = entry.CurrentValues[prop];

                if(!Equals(OriginalValue, CurrentValue))
                {
                    sb.Append($"{prop.Name}: From {OriginalValue} To {CurrentValue}");
                }
            }

            return sb.ToString();
        }
    }
}
