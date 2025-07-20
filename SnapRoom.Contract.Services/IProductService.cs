using SnapRoom.Common.Base;
using SnapRoom.Contract.Repositories.Dtos.ProductDtos;

namespace SnapRoom.Contract.Services
{
	public interface IProductService
	{
		Task<BasePaginatedList<object>> GetFurnitures(int pageNumber, int pageSize);
		Task<BasePaginatedList<object>> GetDesigns(int pageNumber, int pageSize);
		Task<BasePaginatedList<object>> GetFurnituresForDesigner(int pageNumber, int pageSize);
		Task<BasePaginatedList<object>> GetDesignsForDesigner(int pageNumber, int pageSize);
		Task<BasePaginatedList<object>> GetFurnituresByDesignerId(string designerId, int pageNumber, int pageSize);
		Task<BasePaginatedList<object>> GetDesignsByDesignerId(string designerId, int pageNumber, int pageSize);
		Task<object> GetProductById(string id);
		Task CreateDesign(DesignCreateDto dto);
		Task UpdateDesign(string id, DesignUpdateDto dto);
		Task CreateFurniture(FurnitureCreateDto dto);
		Task UpdateFurniture(string id, FurnitureUpdateDto dto);
		Task UpdateFurnituresInDesign(string id, List<InDesignFurnitureDto> dtos);
		Task<BasePaginatedList<object>> GetNewProducts(int pageNumber, int pageSize);
		Task ApproveNewProduct(string id);
		Task Review(string id, string comment, int star);
	}
}
