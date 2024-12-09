using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Repositories;

public class ProdAndServRepository : IProdAndServRepository
{
    private readonly AppDbContext _context;

    public ProdAndServRepository(AppDbContext context)
    {
        _context = context;
    }

    // add categories

    // **Products**

    public async Task<(IEnumerable<Product>, int totalCount)> GetAllProductsAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
    {
        var query = _context.Products.AsQueryable();

        if (from != null)
        {
            query = query.Where(p => p.CreatedAt >= from);
        }

        if (to != null)
        {
            query = query.Where(p => p.CreatedAt <= to);
        }

        var totalCount = await query.CountAsync();
        var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (products, totalCount);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> UpdateProductAsync(Guid productId, Product updatedProduct)
    {
        var existingProduct = await GetProductByIdAsync(productId);

        if (existingProduct == null)
        {
            return null;
        }

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.ImageUrl = updatedProduct.ImageUrl;
        //existingProduct.StockQuantity = updatedProduct.StockQuantity;
        existingProduct.BusinessId = updatedProduct.BusinessId;
        existingProduct.BaseProductId = updatedProduct.BaseProductId;

        _context.Products.Update(existingProduct);
        await _context.SaveChangesAsync();

        return existingProduct;
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }

    // **Services**

    public async Task<(IEnumerable<Service>, int totalCount)> GetAllServicesAsync(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
    {
        var query = _context.Services.AsQueryable();

        if (from != null)
        {
            query = query.Where(s => s.CreatedAt >= from);
        }

        if (to != null)
        {
            query = query.Where(s => s.CreatedAt <= to);
        }

        var totalCount = await query.CountAsync();
        var services = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (services, totalCount);
    }

    public async Task<Service?> GetServiceByIdAsync(Guid id)
    {
        return await _context.Services.FindAsync(id);
    }

    public async Task<Service> AddServiceAsync(Service service)
    {
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();

        return service;
    }

    public async Task<Service?> UpdateServiceAsync(Guid serviceId, Service updatedService)
    {
        var existingService = await GetServiceByIdAsync(serviceId);

        if (existingService == null)
        {
            return null;
        }

        existingService.Name = updatedService.Name;
        existingService.Description = updatedService.Description;
        existingService.Price = updatedService.Price;
        existingService.Interval = updatedService.Interval;
        existingService.EmployeeId = updatedService.EmployeeId;

        _context.Services.Update(existingService);
        await _context.SaveChangesAsync();

        return existingService;
    }

    public async Task<bool> DeleteServiceAsync(Guid id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return false;
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ProductStock?> GetProductStockAsync(Guid productId)
    {
        return await _context.ProductStocks.FindAsync(productId);
    }
}