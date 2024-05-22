namespace Signpdf.Models
{
    public class Ocr
    {
        public int Id { get; set; }
        public string? OcrText { get; set; }
        public DateTime Created_at { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Page {  get; set; }

        public string? PdfFilePath { get; set; }

    }
}
