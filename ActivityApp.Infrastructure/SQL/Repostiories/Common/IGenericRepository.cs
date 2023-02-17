using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Infrastructure.SQL.Repostiories.Common
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(object clientId);
        T GetById(object id, object clientId);
        T GetByName(object name, object clientId);
        void Delete(object id, object clientId);
        void Save();
        T Insert(T entity);
        T Update(T entity);
    }
}
