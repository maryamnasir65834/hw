﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using BookStoreApi.Models;

namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private static List<Book> Books = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", Author = "Author 1", Description = "Description 1", ImageUrl = "https://m.media-amazon.com/images/I/81q77Q39nEL._AC_UF1000,1000_QL80_.jpg", Price = 10.99M, Availability = "In Stock" },
            new Book { Id = 2, Title = "Book 2", Author = "Author 2", Description = "Description 2", ImageUrl = "https://marketplace.canva.com/EAFaQMYuZbo/1/0/1003w/canva-brown-rusty-mystery-novel-book-cover-hG1QhA7BiBU.jpg", Price = 12.99M, Availability = "Out of Stock" },
            new Book { Id = 3, Title = "Book 3", Author = "Author 3", Description = "Description 3", ImageUrl = "https://images-us.bookshop.org/ingram/9781250822055.jpg?height=500&v=v2-775a9b07940f44786ff5ec071ff42da3.jpeg", Price = 15.99M, Availability = "In Stock" },
            new Book { Id = 4, Title = "Book 4", Author = "Author 4", Description = "Description 4", ImageUrl = "https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEgwUFEsq8iUmF26DsoKqCqceFO44Py_5-0rwS1LT5ep4flHnas3-8JULNhAWzt9qfYO1x38tKPVBc4yeG8NLC9D6xJvLj3qtKXLnn6QTi5kAO-0uLOasBLpaXZw22z4Jo3qxLSPl0vZtbRE_35X1RduZBRIpQYRWBbdTNeuKhaZlFSJiLfm7CDKWvY5ZQ/s2362/WHEN%20THE%20SMOKE%20CLEARED%20BOOK%20COVER.jpg", Price = 12.99M, Availability = "Out of Stock" },
            new Book { Id = 5, Title = "Book 5", Author = "Author 5", Description = "Description 5", ImageUrl = "https://www.editorialdepartment.com/wp-content/uploads/2015/04/Book-Title.jpg", Price = 15.99M, Availability = "In Stock" },
            new Book { Id = 6, Title = "Book 6", Author = "Author 6", Description = "Description 6", ImageUrl = "https://skullsinthestars.com/wp-content/uploads/2021/10/howcanhekeepstandinglikethat.jpg", Price = 12.99M, Availability = "Out of Stock" },
        };

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks([FromQuery] string title = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var filteredBooks = Books
                .Where(b => string.IsNullOrEmpty(title) || b.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            bool hasMore = filteredBooks.Count >= pageSize;
            return Ok(new { books = filteredBooks, hasMore });
        }

        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(int id)
        {
            var book = Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public ActionResult<Book> CreateBook([FromBody] Book book)
        {
            if (book == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid book data.");
            }

            book.Id = Books.Max(b => b.Id) + 1;
            Books.Add(book);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book book)
        {
            var existingBook = Books.FirstOrDefault(b => b.Id == id);
            if (existingBook == null)
                return NotFound();

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Description = book.Description;
            existingBook.ImageUrl = book.ImageUrl;
            existingBook.Price = book.Price;
            existingBook.Availability = book.Availability;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();

            Books.Remove(book);
            return NoContent();
        }

        [HttpGet("author/{author}")]
        public ActionResult<IEnumerable<Book>> GetBooksByAuthor(string author)
        {
            var books = Books.Where(b => b.Author.Equals(author, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(books);
        }

        // CONTACT FORM

        [HttpPost("contact-us")]
        public IActionResult ContactUs([FromBody] ContactFormModel contactForm)
        {
            if (contactForm == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid contact form data.");
            }

            // Process the contact form data (e.g., save to database, send email, etc.)
            // For demonstration, we'll just log the data to console
            Console.WriteLine($"Received Contact Form Submission:");
            Console.WriteLine($"Name: {contactForm.Name}");
            Console.WriteLine($"Email: {contactForm.Email}");
            Console.WriteLine($"Subject: {contactForm.Subject}");
            Console.WriteLine($"Message: {contactForm.Message}");

            // Assuming you have a method to save the contact form to a database or similar
            // For demonstration, we'll add it to a list of reviews (not persisted)
            AddCustomerReview(contactForm);

            // Optional: Return a success message or status code
            return Ok("Contact form submitted successfully.");
        }

        // Method to retrieve all customer reviews (for demonstration)
        [HttpGet("customer-reviews")]
        public ActionResult<IEnumerable<ContactFormModel>> GetCustomerReviews()
        {
            // Return all contact form submissions as customer reviews
            return Ok(CustomerReviews);
        }

        // In-memory storage for customer reviews (for demonstration)
        private static List<ContactFormModel> CustomerReviews = new List<ContactFormModel>();

        // Method to add a new customer review (for demonstration)
        private void AddCustomerReview(ContactFormModel contactForm)
        {
            CustomerReviews.Add(contactForm);
        }



        // New endpoint for featured books
        [HttpGet("featured")]
        public ActionResult<IEnumerable<Book>> GetFeaturedBooks()
        {
            // Replace this with logic to get actual featured books, e.g., from a database
            var featuredBooks = Books
                .Where(b => b.Price >= 10)  // Example criteria for featured books
                .ToList();

            return Ok(featuredBooks);
        }
    }
}
