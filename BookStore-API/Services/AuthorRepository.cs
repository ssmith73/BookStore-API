using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Services
{
    /// <summary>
    /// Setup baseline CRUD operations for Author table
    /// </summary>
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _db;

        public AuthorRepository(ApplicationDbContext db)
        {
            //dependency injection - declare (above) and initialise in the ctor
            _db = db;
        }

        //Create a new author record
        public async Task<bool> Create(Author entity)
        {
            await _db.Authors.AddAsync(entity);
            return await Save();

        }

        public async Task<bool> Delete(Author entity)
        {
            _db.Authors.Remove(entity);
            return await Save();
        }

        //Returns a list of all Authors that are in the db
        //Author is the class type
        //Authors is the collection of type Author
        public async Task<IList<Author>> FindAll()
        {
            var authors = await _db.Authors.ToListAsync();
            return authors;
        }

        //Search Author table for id - and return as type Author
        public async Task<Author> FindById(int id)
        {
            var author =  await _db.Authors.FindAsync(id);
            return author;
        }

        public async Task<bool> IsExists(int id)
        {
            return await _db.Authors.AnyAsync(x => x.Id == id);
        }

        //Async method - so declare it so
        //Returns a bool
        public async Task<bool> Save()
        {
            // SaveChangesAsync is an asynchronous method
            // So need the await
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Author entity)
        {
            _db.Authors.Update(entity);
            return await Save();
        }
    }
}
