using CommerceTest.Domain.Models;

namespace CommerceTest.Application.Interfaces;

public interface ICategoryService
{
    Task<Category[]> GetL1Categories(CancellationToken ct = default);
}