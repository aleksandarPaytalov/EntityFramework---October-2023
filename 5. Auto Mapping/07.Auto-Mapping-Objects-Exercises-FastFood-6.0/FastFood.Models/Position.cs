namespace FastFood.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Position
    {
        public int Id { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}