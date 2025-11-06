using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
class Program
{
    static async Task Main(string[] args)
    {
        using var context = new AppDbContext();
        
        while (true)
        {
            Console.WriteLine("\n--- E-Commerce Menu ---");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Add Customer");
            Console.WriteLine("3. Create Order");
            Console.WriteLine("4. View Products");
            Console.WriteLine("5. View Customers");
            Console.WriteLine("6. View Orders");
            Console.WriteLine("7. Filter Products");
            Console.WriteLine("8. Orders Placed by Customers");
            Console.WriteLine("9. Calculate total sales by product category");
            Console.WriteLine("10. Find the top-selling products");
            Console.WriteLine("11. Exit");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddProductAsync(context);
                    break;
                case "2":
                    await AddCustomerAsync(context);
                    break;
                case "3":
                    await CreateOrderAsync(context);
                    break;
                case "4":
                    await ViewProductsAsync(context);
                    break;
                case "5":
                    await ViewCustomersAsync(context);
                    break;
                case "6":
                    await ViewOrdersAsync(context);
                    break;
                case "7":
                    await FilterProductsAsync(context);
                    break;
                case "8":
                    await OrdersByCustomertsAsync(context);
                    break;
                case "9":
                    TotalSalesByCategory(context);
                    break;
                case "10":
                    TopProducts(context);
                    break;
                case "11":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }
    static async Task AddProductAsync(AppDbContext context)
    {
        Console.Write("Enter Product Name: ");
        var name = Console.ReadLine();
        Console.Write("Enter Price: ");
        var price = decimal.Parse(Console.ReadLine() ?? "0");
        Console.Write("Enter Category: ");
        var category = Console.ReadLine();

        var product = new Product { Name = name!, Price = price, Category = category! };
        await context.Products.AddAsync(product);
        Console.WriteLine($"Entity State:{context.Entry(product).State}");
        await context.SaveChangesAsync();
        Console.WriteLine("Product added successfully!");
    }
    static async Task AddCustomerAsync(AppDbContext context)
    {
        Console.WriteLine("Enter Customer Name");
        var name = Console.ReadLine();
        Console.WriteLine("Enter Email:");
        var email = Console.ReadLine()??string.Empty;
        var customer = new Customer { Name = name!, Email = email };
        await context.Customers.AddAsync(customer);
        Console.WriteLine($"Entity State:{context.Entry(customer).State}");
        await context.SaveChangesAsync();
        Console.WriteLine("Customer Added Successfully");
    }
static async Task CreateOrderAsync(AppDbContext context)
{
    Console.Write("Enter Customer ID: ");
    int customerId = int.Parse(Console.ReadLine() ?? "0");

    var customer = await context.Customers.FindAsync(customerId);
    if (customer == null)
    {
        Console.WriteLine("Customer not found!");
        return;
    }

    var order = new Order { CustomerId = customerId, TotalAmount = 0 };

    bool addMore = true;
    while (addMore)
    {
        Console.Write("Enter Product ID: ");
        int productId = int.Parse(Console.ReadLine() ?? "0");
        var product = await context.Products.FindAsync(productId);
        if (product == null)
        {
            Console.WriteLine("Product not found!");
            continue;
        }

        Console.Write("Enter Quantity: ");
        int quantity = int.Parse(Console.ReadLine() ?? "1");

        var orderItem = new OrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            Order = order // Attach order object
        };

        order.OrderItems.Add(orderItem);
        order.TotalAmount += product.Price * quantity;

        Console.Write("Add another item? (y/n): ");
        addMore = Console.ReadLine()?.ToLower() == "y";
    }

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    Console.WriteLine($"Order created successfully! Total Amount: {order.TotalAmount}");
}
    static async Task ViewProductsAsync(AppDbContext context)
    {
        var products = await context.Products.ToListAsync();
        Console.WriteLine("------Products-----");
        foreach (var p in products)
        {
            Console.WriteLine($"ID: {p.ProductId}, Name: {p.Name}, Price: {p.Price}, Category: {p.Category}");
        }

    }
    static async Task ViewCustomersAsync(AppDbContext context)
    {
        var customers = await context.Customers.ToListAsync();
        Console.WriteLine("------Products-----");
        foreach (var c in customers)
        {
            Console.WriteLine($"ID: {c.CustomerId}, Name: {c.Name}, Email: {c.Email}");
        }
    }
    static async Task ViewOrdersAsync(AppDbContext context)
    {
        var orders = await context.Orders.Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(o => o.Product).ToListAsync();
        Console.WriteLine("\n--- Orders ---");
        foreach (var o in orders)
        {
            Console.WriteLine($"OrderID:{o.OrderId},Customer:{o.Customer?.Name},TotalAmount:{o.TotalAmount}");
            foreach (var item in o.OrderItems)
            {
                Console.WriteLine($"Product:{item.Product?.Name} - Quantity:{item.Quantity}");
            }
        }
    }
    static async Task FilterProductsAsync(AppDbContext context)
    {
        Console.Write("Enter min price: ");
        decimal minPrice = decimal.Parse(Console.ReadLine() ?? "0");
        Console.Write("Enter max price: ");
        decimal maxPrice = decimal.Parse(Console.ReadLine() ?? "999999");
        Console.Write("Enter category (optional): ");
        string? category = Console.ReadLine();
        var Param = Expression.Parameter(typeof(Product), "p");
        var priceProp = Expression.Property(Param, "Price");
        var minCondition = Expression.GreaterThanOrEqual(priceProp, Expression.Constant(minPrice));
        var maxCondition = Expression.LessThanOrEqual(priceProp, Expression.Constant(maxPrice));
        var finalCondition = Expression.AndAlso(minCondition, maxCondition);
        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryProp = Expression.Property(Param, "Category");
            var categoryCondition = Expression.Equal(categoryProp, Expression.Constant(category));
            finalCondition = Expression.AndAlso(finalCondition, categoryCondition);
        }
        var lambda = Expression.Lambda<Func<Product, bool>>(finalCondition, Param);
        var products = await context.Products.Where(lambda).ToListAsync();
        Console.WriteLine("\n--- Filtered Products ---");
        foreach (var p in products)
        {
            Console.WriteLine($"ID: {p.ProductId}, Name: {p.Name}, Price: {p.Price}, Category: {p.Category}");
        }
    }
    static void TopProducts(AppDbContext context)
    {
        var topProducts = context.OrderItems.Include(oi => oi.Product).GroupBy(oi => oi.Product!.Name).Select(g => new
        {
            Product = g.Key,
            QuantitySold = g.Sum(oi => oi.Quantity)
        }).OrderByDescending(x => x.QuantitySold).Take(5).ToList();
        Console.WriteLine("Top 5 Products:");
        foreach(var p in topProducts)
        {
            Console.WriteLine($"Product:{p.Product} QuantitySold:{p.QuantitySold}");
        }
    }
    static async Task OrdersByCustomertsAsync(AppDbContext context)
{
    Console.Write("Enter CustomerId: ");
    int customerId = int.Parse(Console.ReadLine() ?? "0");

    var customerOrders = await context.Orders
        .Where(o => o.CustomerId == customerId)
        .Include(o => o.Customer)
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .ToListAsync();

    if (customerOrders.Count > 0)
    {
        Console.WriteLine($"\nOrders placed by Customer ID: {customerId}");
        foreach (var order in customerOrders)
        {
            Console.WriteLine($"Order ID: {order.OrderId}, Customer: {order.Customer?.Name}, Total: {order.TotalAmount}");
            foreach (var item in order.OrderItems)
            {
                Console.WriteLine($"   Product: {item.Product?.Name}, Qty: {item.Quantity}");
            }
        }
    }
    else
    {
        Console.WriteLine("No orders found for this customer.");
    }
}

    static void TotalSalesByCategory(AppDbContext context)
    {
        var salesByCategory = context.OrderItems.Include(oi => oi.Product).GroupBy(oi => oi.Product!.Category).Select(g => new
        {
            Category = g.Key,
            TotalSales = g.Sum(oi => oi.Product!.Price * oi.Quantity)
        }).ToList();
        foreach(var sc in salesByCategory)
        {
            Console.WriteLine($"Category:{sc.Category} TotalSales:{sc.TotalSales}");
        }
    }
}