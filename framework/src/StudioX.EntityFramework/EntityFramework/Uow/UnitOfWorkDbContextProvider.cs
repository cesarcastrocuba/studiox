using System.Data.Entity;
using StudioX.Domain.Uow;
using StudioX.MultiTenancy;

namespace StudioX.EntityFramework.Uow
{
    /// <summary>
    ///     Implements <see cref="IDbContextProvider{TDbContext}" /> that gets DbContext from
    ///     active unit of work.
    /// </summary>
    /// <typeparam name="TDbContext">Type of the DbContext</typeparam>
    public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        private readonly ICurrentUnitOfWorkProvider currentUnitOfWorkProvider;

        /// <summary>
        ///     Creates a new <see cref="UnitOfWorkDbContextProvider{TDbContext}" />.
        /// </summary>
        /// <param name="currentUnitOfWorkProvider"></param>
        public UnitOfWorkDbContextProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            this.currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public TDbContext GetDbContext()
        {
            return GetDbContext(null);
        }

        public TDbContext GetDbContext(MultiTenancySides? multiTenancySide)
        {
            return currentUnitOfWorkProvider.Current.GetDbContext<TDbContext>(multiTenancySide);
        }
    }
}