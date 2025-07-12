
using System.Linq.Expressions;
using EntityFramework.Exceptions.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductosMs.Infrastructure.Database;
using ProductsMs.Core.Database;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Primitives;


namespace ProductsMS.Infrastructure.Database.Context.Postgres
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
    {
        //*Nos ayudara con los eventos de dominio
        private readonly IPublisher _publisher;
        public ApplicationDbContext(
          DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
           // _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }


        public DbContext DbContext
        {
            get { return this; }
        }

       public virtual DbSet<ProductEntity> Products { get; set; } = null!;
       public virtual DbSet<CategoryEntity> Categories { get; set; } = null!;



        public IDbContextTransactionProxy BeginTransaction()
        {
            return new DbContextTransactionProxy(this);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            //TODO: En teoria hacen lo mismo
            //* Esto hara que no agarre la entidad como tal sino la adecuada para la BD
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseExceptionProcessor();
        }

        public virtual void SetPropertyIsModifiedToFalse<TEntity, TProperty>(
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertyExpression
        )
            where TEntity : class
        {
            Entry(entity).Property(propertyExpression).IsModified = false;
        }

        public virtual void ChangeEntityState<TEntity>(TEntity entity, EntityState state)
        {
            if (entity != null)
            {
                Entry(entity).State = state;
            }
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is AggregateRoot
                    && (e.State == EntityState.Added || e.State == EntityState.Modified)
                );

            foreach (var entityEntry in entries)
            {
                var entity = (AggregateRoot)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = DateTime.UtcNow;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;

                    // Marcar explícitamente que `UpdatedAt` está modificado
                    Entry(entity).Property(x => x.UpdatedAt).IsModified = true;

                    // Evitar cambios accidentales en `CreatedAt` y `CreatedBy`
                    Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                    Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(
            string user,
            CancellationToken cancellationToken = default
        )
        {
            var state = new List<EntityState> { EntityState.Added, EntityState.Modified };

            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is AggregateRoot && state.Any(s => e.State == s));

            var dt = DateTime.UtcNow;

            foreach (var entityEntry in entries)
            {
                var entity = (AggregateRoot)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = dt;
                    entity.CreatedBy = user;
                    Entry(entity).Property(x => x.UpdatedAt).IsModified = false;
                    Entry(entity).Property(x => x.UpdatedBy).IsModified = false;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = dt;
                    entity.UpdatedBy = user;
                    Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                    Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> SaveEfContextChanges(CancellationToken cancellationToken = default)
        {
            return await SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> SaveEfContextChanges(
            string user,
            CancellationToken cancellationToken = default
        )
        {
            return await SaveChangesAsync(user, cancellationToken) > 0;
        }
    }
}