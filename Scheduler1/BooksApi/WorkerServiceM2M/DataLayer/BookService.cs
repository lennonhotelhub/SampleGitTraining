using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WorkerServiceM2M.Models;


namespace WorkerServiceM2M.DataLayer
{
    public class BookService : IBookService
    {
        //private readonly IMongoCollection<Book> _books;
        private readonly MongoRepository _repository = null;

        public BookService(IOptions<Settings> settings)
        {
            _repository = new MongoRepository(settings);
            //var client = new MongoClient(settings.ConnectionString);
            //var database = client.GetDatabase(settings.DatabaseName);

            //_books = database.GetCollection<Book>(settings.BooksCollectionName);
        }

        public async Task<IEnumerable<Book>> Get()
        {
            return await _repository.books.Find(book => true).ToListAsync();
        }

        public async Task<Book> Get(string id)
        {
            return await _repository.books.Find<Book>(book => book.Id == id).FirstOrDefaultAsync();
        }
        public async Task Create(Book book)
        {
            await _repository.books.InsertOneAsync(book);
        }

        public async Task<bool> Update(string id, Book bookIn)
        {
            await _repository.books.ReplaceOneAsync(book => book.Id == id, bookIn);
            return true;
        }

        public async Task<DeleteResult> Remove(Book bookIn)
        {
            return await _repository.books.DeleteOneAsync(book => book.Id == bookIn.Id);
        }

        public async Task<DeleteResult> Remove(string id)
        {
            return await _repository.books.DeleteOneAsync(book => book.Id == id);
        }
    }
}
