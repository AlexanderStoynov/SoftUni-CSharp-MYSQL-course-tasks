using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Globalization;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplierDto, Supplier>();

            this.CreateMap<ImportPartDto, Part>()
                .ForMember(d => d.SupplierId, opt => opt.MapFrom(s => s.SupplierId!.Value));
            this.CreateMap<Part, ExportCarPartDto>();

            this.CreateMap<ImportCarDto, Car>()
                .ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());
            this.CreateMap<Car, ExportCarDto>();
            this.CreateMap<Car, ExportCarFromBmwDto>();
            this.CreateMap<Car, ExportCarWithPartsDto>()
                .ForMember(d => d.Parts, opt => opt.MapFrom(s => s.PartsCars.Select(pc => pc.Part).OrderByDescending(p => p.Price).ToArray()));
                
            

            this.CreateMap<ImportCustomerDto, Customer>()
                .ForMember(d => d.BirthDate, opt => opt.MapFrom(s => DateTime.Parse(s.BirthDate, CultureInfo.InvariantCulture)));

            this.CreateMap<ImportSaleDto, Sale>()
                .ForMember(d => d.CarId, opt => opt.MapFrom(s => s.CarId!.Value));

                
                
        }
    }
}
