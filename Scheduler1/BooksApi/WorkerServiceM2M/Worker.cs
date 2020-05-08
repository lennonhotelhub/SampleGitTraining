using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using WorkerServiceM2M.DataLayer;
using WorkerServiceM2M.Models;
using Microsoft.Extensions.Options;
//using BookStoreLibrary;
//using BookStoreLibrary.Entities;

namespace WorkerServiceM2M
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient _client;
        private BooksApi.Services.BookService _bookService;
        //private IBookService _bookService;// class library

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //Settings settings = new Settings { ConnectionString = "mongodb://localhost:27017", Database = "Bookstore2Db" };
            //IOptions<Settings> config = settings;
            BooksApi.Models.IBookstoreDatabaseSettings settings = new BooksApi.Models.BookstoreDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                BooksCollectionName = "Books",
                DatabaseName = "Bookstore2Db"
            };
            _bookService = new BooksApi.Services.BookService(settings);
            #region using class library
            //IBookstoreDatabaseSettings settings = new BookstoreDatabaseSettings
            //{
            //    ConnectionString = "mongodb://localhost:27017",
            //    BooksCollectionName = "Books",
            //    DatabaseName = "Bookstore2Db"
            //};
            //_bookService = new BookService(settings);
            //List<Book> books = _bookService.Get();
            #endregion

            _client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                //BooksApi.Models.Book bookDetail = await GetBookAsync("5e8eb599c27c327dc1562b09");//http call throught web api
                //_bookService.Create(bookDetail);
                //List<BooksApi.Models.Book> books = _bookService.Get();

                List<BooksApi.Models.Book> books = await GetAllBooksAsync();
                foreach(var book in books)
                {
                    _bookService.Create(book);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(20*1000, stoppingToken);
            }
        }

        public async Task<BooksApi.Models.Book> GetBookAsync(string id)
        {
            var httpResponse = await _client.GetAsync("https://localhost:5001/api/Books/" + id);
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception("Cannot retrieve tasks");
            }
            var content = await httpResponse.Content.ReadAsStringAsync();
            var bookItem = JsonConvert.DeserializeObject<BooksApi.Models.Book>(content);
            return bookItem;
        }

        public async Task<List<BooksApi.Models.Book>> GetAllBooksAsync()
        {
            var httpResponse = await _client.GetAsync("https://localhost:5001/api/Books/");
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception("Cannot retrieve tasks");
            }
            var content = await httpResponse.Content.ReadAsStringAsync();
            List<BooksApi.Models.Book> bookItems = JsonConvert.DeserializeObject<List<BooksApi.Models.Book>>(content).ToList();
            return bookItems;
        }

        //alternative approach without defining HttpClient in StartAsync
        //public async Task<Book> GetBookAsync(string id)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var response = await httpClient.GetAsync("https://localhost:5001/api/Books/" + id))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            return JsonConvert.DeserializeObject<Book>(apiResponse);
        //        }
        //    }
        //}

    }
}
