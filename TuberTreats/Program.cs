using TuberTreats.Models;
using TuberTreats.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Sample data for TuberDrivers
var drivers = new List<TuberDriver>
{
    new TuberDriver { Id = 1, Name = "John Doe" },
    new TuberDriver { Id = 2, Name = "Jane Smith" },
    new TuberDriver { Id = 3, Name = "Michael Johnson" }
};

// Sample data for Customers
var customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Alice Johnson", Address = "123 Main St" },
    new Customer { Id = 2, Name = "Bob Smith", Address = "456 Oak Ave" },
    new Customer { Id = 3, Name = "Charlie Brown", Address = "789 Pine Rd" },
    new Customer { Id = 4, Name = "David White", Address = "101 Maple St" },
    new Customer { Id = 5, Name = "Eva Green", Address = "202 Birch Rd" }
};

// Sample data for Toppings
var toppings = new List<Topping>
{
    new Topping { Id = 1, Name = "Cheese" },
    new Topping { Id = 2, Name = "Bacon" },
    new Topping { Id = 3, Name = "Chives" },
    new Topping { Id = 4, Name = "Sour Cream" },
    new Topping { Id = 5, Name = "Butter" }
};

// Sample data for TuberOrders
List<TuberOrder> orders = new List<TuberOrder>
{
    new TuberOrder
    {
        Id = 1,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 1,
        TuberDriverId = 1, // Assigned to John Doe
        DeliveredOnDate = null,
    },
    new TuberOrder
    {
        Id = 2,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 2,
        TuberDriverId = 2, // Assigned to Jane Smith
        DeliveredOnDate = DateTime.Now,
    },
    new TuberOrder
    {
        Id = 3,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 3,
        TuberDriverId = 3, // Assigned to Michael Johnson
        DeliveredOnDate = null,
    }
};

var tuberToppings = new List<TuberTopping>
{
    new TuberTopping { Id = 1, TuberOrderId = 1, ToppingId = 1 },
    new TuberTopping { Id = 2, TuberOrderId = 1, ToppingId = 2 },
    new TuberTopping { Id = 3, TuberOrderId = 2, ToppingId = 3 },
    new TuberTopping { Id = 4, TuberOrderId = 2, ToppingId = 4 }
};

// Populate relationships
foreach (var order in orders)
{
    order.Customer = customers.Find(c => c.Id == order.CustomerId);
    order.TuberDriver = drivers.Find(d => d.Id == order.TuberDriverId);
    order.Customer.TuberOrders.Add(order);
    order.TuberDriver.TuberDeliveries.Add(order);
}

// Configure Swagger for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Route for TuberOrders
// Get all TuberOrders
app.MapGet("/tuberorders", () =>
{
    return orders.Select(o =>
    {
        // Get the TuberToppings associated with this order
        List<TuberTopping> orderToppings = tuberToppings.Where(tt => tt.TuberOrderId == o.Id).ToList();

        return new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            CustomerId = o.CustomerId,
            TuberDriverId = o.TuberDriverId ?? -1, // Handle missing driver with default -1
            DeliveredOnDate = o.DeliveredOnDate,
            Toppings = orderToppings.Select(ot => new ToppingDTO
            {
                Id = toppings.FirstOrDefault(t => ot.ToppingId == t.Id)?.Id ?? 0,
                Name = toppings.FirstOrDefault(t => ot.ToppingId == t.Id)?.Name ?? "Unknown"
            }).ToList()
        };
    }).ToList();
});

// Get a TuberOrder by Id
app.MapGet("/tuberorders/{id}", (int id) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null) return Results.NotFound();

    // Get the TuberToppings associated with this order
    List<TuberTopping> orderToppings = tuberToppings.Where(tt => tt.TuberOrderId == order.Id).ToList();

    return Results.Ok(new TuberOrderDTO
    {
        Id = order.Id,
        OrderPlacedOnDate = order.OrderPlacedOnDate,
        CustomerId = order.CustomerId,
        TuberDriverId = order.TuberDriverId ?? -1, // Default to -1 if no driver
        DeliveredOnDate = order.DeliveredOnDate,
        Toppings = orderToppings.Select(ot => new ToppingDTO
        {
            Id = toppings.FirstOrDefault(t => ot.ToppingId == t.Id)?.Id ?? 0,
            Name = toppings.FirstOrDefault(t => ot.ToppingId == t.Id)?.Name ?? "Unknown"
        }).ToList()
    });
});

