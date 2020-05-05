using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WebApiWithHangfire.Models;

namespace WebApiWithHangfire.Services
{
    public class BookService : IBookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["BookstoreDatabaseSettings:ConnectionString"]);//(configuration.GetConnectionString("BookstoreDatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration["BookstoreDatabaseSettings:DatabaseName"]);

            _books = database.GetCollection<Book>(configuration["BookstoreDatabaseSettings:BooksCollectionName"]);
        }

        public List<Book> Get() =>
            _books.Find(book => true).ToList();
   

        public Book Get(string id) =>
            _books.Find<Book>(book => book.Id == id).FirstOrDefault();

        public Book Create(Book book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, Book bookIn) =>
            _books.ReplaceOne(book => book.Id == id, bookIn);

        public void Remove(Book bookIn) =>
            _books.DeleteOne(book => book.Id == bookIn.Id);

        public void Remove(string id) =>
            _books.DeleteOne(book => book.Id == id);
    }
}
