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

    // **Product Stock**

    public async Task<ProductStock?> GetProductStockAsync(Guid productId)
    {
        return await _context.ProductStocks
     .FirstOrDefaultAsync(ps => ps.ProductId == productId);

    }

    public async Task AddProductStockAsync(ProductStock productStock)
    {
        await _context.ProductStocks.AddAsync(productStock);
        await _context.SaveChangesAsync();
    }

    public async Task<ProductStock?> UpdateProductStockAsync(ProductStock productStock)
    {
        var existingProductStock = await _context.ProductStocks.FindAsync(productStock.ProductId);
        if (existingProductStock == null)
        {
            return null;
        }

        existingProductStock.Quantity = productStock.Quantity;
        _context.ProductStocks.Update(existingProductStock);
        await _context.SaveChangesAsync();

        return existingProductStock;
    }

    public async Task<bool> DeleteProductStockAsync(ProductStock productStock)
    {
        var existingProductStock = await _context.ProductStocks.FindAsync(productStock.ProductId);
        if (existingProductStock == null)
        {
            return false;
        }

        _context.ProductStocks.Remove(existingProductStock);
        await _context.SaveChangesAsync();

        return true;
    }

    // **Product Groups**

    public async Task<(IEnumerable<ProductGroup>, int)> GetAllProductGroupsAsync(int skip, int limit)
    {
        var query = _context.ProductGroups.AsQueryable();
        var totalCount = await query.CountAsync();
        var productGroups = await query.Skip(skip).Take(limit).ToListAsync();
        return (productGroups, totalCount);
    }

    public async Task<ProductGroup?> GetProductGroupByIdAsync(Guid id)
    {
        return await _context.ProductGroups.FindAsync(id);
    }

    public async Task<ProductGroup> AddProductGroupAsync(ProductGroup group)
    {
        await _context.ProductGroups.AddAsync(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<ProductGroup?> UpdateProductGroupAsync(ProductGroup group)
    {
        var existingGroup = await GetProductGroupByIdAsync(group.Id);
        if (existingGroup == null)
        {
            return null;
        }

        existingGroup.Name = group.Name;
        existingGroup.Description = group.Description;
        existingGroup.productOrServiceIds = group.productOrServiceIds;

        _context.ProductGroups.Update(existingGroup);
        await _context.SaveChangesAsync();
        return existingGroup;
    }

    public async Task<bool> DeleteProductGroupAsync(Guid id)
    {
        var group = await _context.ProductGroups.FindAsync(id);
        if (group == null)
        {
            return false;
        }

        _context.ProductGroups.Remove(group);
        await _context.SaveChangesAsync();
        return true;
    }

    // **Service Groups**

    public async Task<(IEnumerable<ServiceGroup>, int)> GetAllServiceGroupsAsync(int skip, int limit)
    {
        var query = _context.ServiceGroups.AsQueryable();
        var totalCount = await query.CountAsync();
        var serviceGroups = await query.Skip(skip).Take(limit).ToListAsync();
        return (serviceGroups, totalCount);
    }

    public async Task<ServiceGroup?> GetServiceGroupByIdAsync(Guid id)
    {
        return await _context.ServiceGroups.FindAsync(id);
    }

    public async Task<ServiceGroup> AddServiceGroupAsync(ServiceGroup group)
    {
        await _context.ServiceGroups.AddAsync(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<ServiceGroup?> UpdateServiceGroupAsync(ServiceGroup group)
    {
        var existingGroup = await GetServiceGroupByIdAsync(group.Id);
        if (existingGroup == null)
        {
            return null;
        }

        existingGroup.Name = group.Name;
        existingGroup.Description = group.Description;

        _context.ServiceGroups.Update(existingGroup);
        await _context.SaveChangesAsync();
        return existingGroup;
    }

    public async Task<bool> DeleteServiceGroupAsync(Guid id)
    {
        var group = await _context.ServiceGroups.FindAsync(id);
        if (group == null)
        {
            return false;
        }

        _context.ServiceGroups.Remove(group);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsProductAsync(Guid id)
    {
        // Logic to check if the ID corresponds to a product
        var product = await _context.Products.FindAsync(id);
        return product != null;
    }

    public async Task<bool> IsServiceAsync(Guid id)
    {
        // Logic to check if the ID corresponds to a service
        var service = await _context.Services.FindAsync(id);
        return service != null;
    }

    public async Task<bool> RemoveServiceFromGroupAsync(Guid serviceId)
    {
        var serviceGroups = await _context.ServiceGroups
            .Where(sg => sg.productOrServiceIds != null && sg.productOrServiceIds.Contains(serviceId))
            .ToListAsync();

        if (!serviceGroups.Any()) return false;

        foreach (var serviceGroup in serviceGroups)
        {
            serviceGroup.productOrServiceIds = serviceGroup.productOrServiceIds?
                .Where(id => id != serviceId)
                .ToArray();

            _context.ServiceGroups.Update(serviceGroup);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveProductFromGroupAsync(Guid productId)
    {
        var productGroups = await _context.ProductGroups
            .Where(pg => pg.productOrServiceIds != null && pg.productOrServiceIds.Contains(productId))
            .ToListAsync();

        if (!productGroups.Any()) return false;

        foreach (var productGroup in productGroups)
        {
            productGroup.productOrServiceIds = productGroup.productOrServiceIds?
                .Where(id => id != productId)
                .ToArray();

            _context.ProductGroups.Update(productGroup);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}