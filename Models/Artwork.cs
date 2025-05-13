using System;

namespace CreativeCanvas.Models
{
    public class Artwork
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string ImageUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ColorTheme Theme { get; set; } = new ColorTheme();
    }
}