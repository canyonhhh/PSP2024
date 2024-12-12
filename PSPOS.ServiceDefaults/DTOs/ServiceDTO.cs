using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace PSPOS.ServiceDefaults.Models;

public class ServiceDTO
{
    public ServiceDTO(string name, string? description, decimal price, DateInterval duration, Guid employeeId)
    {
        Name = name;
        Description = description;
        Price = price;
        Duration = duration;
        EmployeeId = employeeId;
    }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    public DateInterval Duration { get; set; }

    [Required(ErrorMessage = "EmployeeId is required")]
    public Guid EmployeeId { get; set; }
}