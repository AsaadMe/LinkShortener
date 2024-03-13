namespace LinkShortener.Models
{
    public class Link
    {
        public int ID {  get; set; }
        public string LongUrl { get; set; }
        public string ShortKey { get; set; }
        public DateTime? CreationTime { get; set; }

        public override string ToString()
        {
            return $"ID: {ID}, URL: {LongUrl}, Short Url: {ShortKey}, Creation Time: {CreationTime.ToString()}";
        }
    }
}