// Submit a new TuberOrder
app.MapPost("/tuberorders", (TuberOrder newOrder) =>
{
    // Validate CustomerId and TuberDriverId
    var customer = customers.FirstOrDefault(c => c.Id == newOrder.CustomerId);
    // if (customer == null) return Results.BadRequest("Invalid CustomerId");

    var driver = drivers.FirstOrDefault(d => d.Id == newOrder.TuberDriverId);
    // if (driver == null) return Results.BadRequest("Invalid TuberDriverId");

    // Map DTO to Model
    var newTuberOrder = new TuberOrder
    {

        Id = orders.Count > 0 ? orders.Max(o => o.Id + 1) :1,  // Generate new ID
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = newOrder.CustomerId,
        DeliveredOnDate = null,
        Toppings = newOrder.Toppings.Select(t => new Topping
        {
            Id = t.Id,
            Name = t.Name
        }).ToList()
    };

    orders.Add(newTuberOrder);

    return newTuberOrder;

    //return Results.Created($"/tuberorders/{newTuberOrder.Id}", newTuberOrder);
});



    // return Results.Ok(new TuberOrderDTO
    // {
    //     Id = newOrder.Id,
    //     OrderPlacedOnDate = newOrder.OrderPlacedOnDate,
    //     CustomerId = newOrder.CustomerId,
    //     TuberDriverId = newOrder.TuberDriverId ?? -1,
    //     DeliveredOnDate = newOrder.DeliveredOnDate,
    //     Toppings = newOrder.Toppings
    //         .Select(tt => new ToppingDTO
    //         {
    //             Id = toppings.FirstOrDefault(t => tt.Id == t.Id)?.Id ?? 0,
    //             Name = toppings.FirstOrDefault(t => tt.Id == t.Id)?.Name ?? "Unknown"
    //         }).ToList()
    // });


// Assign a driver to an order (PUT)
app.MapPut("/tuberorders/{id}", (int id, TuberOrderDTO updatedOrderDto) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null) return Results.NotFound();

    // Validate if TuberDriverId exists in the drivers list
    var driver = drivers.FirstOrDefault(d => d.Id == updatedOrderDto.TuberDriverId);
    if (driver == null) return Results.BadRequest("Invalid TuberDriverId");

    // Update the order's TuberDriverId
    order.TuberDriverId = updatedOrderDto.TuberDriverId;
    return Results.NoContent();
});

// Complete an order (POST)
app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);

    order.DeliveredOnDate = DateTime.Now;
    
    // if (order == null) return Results.NotFound();

    // return Results.Ok(new TuberOrderDTO
    // {
    //     Id = order.Id,
    //     OrderPlacedOnDate = order.OrderPlacedOnDate,
    //     CustomerId = order.CustomerId,
    //     TuberDriverId = order.TuberDriverId ?? -1,
    //     DeliveredOnDate = DateTime.Now,
    //     Toppings = order.Toppings
    //         .Select(tt => new ToppingDTO
    //         {
    //             Id = toppings.FirstOrDefault(t => tt.Id == t.Id).Id,
    //             Name = toppings.FirstOrDefault(t => tt.Id == t.Id).Name
    //         }).ToList() 
    // });
});

// Route for Toppings
// Get all toppings
app.MapGet("/toppings", () =>
{
    return toppings.Select(t => new ToppingDTO
    {
        Id = t.Id,
        Name = t.Name
    });
});

// Get topping by Id
app.MapGet("/toppings/{id}", (int id) =>
{
    var topping = toppings.FirstOrDefault(t => t.Id == id);
    if (topping == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new ToppingDTO
    {
        Id = topping.Id,
        Name = topping.Name
    });
});

// Route for TuberToppings
// Get all TuberToppings
app.MapGet("/tubertoppings", () =>
{
    return 
        tuberToppings.Select(tt => new TuberToppingDTO
        {
            Id = tt.Id,
            TuberOrderId = tt.TuberOrderId,
            ToppingId = tt.ToppingId
        }).ToList();
});

// Add a topping to a TuberOrder
app.MapPost("/tubertoppings", (TuberToppingDTO newTuberTopping) =>
{
    var order = orders.FirstOrDefault(o => o.Id == newTuberTopping.TuberOrderId);
    if (order == null) return Results.NotFound();

    var topping = toppings.FirstOrDefault(t => t.Id == newTuberTopping.ToppingId);
    if (topping == null) return Results.BadRequest("Invalid ToppingId");

    var tuberTopping = new TuberTopping
    {
        Id = tuberToppings.Max(tt => tt.Id) + 1,
        TuberOrderId = newTuberTopping.TuberOrderId,
        ToppingId = newTuberTopping.ToppingId
    };

    tuberToppings.Add(tuberTopping);

    return Results.Ok(tuberTopping);
});

