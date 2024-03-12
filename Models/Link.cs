namespace LinkShortener.Models
{
    public class Link
    {
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime? CreationTime { get; set; }

        public override string ToString()
        {
            return $"URL: {LongUrl}, Short Url: {ShortUrl}, Creation Time: {CreationTime.ToString()}";
        }
    }
}
