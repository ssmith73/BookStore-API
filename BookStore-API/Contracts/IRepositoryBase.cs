using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Contracts
{

    //Base Repository will be relative to some (data) class T
    public interface IRepositoryBase<T> where T : class
    {
        //Declare an asynchronous (parallel) function with Task
        //Method should return an IList of class type T
        Task<IList<T>> FindAll();
        Task<T> FindById(int id);
        Task<bool> IsExists(int id);
        Task<bool> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);

        //In Entity framework, need to save before a commit to db is made
        Task<bool> Save();


    }
}
