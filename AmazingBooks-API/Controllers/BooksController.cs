﻿using Microsoft.AspNetCore.Mvc;
using AmazingBooks_API.Entities;
using AutoMapper;
using AmazingBooks_API.Configuration.Repository;
using AmazingBooks_API.Configuration.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace AmazingBooks_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommonRepository<Book> _repository;

        public BooksController(ICommonRepository<Book> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookListDto>>> GetBooks(int id = 0, string name="", string author="" )
        {
            name = name.ToLower();
            author = author.ToLower();
            List<BookListDto> bookDtos = _repository
                .GetRecordsByFilter(data => (data.Name.ToLower().Contains(name) || name=="") && (data.Author.ToLower().Contains(author) || author == "") && data.Id >= id, data => data.Id)
                .Result.Select(record =>
                new BookListDto()
                {
                    Id = record.Id,
                    Name = record.Name,
                    ImgUrl = record.ImgUrl,
                    Price = record.Price,
                    Author = record.Author,
                    Quantity = record.Quantity
                }

            ).ToList();
           

            return Ok(bookDtos);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            Book book = _repository.GetRecord( book => book.Id == id ).Result;

            if (book == null)
            {
                return NotFound("Book not found");
            }
            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutBook(BookDto bookDto)
        {
            if (bookDto == null || bookDto.Id == 0)
            {
                return BadRequest("Input missing");
            }

            Book book = _repository.GetRecord(book=> book.Id == bookDto.Id).Result;

            if (book == null)
            {
                return NotFound("Data not found");
            }

           book = _mapper.Map<Book>(bookDto);
            await _repository.UpdateRecord(book);           

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook(BookDto bookDto)
        {
            Book book = _mapper.Map<Book>(bookDto);
            book = _repository.CreateRecord(book).Result;
            bookDto.Id = book.Id;

            return CreatedAtAction("GetBook", new { id = book.Id }, bookDto);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            Book book = _repository.GetRecord(book => book.Id == id).Result;
            if (book == null)
            {
                return NotFound();
            }

            var result = _repository.DeleteRecord(book).Result;

            return NoContent();
        }      
    }
}
