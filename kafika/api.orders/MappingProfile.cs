using api.orders.Models;
using api.orders.persistence.Entities;
using AutoMapper; 

namespace api.orders
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderRequest>().ReverseMap();
            CreateMap<Order, OrderModel>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailModel>().ReverseMap();

            CreateMap<AuthenticateRequest, AuthenticateRequestModel>().ReverseMap();
            CreateMap<AuthenticateResponse, AuthenticateResponseModel>().ReverseMap();
        }
    }
}