// Remove a topping from a TuberOrder
app.MapDelete("/tubertoppings/{id}", (int id) =>
{
    var tuberTopping = tuberToppings.FirstOrDefault(tt => tt.Id == id);
    if (tuberTopping == null) return Results.NotFound();

    // Remove the TuberTopping entry
    tuberToppings.Remove(tuberTopping);

    // Also remove the corresponding topping from the order
    var order = orders.FirstOrDefault(o => o.Id == tuberTopping.TuberOrderId);
    if (order != null)
    {
        var topping = order.Toppings.FirstOrDefault(tt => tt.Id == tuberTopping.Id);
        if (topping != null)
        {
            order.Toppings.Remove(topping);
        }
    }

    return Results.NoContent();
});


// Route for Customers
// Get all Customers
app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

// Get a customer by id, with their orders
app.MapGet("/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) return Results.NotFound();

    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        Orders = customer.TuberOrders.Select(o => new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            DeliveredOnDate = o.DeliveredOnDate,
            Toppings = o.Toppings.Select(tt => new ToppingDTO
            {
                Id = toppings.FirstOrDefault(t => tt.Id == t.Id)?.Id ?? 0,
                Name = toppings.FirstOrDefault(t => tt.Id == t.Id)?.Name ?? "Unknown"
            }).ToList()
        }).ToList()
    });
});

// Add a new Customer
app.MapPost("/customers", (CustomerDTO newCustomerDto) =>
{
    var newCustomer = new Customer
    {
        Id = customers.Max(c => c.Id) + 1,
        Name = newCustomerDto.Name,
        Address = newCustomerDto.Address
    };

    customers.Add(newCustomer);

    return Results.Ok(new CustomerDTO
    {
        Id = newCustomer.Id,
        Name = newCustomer.Name,
        Address = newCustomer.Address
    });
});

// Delete a Customer
app.MapDelete("/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) return Results.NotFound();

    customers.Remove(customer);
    return Results.NoContent();
});

// Route for TuberDrivers
// Get all TuberDrivers
app.MapGet("/tuberdrivers", () =>
{
    return drivers.Select(d => new TuberDriverDTO
    {
        Id = d.Id,
        Name = d.Name,
        TuberDeliveries = d.TuberDeliveries.Select(o => new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            DeliveredOnDate = o.DeliveredOnDate,
            CustomerId = o.CustomerId,
            TuberDriverId = o.TuberDriverId ?? -1, // Default to -1 if no driver
            Toppings = tuberToppings
                .Where(tt => tt.TuberOrderId == o.Id) // Filter to only those toppings that belong to this order
                .Select(tt => new ToppingDTO
                {
                    Id = toppings.FirstOrDefault(t => t.Id == tt.Id)?.Id ?? 0,  // Handle null safely
                    Name = toppings.FirstOrDefault(t => t.Id == tt.Id)?.Name ?? "Unknown"  // Handle null safely
                }).ToList()
        }).ToList()  // Ensure the deliveries are fully materialized into a list
    });
});



// Get a TuberDriver by id with their deliveries
app.MapGet("/tuberdrivers/{id}", (int id) =>
{
    var driver = drivers.FirstOrDefault(d => d.Id == id);
    if (driver == null) return Results.NotFound();

    // Get the TuberOrders (Deliveries) associated with this driver
    var deliveries = driver.TuberDeliveries.Select(o =>
    {
        // Get the TuberToppings associated with this order
        List<TuberTopping> orderToppings = tuberToppings.Where(tt => tt.TuberOrderId == o.Id).ToList();

        // Return the TuberOrderDTO
        return new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            DeliveredOnDate = o.DeliveredOnDate,
            CustomerId = o.CustomerId,
            TuberDriverId = o.TuberDriverId ?? -1, // Default to -1 if no driver
            Toppings = orderToppings.Select(ot => new ToppingDTO
            {
                Id = toppings.FirstOrDefault(t => ot.ToppingId == t.Id)?.Id ?? 0,
                Name = toppings.FirstOrDefault(t => ot.ToppingId == t.Id)?.Name ?? "Unknown"
            }).ToList()
        };
    }).ToList();  // Ensure it's being materialized into a list

    // Return the driver with its associated orders (deliveries)
    return Results.Ok(new TuberDriverDTO
    {
        Id = driver.Id,
        Name = driver.Name,
        TuberDeliveries = deliveries
    });
});



app.Run();

//don't touch or move this!
public partial class Program { }