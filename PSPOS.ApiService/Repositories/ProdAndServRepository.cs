using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories;

public class ProdAndServRepository : IProdAndServRepository
{
    private readonly AppDbContext _context;

    
}