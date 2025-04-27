using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Repository.Interface;

namespace OhBau.Service
{
    public abstract class BaseService<T> where T : class
    {
        protected IUnitOfWork<OhBauContext> _unitOfWork;
        protected ILogger<T> _logger;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        public BaseService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<T> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string GetUsernameFromJwt()
        {
            var claim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            string username = claim?.Value;
            return username;
        }

        protected string GetRoleFromJwt()
        {
            var claim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Role);
            string role = claim?.Value;
            return role;
        }

    }
}
