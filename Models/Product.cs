using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace ECommerce.Models
{
    public class Product
{
    public int ProductId { get; set; }
    [MaxLength(50)]
    
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public required string Category { get; set; }
    
}
}
