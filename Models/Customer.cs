using System.ComponentModel.DataAnnotations;
namespace ECommerce.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
    }
}
