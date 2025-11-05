using System.Collections.Generic;
namespace ECommerce.Models{
    public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    public decimal TotalAmount {  get; set; }
    public List<OrderItem> OrderItems { get; set; }=new List<OrderItem> { };

}
}
