using System.ComponentModel.DataAnnotations;

namespace InforceTest.Models
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(30)]
        public  string FullName { get; set; }
        public  string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
