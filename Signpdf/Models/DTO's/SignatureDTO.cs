﻿namespace Signpdf.Models.DTO_s
{
    public class SignatureDTO
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public int Page { get; set; }
        public string? ImageData { get; set; }
    }
}
