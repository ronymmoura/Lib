#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
#endregion

namespace Lib.Data.Repository
{
    public class BaseRepository<TEntity, TContext> : IDisposable
        where TEntity : class
        where TContext : DbContext, new()
    {
        #region Properties

        protected TContext Context;
        protected bool IsDisposed;

        #endregion Fields

        #region Constructors

        public BaseRepository()
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{envName}.json", true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            Context = (TContext)Activator.CreateInstance(typeof(TContext), new object[] { optionsBuilder.Options });
            Context.Database.EnsureCreated();
        }

        #endregion Constructors

        #region Public Methods

        #region Find

        public virtual TEntity Find(int id)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var expression = (Expression<Func<TEntity, bool>>)
                Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, "ID"),
                        Expression.Constant(id)),
                    parameter);
            return Context.Set<TEntity>().Where(expression).SingleOrDefault();
        }

        #endregion Find

        #region List

        public virtual List<TEntity> List()
        {
            return Context.Set<TEntity>().ToList();
        }

        public virtual List<TEntity> List(int page, int pageSize)
        {
            var results = List()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return results;
        }

        #endregion List

        #region List Descending

        public List<TEntity> ListDescending<TKey>(Func<TEntity, TKey> keySelector)
        {
            return Context.Set<TEntity>()
                .OrderByDescending(keySelector)
                .ToList();
        }

        public List<TEntity> ListDescending<TKey>(int page, int pageSize, Func<TEntity, TKey> keySelector)
        {
            var results = ListDescending(keySelector)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return results;
        }

        #endregion

        #region Limit

        public List<TEntity> Limit(int size)
        {
            var model = List()
                .Take(size)
                .ToList();
            return model;
        }

        #endregion Limit

        #region Count

        public int Count()
        {
            return Context.Set<TEntity>().Count();
        }

        #endregion Count

        #region Add

        public virtual TEntity Add(TEntity item)
        {
            return Context.Set<TEntity>().Add(item).Entity;
        }

        #endregion Add

        #region Remove

        public virtual void Remove(TEntity item)
        {
            Context.Set<TEntity>().Remove(item);
        }

        public virtual void RemoveById(int id)
        {
            var item = Find(id);

            if (item is int)
                Remove(item);
        }

        #endregion Remove

        #region Edit

        public virtual void Edit(TEntity item)
        {
            Context.Entry(item).State = EntityState.Modified;
        }

        #endregion Edit

        #region Save

        public void Save()
        {
            Context.SaveChanges();
        }

        #endregion Save

        #endregion Public Methods

        #region IDisposable implementations

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                    Context = null;
                }
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable implementations
    }
}
