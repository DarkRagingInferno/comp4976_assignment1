using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using harry_potter_asn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Authorize]
public class BooksController : Controller {
//   const string BASE_URL = "https://www.googleapis.com/books/v1/volumes?q=harry+potter";
  private readonly ILogger<BooksController> _logger;
  private readonly System.Net.Http.IHttpClientFactory _clientFactory;
  public ArrayList books = new ArrayList();
  public bool GetBooksError { get; private set; }

  public BooksController(ILogger<BooksController> logger, IHttpClientFactory clientFactory)
  {
    _logger = logger;
    _clientFactory = clientFactory;
  }

// HttpClient client = new HttpClient();
// public async Task<string> GetResponseString() {
//     var request = "https://www.googleapis.com/books/v1/volumes?q=harry+potter";
//     var response = await client.GetAsync(request);
//     var content = await response.Content.ReadAsStringAsync();
//     return content;
// }
  public async Task<IActionResult> Index()
  {
    HttpClient client = new HttpClient();
    var request = "https://www.googleapis.com/books/v1/volumes?q=harry+potter";
    var response = await client.GetAsync(request);

    // var content = await response.Content.ReadAsStringAsync();

    // var message = new HttpRequestMessage();
    // message.Method = HttpMethod.Get;
    // message.RequestUri = new Uri($"{BASE_URL}api/Books");
    // message.Headers.Add("Accept", "application/json");

    // var client = _clientFactory.CreateClient();

    // var response = await client.SendAsync(message);

    // Console.WriteLine(response);

    if (response.IsSuccessStatusCode) 
    {
        var content = await response.Content.ReadAsStringAsync();
        // Console.WriteLine(content);

        dynamic myList = JObject.Parse(content);
        var items = myList.items;
        foreach(var item in items)
        {
           Books newBook = new Books();
           
            newBook.title = item.volumeInfo.title;
            newBook.smallThumbnail = item.volumeInfo.imageLinks.smallThumbnail;
            newBook.authors = item.volumeInfo.authors.ToString();
            newBook.publishers = item.volumeInfo.publishers;
            newBook.publishedDate = item.volumeInfo.publisedDate;
            newBook.description = item.volumeInfo.description;
            newBook.ISBN_10 = item.volumeInfo.industryIdentifiers[1].indentifier;

            books.Add(newBook);
        }
    } 
    else 
    {
        GetBooksError = true;
        // books.Clear();
    }
    ViewBag.Books = books;
    return View(books);
  }

  public async Task<IActionResult> Details(string title, string smallThumbnail, string authors, string publishers, 
                                            string publishedDate,  string description, string ISBN_10) 
{   
    Books pseudoBook = new Books();
    pseudoBook.title = title;
    pseudoBook.smallThumbnail = smallThumbnail;
    pseudoBook.authors = authors;
    pseudoBook.publishers = publishers;
    pseudoBook.publishedDate = publishedDate;
    pseudoBook.description = description;
    pseudoBook.ISBN_10 = ISBN_10;

    ViewBag.Pseudo = pseudoBook;
    return View(books);
  }
}
