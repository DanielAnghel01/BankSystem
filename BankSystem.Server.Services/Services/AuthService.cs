using AutoMapper;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Services
{
    public class AuthService
    {
        private readonly BankDbContext _bankRepository;
        private readonly IMapper _mapper;
        public AuthService(BankDbContext bankRepository, IMapper mapper)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }

        public async Task<HttpResult> Login(LoginServiceDto loginServiceDto)
        {
            try
            {
                var IsValidUser = await _bankRepository.Users.AnyAsync(u => u.Username == loginServiceDto.UserName && u.Password == loginServiceDto.Password);

                if(IsValidUser)
                {
                    return HttpResult.Factory.Create(HttpStatusCode.OK, "Login succesfully");
                }
                else
                {
                    return HttpResult.Factory.Create(HttpStatusCode.BadRequest, null, "Username or Password is incorect");
                }    
            }
            catch(Exception ex)
            {
                //_logger.Error(ex);
                return HttpResult.Factory.Create(HttpStatusCode.InternalServerError, null, "Internal server error");
            }
        }
    }
}
