using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using Castle.Core.Internal;
using Castle.Core.Resource;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext context = new CarDealerContext();

            //string inputXml = File.ReadAllText("../../../Datasets/cars.xml");

            string result = GetCarsWithTheirListOfParts(context);

            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportSupplierDto[] supplierDtos = xmlHelper.Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");
            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();

            foreach(ImportSupplierDto supplierDto in supplierDtos)
            {
                
                if(String.IsNullOrEmpty(supplierDto.Name)) 
                {
                    continue;
                }

                Supplier supplier = mapper.Map<Supplier>(supplierDto);
                validSuppliers.Add(supplier);
            }

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportPartDto[] partDtos = xmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");
            ICollection<Part> validParts = new HashSet<Part>();

            foreach(ImportPartDto partDto in partDtos)
            {
                if (string.IsNullOrEmpty(partDto.Name))
                {
                    continue;
                }

                if (!partDto.SupplierId.HasValue || !context.Suppliers.Any(s => s.Id == partDto.SupplierId))
                {
                    continue;
                }

                Part part = mapper.Map<Part>(partDto);
                validParts.Add(part);
            }

            context.Parts.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportCarDto[] carDtos = xmlHelper.Deserialize<ImportCarDto[]>(inputXml, "Cars");
            ICollection<Car> validCars = new HashSet<Car>();

            foreach(ImportCarDto carDto in carDtos)
            {
                if(string.IsNullOrEmpty(carDto.Make) || string.IsNullOrEmpty(carDto.Model)) 
                {
                    continue;
                }

                Car car = mapper.Map<Car>(carDto);
                
                foreach(var partDto in carDto.Parts.DistinctBy(p => p.PartId))
                {
                    if(!context.Parts.Any(p => p.Id == partDto.PartId))
                    {
                        continue;
                    }

                    PartCar carPart = new PartCar()
                    {
                        PartId = partDto.PartId
                    };

                    car.PartsCars.Add(carPart);
                }

                validCars.Add(car);
            }

            context.Cars.AddRange(validCars);
            context.SaveChanges();

            return $"Successfully imported {validCars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string  inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportCustomerDto[] customerDtos = xmlHelper.Deserialize<ImportCustomerDto[]>(inputXml, "Customers");
            ICollection<Customer> validCustomers = new HashSet<Customer>();

            foreach(ImportCustomerDto customerDto in customerDtos)
            {
                if(string.IsNullOrEmpty(customerDto.Name) || string.IsNullOrEmpty(customerDto.BirthDate)) 
                {
                    continue;
                }

                Customer customer = mapper.Map<Customer>(customerDto);

                validCustomers.Add(customer);
            }

            context.Customers.AddRange(validCustomers);
            context.SaveChanges();

            return $"Successfully imported {validCustomers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportSaleDto[] saleDtos = xmlHelper.Deserialize<ImportSaleDto[]>(inputXml, "Sales");
            ICollection<Sale> validSales = new HashSet<Sale>();

            foreach(ImportSaleDto saleDto in saleDtos)
            {
                if(!saleDto.CarId.HasValue || !context.Cars.Any(c => c.Id == saleDto.CarId.Value))
                {
                    continue;
                }   
                
                Sale sale = mapper.Map<Sale>(saleDto);

                validSales.Add(sale);
            }

            context.Sales.AddRange(validSales);
            context.SaveChanges();

            return $"Successfully imported {validSales.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCarDto[] cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarDto>(mapper.ConfigurationProvider)
                .ToArray();

            return xmlHelper.Serialize<ExportCarDto[]>(cars, "cars");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCarFromBmwDto[] cars = context.Cars
                .Where(c => c.Make.ToUpper() == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportCarFromBmwDto>(mapper.ConfigurationProvider)
                .ToArray();

            return xmlHelper.Serialize<ExportCarFromBmwDto[]>(cars, "cars");

        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCarWithPartsDto[] cars = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarWithPartsDto>(mapper.ConfigurationProvider)
                .ToArray();

            return xmlHelper.Serialize<ExportCarWithPartsDto[]>(cars, "cars");
        }

        private static IMapper InitializeAutoMapper()
            => new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));
    }
}