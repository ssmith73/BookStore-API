using AutoMapper;
using BookStore_API.Data;
using BookStore_API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Mappings
{
    /// <summary>
    /// Let automapper understand the mappings b/n classes and DTO
    /// DTO is object from user, Only matching properties are matched
    /// so if user makes spelling error in name it's ignored
    /// In the field of programming a data transfer object (DTO ) 
    /// is an object that carries data between processes. 
    /// </summary>
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Author, AuthorCreateDTO>().ReverseMap();
            CreateMap<Author, AuthorUpdateDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, BookCreateDTO>().ReverseMap();
            CreateMap<Book, BookUpdateDTO>().ReverseMap();
        }
    }
}
