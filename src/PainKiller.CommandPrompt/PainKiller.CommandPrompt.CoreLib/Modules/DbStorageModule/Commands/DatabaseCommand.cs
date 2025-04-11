using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Services;

namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Commands;

[CommandDesign(description:"Try out the DbStorage module",
                   options: ["insert"],
                  examples: ["//Show database info","database","//Insert some products","database --insert"])]
public class DatabaseCommand(string identifier) : ConsoleCommandBase<ApplicationConfiguration>(identifier)
{
    private DbStorageService<Product>? _dbStorageService;
    public override void OnInitialized() => _dbStorageService = new DbStorageService<Product>(Configuration.Core.Modules.DbStorage);

    public override RunResult Run(ICommandLineInput input)
    {
        _dbStorageService?.Initialize();

        if (input.HasOption("insert")) Insert();
        var products = _dbStorageService?.GetAll() ?? [];
        Writer.WriteTable(products);
        return Ok();
    }

    private void Insert()
    {
        var rnd = new Random();
        var productNames = new[] { "Wireless Mouse", "Coffee Mug", "Notebook", "Bluetooth Speaker", "Desk Lamp" };

        for (int i = 0; i < 5; i++)
        {
            var product = new Product
            {
                Name = productNames[i],
                Price = (decimal)Math.Round(rnd.NextDouble() * 100, 2), // 0.00 - 100.00
                CreatedAt = DateTime.Now.AddDays(-rnd.Next(1, 10)),
                Orders = [new Order() { Id = 1, Quantity = 100 }, new Order() { Id = 2, Quantity = 32 + i }, new Order() { Id = 3 + i + 3, Quantity = 110 + i + 5 }]
            };
            _dbStorageService?.InsertObject<int>(product);
        }
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonColumn]
        public List<Order> Orders { get; set; } = [];
    }

    public class Order
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}