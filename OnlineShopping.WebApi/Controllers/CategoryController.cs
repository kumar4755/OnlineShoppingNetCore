using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.FrontEnd.Repository;

namespace OnlineShopping.FrontEnd.Api.Controllers
{
    [Route("api/Category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository = null;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Route("GetAllCategories")]
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            try
            {
                var _categoryList = _categoryRepository.GetAllCategories().ToList();
                return Ok(_categoryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}