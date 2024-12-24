using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Runtime.CompilerServices;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using var context = new ProductShopContext();

            //string inputJason = File.ReadAllText(@"../../../Datasets/categories-products.json");

            string result = GetUsersWithProducts(context);

            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> validUsers = new HashSet<User>();

            foreach (var userDto in userDtos)
            {
                User user  = mapper.Map<User>(userDto);
                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

            ICollection<Product> validProducts = new HashSet<Product>();

            foreach (var productDto in productDtos)
            {
                Product product = mapper.Map<Product>(productDto);
                validProducts.Add(product);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";

        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            ImportCategoryDto[] categoryDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (var categoryDto in categoryDtos)
            {
                if(String.IsNullOrEmpty(categoryDto.Name)) 
                {
                    continue;
                }

                Category category = mapper.Map<Category>(categoryDto);
                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            ImportCategoryProductDto[] categoryProductDtos = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            ICollection<CategoryProduct> validCategoryProducts = new HashSet<CategoryProduct>();

            foreach (var categoryProductDto in categoryProductDtos)
            {
                if (!context.Categories.Any(c => c.Id == categoryProductDto.CategoryId) ||
                   !context.Products.Any(p => p.Id == categoryProductDto.ProductId))
                {
                    continue;
                }

                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(categoryProductDto);
                validCategoryProducts.Add(categoryProduct);
            }

            context.CategoriesProducts.AddRange(validCategoryProducts);
            context.SaveChanges();

            return $"Successfully imported {validCategoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            //var products = context.Products
            //    .Where(p => p.Price >= 500 && p.Price <= 1000)
            //    .OrderBy(p => p.Price)
            //    .Select(p => new
            //    {
            //        name = p.Name,
            //        price = p.Price,
            //        seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
            //    })
            //    .AsNoTracking()
            //    .ToArray();
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            ExportProductsInRangeDto[] productDtos = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .AsNoTracking()
                .ProjectTo<ExportProductsInRangeDto>(mapper.ConfigurationProvider)
                .ToArray();

            return JsonConvert.SerializeObject(productDtos, Formatting.Indented);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(false, true)
            };

            
            var usersWithSoldProducts = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.LastName)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    SoldProducts = u.ProductsSold
                    .Where(p => p.Buyer != null)
                    .Select(p => new
                    {
                        p.Name,
                        p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                    .ToArray()
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(usersWithSoldProducts, Formatting.Indented, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver
            });
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(false, true)
            };

            var categoriesByProductCount = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new
                {
                    Category = c.Name,
                    ProductCount = c.CategoriesProducts.Count,
                    AveragePrice = Math.Round((double)c.CategoriesProducts.Average(p => p.Product.Price), 2),
                    TotalRevenue = Math.Round((double)c.CategoriesProducts.Sum(p => p.Product.Price), 2)
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(categoriesByProductCount, Formatting.Indented, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver
            });
        }
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(false, true)
            };

            var usersWithBuyer = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new
                {
                    
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts =  new 
                    {
                        Count = u.ProductsSold.Count(p => p.BuyerId != null),
                        Products = u.ProductsSold
                          .Where(p => p.BuyerId != null)
                          .Select(p => new
                          {
                             Name = p.Name,
                             Price = p.Price,
                          })
                          .ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .AsNoTracking()
                .ToArray();
            var usersWithBuyerWrapper = new
            {
                UsersCount = usersWithBuyer.Length,
                Users = usersWithBuyer
            };

            return JsonConvert.SerializeObject(usersWithBuyerWrapper, Formatting.Indented, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}