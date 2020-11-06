using AutoMapper;
using OnlineShopping.FrontEnd.Api.Models.Entities;
using OnlineShopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.FrontEnd.Api.Models.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistrationViewModel, AppUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
        }
    }
}
