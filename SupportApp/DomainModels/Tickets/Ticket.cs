﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportApp.Areas.Identity.Data;
using SupportApp.Migrations;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
using SupportApp.Models.Images;
using SupportApp.ViewModels.Categories;

namespace SupportApp.Models.Tickets
{
    public class Ticket
    {
        [Key] 
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid AuthorId { get; private set; }
        public string Author { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime Deadline { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public bool IsCompleted { get; private set; }
        public int CategoryId { get; private set; }
        public virtual Category Category { get; private set; }
        protected virtual List<Image> Images { get; private set; } = new List<Image>();

        protected virtual List<Comment> Comments { get; private set; } = new List<Comment>();
        public static IFormFile FormFile { get; set; }

        public IReadOnlyCollection<Comment> GetComments()
        {
            return Comments;
        }

        public IReadOnlyCollection<Image> GetImages()
        {
            return Images;
        }
        public Ticket(string description, string title, Guid authorId, string author, DateTime createdAt, int categoryId)
        {
            CreatedAt = DateTime.Now;
            Deadline = CreatedAt.AddDays(2);
            Description = description;
            Title = title;
            AuthorId = authorId;
            Author = author;
            CategoryId = categoryId;

        }

        public void AddComment(Comment comment)
        {
            Comments.Add(comment);
        }

        public void MarkDone()
        {
            CompletedAt = DateTime.Now;
            IsCompleted = true;
        }

        public void EditTicket(string description, string title, int categoryId)
        {
            Description = description;
            Title = title;
            CategoryId = categoryId;
        }

        public void MarkUndone()
        {
            CompletedAt = null;
            IsCompleted = false;
        }

        public void AddImage(Image image)
        {
            Images.Add(image);
        }
        public static class DbConfiguration
        {
            public static void Configure(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Ticket>().HasMany<Comment>(t => t.Comments)
                    .WithOne(x => x.Ticket)
                    .HasForeignKey(x => x.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
                modelBuilder.Entity<Ticket>().HasMany<Image>(t => t.Images)
                    .WithOne(x => x.Ticket)
                    .HasForeignKey(x => x.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
                modelBuilder.Entity<Ticket>().HasOne<Category>(t => t.Category);
            }
        }
    }
}
