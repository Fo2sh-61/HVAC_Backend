using AutoMapper;
using Backend.DTO.EngineerDto;
using Backend.DTO.ReveiwDto;
using Backend.DTO.ReviewDto;
using Backend.DTO.ServiceRequestDto;
using Backend.Models;

namespace Backend.helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateEngineerDto, Engineer>();
            CreateMap<UpdateEngineerDto, Engineer>();

            CreateMap<CreateServiceDto, Service>();

            CreateMap<CreateServiceRequestDto, ServiceRequest>();
            CreateMap<UpdateServiceRequestStatusDto, ServiceRequest>();
            CreateMap<UpdateServiceRequestPriceDto, ServiceRequest>();

            CreateMap<CreateReviewDto, Review>();

            //CreateMap<CreateProduct, Product>().ForMember(d => d.ImageURL, opt => opt.Ignore());
            //CreateMap<Product, GetProduct>().ForMember(p=>p.OldPrice,opt=>opt.MapFrom(src=>src.Price*1.1m));
            //CreateMap<UpdateProduct,Product>().ForMember(d => d.ImageURL, opt => opt.Ignore());

        }


    }
}

