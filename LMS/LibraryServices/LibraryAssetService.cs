using LibraryData;
using LibraryData.Interfaces;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _libraryContext;
        public LibraryAssetService(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public void Add(LibraryAsset newAsset)
        {
            _libraryContext.LibraryAssets.Add(newAsset);
            _libraryContext.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _libraryContext.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);
        }

        public LibraryAsset GetById(int id)
        {
            return 
                GetAll()
                .FirstOrDefault(asset => asset.Id == id);   //FirstOrDefault() gives the default value i.e. null  in this case if no condition matches.
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
            //return _libraryContext.LibraryAssets.FirstOrDefault(asset => asset.Id == id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_libraryContext.Books.Any(book => book.Id == id))
            {
                return _libraryContext.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }

            //Can also use "OfType<>()" method to check if child class Book exists with this id. Like this below:
            //var isBook = _libraryContext.LibraryAssets.OfType<Book>().Where(a => a.Id == id).Any();

            else return "";
        }

        public string GetIsbn(int id)
        {
            if (_libraryContext.Books.Any(book => book.Id == id))
            {
                return _libraryContext.Books.FirstOrDefault(book => book.Id == id).ISBN;
            }

            //Can also use "OfType<>()" method to check if child class Book exists with this id. Like this below:
            //var isBook = _libraryContext.LibraryAssets.OfType<Book>().Where(a => a.Id == id).Any();

            else return "";
        }

        public string GetTitle(int id)
        {
            return
                GetAll()
                .FirstOrDefault(asset => asset.Id == id)
                .Title;
        }

        public string GetType(int id)
        {
            var book = _libraryContext.LibraryAssets.OfType<Book>().Where(a => a.Id == id);

            return book.Any() ? "Book" : "Video";
        }
        public string GetAuthorOrDirector(int id)
        {
            var isBook = _libraryContext.LibraryAssets.OfType<Book>().Where(a => a.Id == id).Any();

            var isVideo = _libraryContext.LibraryAssets.OfType<Video>().Where(a => a.Id == id).Any();

            return isBook ? 
                _libraryContext.Books.FirstOrDefault(book => book.Id == id).Author : 
                _libraryContext.Videos.FirstOrDefault(video => video.Id == id).Director
                ?? "Unknown";
        }
    }
}
