using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarDealer.Data;
using CarDealer.DTO.ExportDTO;
using CarDealer.DTO.ImportDTO;
using CarDealer.Models;
using CarDealer.XMLHelper;
using Microsoft.EntityFrameworkCore.Internal;

namespace CarDealer
{
    public class StartUp
    {
        public const string ResultDirectoryPath = "../../../Datasets/Results";

        public static void Main(string[] args)
        {

            CarDealerContext context = new CarDealerContext();

            //just in the begining to create DB:
            //ResetDataBase(context);

            var suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            var carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            var customersXml = File.ReadAllText("../../../Datasets/customers.xml");
            var partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            var salesXml = File.ReadAllText("../../../Datasets/sales.xml");

            //09÷13
            //var result = ImportSales(context, salesXml);
            //Console.WriteLine(result);

            //14÷19
            var discountSales = GetSalesWithAppliedDiscount(context);

            if (!Directory.Exists(ResultDirectoryPath))
            {
                Directory.CreateDirectory(ResultDirectoryPath);
            }

            File.WriteAllText("../../../Datasets/Results/sales-discounts.xml", discountSales);

        }

        public static void ResetDataBase(CarDealerContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("DB deleted");
            db.Database.EnsureCreated();
            Console.WriteLine("DB created");

        }

        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {

            //var result = ImportSuppliers(context, suppliersXml);
            //Console.WriteLine(result);

            const string rootElement = "Suppliers";

            var supplierResult = XMLConverter.Deserializer<ImportSuppliersDto>(inputXml, rootElement);

            var suppliers = supplierResult
                .Select(s => new Supplier
                {
                    Name = s.Name,
                    IsImporter = s.IsImporter
                })
                .ToArray();


            context.Suppliers.AddRange(suppliers);
            int suppliersCount = context.SaveChanges();

            return $"Successfully imported {suppliersCount}";

        }

        //Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            //var result = ImportParts(context, partsXml);
            //Console.WriteLine(result);

            const string rootElement = "Parts";

            var partsResult = XMLConverter.Deserializer<ImportPartsDto>(inputXml, rootElement);

            var parts = partsResult
                .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                .Select(p => new Part
                {
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    SupplierId = p.SupplierId
                })
                .ToArray();

            context.Parts.AddRange(parts);
            int partsCount = context.SaveChanges();


            return $"Successfully imported {partsCount}";
        }

        //Query 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            //var result = ImportCars(context, carsXml);
            //Console.WriteLine(result);

            const string rootElement = "Cars";

            var carsResult = XMLConverter.Deserializer<ImportCarsDto>(inputXml, rootElement);

            var cars = new List<Car>();

            foreach (var carDto in carsResult)
            {
                var uniqueParts = carDto.Parts.Select(p => p.Id).Distinct().ToArray();
                var realParts = uniqueParts.Where(id => context.Parts.Any(rp => rp.Id == id));


                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance,
                    PartCars = realParts.Select(id => new PartCar
                    {
                        PartId = id
                    })
                    .ToArray()
                };

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            int carsCount = context.SaveChanges();

