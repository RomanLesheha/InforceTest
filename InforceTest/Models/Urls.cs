namespace InforceTest.Models
{
    public class Urls
    {
        public int Id { get; set; }
        public  string LongURL { get; set; }
        public  string ShortURL { get; set; }
        public DateTime Created { get; set; }
        public  string CreatedBy { get; set; }
    }
}
