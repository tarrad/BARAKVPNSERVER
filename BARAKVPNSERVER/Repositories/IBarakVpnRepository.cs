using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Repositories
{
    public interface IBarakVpnRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        //Task<float> Update(Hero entity); 
        Task<bool> SaveAll();
        public void Update<T>(T entity) where T : class;
       
    }
}
