using Microsoft.EntityFrameworkCore;
using UT2024P4LP4.Web.Data;
using UT2024P4LP4.Web.Data.Dtos;
using UT2024P4LP4.Web.Data.Entities;

namespace UT2024P4LP4.Web.Services;

public interface ICategoriaService
{
    Task<ResultList<CategoriaDtos>> GetAll(CancellationToken cancellationToken = default);
    Task<Result> Create(CategoriaRequest categoria);
    Task<Result> Delete(int Id);
    Task<ResultList<CategoriaDto>> Get(string filtro = "");
    Task<Result<CategoriaDto>> GetById(int Id);
    Task<Result> Update(CategoriaRequest categoria);
}

public class CategoriaService(IApplicationDbContext context) : ICategoriaService
{
    public async Task<ResultList<CategoriaDtos>> GetAll(CancellationToken cancellationToken = default)
    {
        var categorias = await context.Categorias
        .Select(x => x.ToDto())
        .ToListAsync(cancellationToken);
        return ResultList<CategoriaDtos>.Success(categorias);
    }
    public async Task<Result> Create(CategoriaRequest categoria)
    {
        try
        {
            var entity = Categoria.Create(categoria.Nombre);
            context.Categorias.Add(entity);
            await context.SaveChangesAsync();
            return Result.Success("Categoria registrado con exito!");
        }
        catch (Exception Ex)
        {
            return Result.Failure($"Error: {Ex.Message}");
        }
    }
    public async Task<Result> Update(CategoriaRequest categoria)
    {
        try
        {
            var entity = context.Categorias.Where(p => p.Id == categoria.Id).FirstOrDefault();
            if (entity == null)
                return Result.Failure($"la categoria '{categoria.Id}' no existe!");
            if (entity.Update(categoria.Nombre))
            {
                await context.SaveChangesAsync();
                return Result.Success("Categoria modificado con exito!");
            }
            return Result.Success("No has realizado ningun cambio!");
        }
        catch (Exception Ex)
        {
            return Result.Failure($"Error: {Ex.Message}");
        }
    }
    public async Task<Result> Delete(int Id)
    {
        try
        {
            var entity = context.Categorias.Where(p => p.Id == Id).FirstOrDefault();
            if (entity == null)
                return Result.Failure($"La categoria '{Id}' no existe!");
            context.Categorias.Remove(entity);
            await context.SaveChangesAsync();
            return Result.Success("Categoria eliminado con exito!");
        }
        catch (Exception Ex)
        {
            return Result.Failure($"Error: {Ex.Message}");
        }
    }
    public async Task<Result<CategoriaDto>> GetById(int Id)
    {
        try
        {
            var entity = await context.Categorias.Where(p => p.Id == Id)
                .Select(p => new CategoriaDto(p.Id, p.Nombre))
                .FirstOrDefaultAsync();
            if (entity == null)
                return Result<CategoriaDto>.Failure($"La categoria '{Id}' no existe!");

            return Result<CategoriaDto>.Success(entity);
        }
        catch (Exception Ex)
        {
            return Result<CategoriaDto>.Failure($"☠Error: {Ex.Message}");
        }
    }
    public async Task<ResultList<CategoriaDto>> Get(string filtro = "")
    {
        try
        {
            var entities = await context.Categorias
                .Where(p => p.Nombre.ToLower().Contains(filtro.ToLower()))
                .Select(p => new CategoriaDto(p.Id, p.Nombre))
                .ToListAsync();
            return ResultList<CategoriaDto>.Success(entities);
        }
        catch (Exception Ex)
        {
            return ResultList<CategoriaDto>.Failure($"☠Error: {Ex.Message}");
        }
    }
}
