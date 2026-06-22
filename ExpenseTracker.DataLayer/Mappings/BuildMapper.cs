using AutoMapper;
using ExpenseTracker.Application.Dtos;
using ExpenseTracker.DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExpenseTracker.DataLayer.Mappings
{
    public class BuildMapper :Profile
    {
        public BuildMapper()
        {
            CreateMap<User,UserDto>();
            CreateMap<UserDto,User>();

            CreateMap<CategoryDto,Category>();
            CreateMap<Category,CategoryDto>();

            CreateMap<Expense,ExpenseDto>();
            CreateMap<ExpenseDto,Expense>();
            

        }
    }
}