            return $"Successfully imported {carsCount}";
        }

        //Query 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            //var result = ImportCustomers(context, customersXml);
            //Console.WriteLine(result);

            const string rootElement = "Customers";

            var customerResult = XMLConverter.Deserializer<ImportCustomersDto>(inputXml, rootElement);

            var customers = customerResult
                .Select(c => new Customer
                {
                    Name = c.Name,
                    BirthDate = DateTime.Parse(c.BirthDate),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            context.Customers.AddRange(customers);
            int customersCount = context.SaveChanges();


            return $"Successfully imported {customersCount}";
        }

        //Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            //var result = ImportSales(context, salesXml);
            //Console.WriteLine(result);

            const string rootElement = "Sales";

            var salesResult = XMLConverter.Deserializer<ImportSalesDto>(inputXml, rootElement);

            var sales = salesResult
                .Where(s => context.Cars.Any(c => c.Id == s.CarId))
                .Select(s => new Sale
                {
                    CarId = s.CarId,
                    CustomerId = s.CustomerId,
                    Discount = s.Discount
                })
                .ToArray();

            context.Sales.AddRange(sales);
            int salesCount = context.SaveChanges();


            return $"Successfully imported {salesCount}";
        }

        //Query 14. Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            //var carsWithDistance = GetCarsWithDistance(context);

            //if (!Directory.Exists(ResultDirectoryPath))
            //{
            //Directory.CreateDirectory(ResultDirectoryPath);
            //}

            //File.WriteAllText("../../../Datasets/Results/cars.xml", carsWithDistance);

            const string rootElement = "cars";

            var carsWithDistance = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new ExportCarsWithDistance
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();


            var result = XMLConverter.Serialize(carsWithDistance, rootElement);

            return result;

        }

        //Query 15. Cars from make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            //var carsMakeBmw = GetCarsFromMakeBmw(context);

            //if (!Directory.Exists(ResultDirectoryPath))
            //{
            //Directory.CreateDirectory(ResultDirectoryPath);
            //}

            //File.WriteAllText("../../../Datasets/Results/bmw-cars.xml", carsMakeBmw);

            const string rootElement = "cars";

            var carsBmw = context.Cars
                .Where(c => c.Make.ToLower() == "bmw")
                .Select(c => new ExportCarBmwDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();


            var result = XMLConverter.Serialize(carsBmw, rootElement);

            return result;

        }

        //Query 16. Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            //var localSuppliers = GetLocalSuppliers(context);

            //if (!Directory.Exists(ResultDirectoryPath))
            //{
            //Directory.CreateDirectory(ResultDirectoryPath);
            //}

            //File.WriteAllText("../../../Datasets/Results/local-suppliers.xml", localSuppliers);

            const string rootElement = "suppliers";

            var localSupppliersResult = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportLocalSuppliersDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();


            var result = XMLConverter.Serialize(localSupppliersResult, rootElement);

            return result;

        }

        //Query 17. Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            //var carPartsList = GetCarsWithTheirListOfParts(context);

            //if (!Directory.Exists(ResultDirectoryPath))
            //{
            //Directory.CreateDirectory(ResultDirectoryPath);
            //}

            //File.WriteAllText("../../../Datasets/Results/cars-and-parts.xml", carPartsList);

            const string rootElement = "cars";

            var carsWithListOfParts = context.Cars
                .Select(c => new ExportCarsWithListOfPartsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(cp => new PartsDto
                    {
                        Name = cp.Part.Name,
                        Price = cp.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();


            var result = XMLConverter.Serialize(carsWithListOfParts, rootElement);

            return result;

        }

        //Query 18. Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //var totalSales = GetTotalSalesByCustomer(context);

            //if (!Directory.Exists(ResultDirectoryPath))
            //{
            //Directory.CreateDirectory(ResultDirectoryPath);
            //}

            //File.WriteAllText("../../../Datasets/Results/customers-total-sales.xml", totalSales);

            const string rootElement = "customers";

            var customerWithCars = context.Sales
                .Where(s => s.Car.Sales.Any())
                .Select(s => new ExportTotalSalesCustomersDto
                {
                    FullName = s.Customer.Name,
                    BoughtCars = s.Customer.Sales.Count,
                    SpentMoney = s.Car.PartCars.Sum(p => p.Part.Price)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();


            var result = XMLConverter.Serialize(customerWithCars, rootElement);

            return result;

        }

        //Query 19. Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            //var discountSales = GetSalesWithAppliedDiscount(context);

            //if (!Directory.Exists(ResultDirectoryPath))
            //{
            //Directory.CreateDirectory(ResultDirectoryPath);
            //}

            //File.WriteAllText("../../../Datasets/Results/sales-discounts.xml", discountSales);

            const string rootElement = "sales";

            var discountSales = context.Sales
                .Select(s => new ExportDicountSalesDto
                {
                    Car = new CarDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance,
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(p => p.Part.Price) - s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100

                })
                .ToArray();


            var result = XMLConverter.Serialize(discountSales, rootElement);

            return result;

        }
    }
}
