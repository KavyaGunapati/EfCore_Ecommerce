using System.Threading.Tasks;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        using var context = new AppDbContext();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n--- E-Commerce Menu ---");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Add Customer");
            Console.WriteLine("3. Create Order");
            Console.WriteLine("4. View Products");
            Console.WriteLine("5. View Customers");
            Console.WriteLine("6. View Orders");
            Console.WriteLine("7. Filter Products");
            Console.WriteLine("8. Exit");
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
                    exit = true;
                    break;
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
        var email = Console.ReadLine();
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

    }
    static void ViewProductsAsync(AppDbContext context)
    {

    }
    static void ViewCustomersAsync(AppDbContext context)
    {

    }
    static void ViewOrdersAsync(AppDbContext context)
    {

    }
    static void FilterProductsAsync(AppDbContext context)
    {

    }
}