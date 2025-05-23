﻿using System.Linq.Expressions;

namespace AmazingBooks_API.Configuration.Repository
{
    public interface ICommonRepository<T> where T : class 
    {
        public Task<IEnumerable<T>> GetRecords();
        public Task<IEnumerable<T>> GetRecordsByFilter(Expression<Func<T, bool>> filter);
        public Task<IEnumerable<T>> GetRecordsByFilter(Expression<Func<T, bool>> filter, Expression<Func<T, int>> sortKey);
        public Task<T> GetRecord(Expression<Func<T, bool>> filter);
        public Task<T> UpdateRecord(T record);
        public Task<T> CreateRecord(T record);
        public Task<bool> DeleteRecord(T record);
       
    }
}
