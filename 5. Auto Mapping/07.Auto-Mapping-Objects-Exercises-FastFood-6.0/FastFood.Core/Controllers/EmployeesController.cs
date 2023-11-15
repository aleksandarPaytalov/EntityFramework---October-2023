using AutoMapper.QueryableExtensions;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Core.Controllers
{
    using System;
    using AutoMapper;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Employees;

    public class EmployeesController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public EmployeesController(FastFoodContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Register()
        {
            var position = await _context.Positions
                .ProjectTo<RegisterEmployeeViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return View(position);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterEmployeeInputModel model)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Error", "Home");
            }

            //Option 1 without Automapper
            //var employee = new Employee
            //{
            //    Name = model.Name,
            //    Age = model.Age,
            //    Address = model.Address,
            //    PositionId = model.PositionId
            //};

            var employee = _mapper.Map<Employee>(model);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        public async Task<IActionResult> All()
        {
            var employees = await _context.Employees
                .Select(e => new EmployeesAllViewModel
                {
                    Name = e.Name,
                    Age = e.Age,
                    Address = e.Address,
                    Position = e.Position.Name
                })
                .ToListAsync();

            return View(employees);
        }
    }
}
