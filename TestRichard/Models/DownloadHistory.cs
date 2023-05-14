using TestRichard.Enums;

namespace TestRichard.Models
{
    public class DownloadHistory
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public DateTime DownloadedDate { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
    }
}
