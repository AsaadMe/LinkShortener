namespace LinkShortener.Services
{
    public class LinkShorteningService
    {
        Random ran = new Random();
        public string GenerateShortLink(int length)
        {
            const string chars = "qwertyupkjhgfdsazxcvbnm23456789";
            var shortlink = "";

            shortlink = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[ran.Next(s.Length)])
                .ToArray());
            

            return shortlink;
        }
    }
}
