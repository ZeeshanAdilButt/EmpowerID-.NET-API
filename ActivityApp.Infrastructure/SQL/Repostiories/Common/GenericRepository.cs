using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ActivityApp.Infrastructure.SQL.Repostiories.Common
{
    public class GenericRepository<T> : IDisposable, IGenericRepository<T> where T : class
    {
        private ApplicationDbContext _context = null;
        private DbSet<T> table = null;
        public GenericRepository(ApplicationDbContext applicationContext)
        {
            this._context = applicationContext;
            table = _context.Set<T>();
        }

        /// <summary>
        /// Delete entity by Id and Client Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        public void Delete(object id, object clientId)
        {
            T existing = table.Find(id);
            var ClientId = GetPropValue(existing, "ClientId");
            if (Convert.ToInt32(ClientId) != Convert.ToInt32(clientId))
            {
                throw new Exception($"You are not permitted to delete {existing.GetType()} for this client");
            }
            table.Remove(existing);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get all entities by client Id.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public IEnumerable<T> GetAll(object clientId)
        {
            List<Filter> filter = new List<Filter>()
            {
                new Filter { PropertyName = "ClientId" ,
                    Operation = Op .Equals, Value = clientId  }
            };
            var deleg = ExpressionBuilder.GetExpression<T>(filter).Compile();
            var filteredCollection = table.Where(deleg);
            return filteredCollection;
        }

        /// <summary>
        /// Get entity by Id and client Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public T GetById(object id, object clientId)
        {
            List<Filter> filter = new List<Filter>()
            {
                new Filter { PropertyName = "Id" ,
                    Operation = Op .Equals, Value = id  },
                 new Filter { PropertyName = "ClientId" ,
                    Operation = Op .Equals, Value = clientId  }
            };
            var deleg = ExpressionBuilder.GetExpression<T>(filter).Compile();
            var filteredResult = table.FirstOrDefault(deleg);
            return filteredResult;
        }

        /// <summary>
        /// Get entity by name and client id.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public T GetByName(object name, object clientId)
        {
            List<Filter> filter = new List<Filter>()
            {
                new Filter { PropertyName = "Name" ,
                    Operation = Op .Equals, Value = name  },
                 new Filter { PropertyName = "ClientId" ,
                    Operation = Op .Equals, Value = clientId  }
            };
            var deleg = ExpressionBuilder.GetExpression<T>(filter).Compile();
            var filteredResult = table.FirstOrDefault(deleg);
            return filteredResult;
        }

        /// <summary>
        /// Add new entity to database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Insert(T entity)
        {
            table.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Save Context changes.
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Update the entity in database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Update(T entity)
        {
            table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return entity;
        }

        public virtual void InsertMultiple(IEnumerable<T> entities)
        {
            _context.AddRange(entities);
            Save();
        }

        /// <summary>
        /// Get property value from object by property name.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        public void Dispose()
        {

        }
    }
}
