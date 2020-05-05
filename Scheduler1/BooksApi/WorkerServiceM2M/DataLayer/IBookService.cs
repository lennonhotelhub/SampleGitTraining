using BooksApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkerServiceM2M.DataLayer
{
    public interface IBookService
    {
        Task Create(Book book);
        Task<IEnumerable<Book>> Get();
        Task<Book> Get(string id);
        Task<DeleteResult> Remove(Book bookIn);
        Task<DeleteResult> Remove(string id);
        Task<bool> Update(string id, Book bookIn);
    }
}