using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupportApp.Data
{
        public  interface ISupportAppRepository<T> where T : class
        {
            T GetById(int id);
            IEnumerable<T> Find(Expression<Func<T, bool>> expression);
            void Add(T entity);
            void Remove(T entity);
            void Update(T entity);

        }
}
