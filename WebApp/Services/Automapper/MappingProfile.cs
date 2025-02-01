using AutoMapper;
using Infrastructure.Models;
using Models.Request;
using Models.Response;

namespace Services.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProcessFoodRequest, FoodAnalysis>().ReverseMap();
            CreateMap<ProcessFoodResponse, FoodAnalysis>().ReverseMap();
        }
    }
}
