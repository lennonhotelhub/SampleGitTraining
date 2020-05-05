using System.Collections.Generic;
using WebApiWithHangfire.Models;

namespace WebApiWithHangfire.Services
{
    public interface IBookService
    {
        Book Create(Book book);
        List<Book> Get();
        Book Get(string id);
        void Remove(Book bookIn);
        void Remove(string id);
        void Update(string id, Book bookIn);
    }
}