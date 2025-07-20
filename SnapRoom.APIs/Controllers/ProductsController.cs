using Microsoft.AspNetCore.Mvc;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Dtos.ProductDtos;
using SnapRoom.Contract.Services;

namespace SnapRoom.APIs.Controllers
{
	[Route("api")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductsController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet("designs")]
		public async Task<IActionResult> GetDesigns(int pageNumber = -1, int pageSize = -1)
		{
			var products = await _productService.GetDesigns(pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy sản phẩm thành công",
				data: products
			));
		}

		[HttpGet("designer/designs")]
		public async Task<IActionResult> GetDesignsForDesigner(int pageNumber = -1, int pageSize = -1)
		{
			var products = await _productService.GetDesignsForDesigner(pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy sản phẩm thành công",
				data: products
			));
		}

		[HttpGet("designerId/{designerId}/designs")]
		public async Task<IActionResult> GetDesignsForDesigner(string designerId, int pageNumber = -1, int pageSize = -1)
		{
			var products = await _productService.GetDesignsByDesignerId(designerId, pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy sản phẩm thành công",
				data: products
			));
		}

		[HttpGet("furnitures")]
		public async Task<IActionResult> GetFurnitures(int pageNumber = -1, int pageSize = -1)
		{
			var products = await _productService.GetFurnitures(pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy sản phẩm thành công",
				data: products
			));
		}

		[HttpGet("designer/furnitures")]
		public async Task<IActionResult> GetFurnituresForDesigner(int pageNumber = -1, int pageSize = -1)
		{
			var products = await _productService.GetFurnituresForDesigner(pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy sản phẩm thành công",
				data: products
			));
		}

		[HttpGet("designerId/{designerId}/furnitures")]
		public async Task<IActionResult> GetFurnituresForDesigner(string designerId, int pageNumber = -1, int pageSize = -1)
		{
			var products = await _productService.GetFurnituresByDesignerId(designerId, pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy sản phẩm thành công",
				data: products
			));
		}


		[HttpGet("products/new")]
		public async Task<IActionResult> GetNewProducts(int pageNumber = -1, int pageSize = -1)
		{
			var products = await _productService.GetNewProducts(pageNumber, pageSize);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy sản phẩm thành công",
				data: products
			));
		}

		[HttpPost("products/new")]
		public async Task<IActionResult> ApproveNewProduct(string id)
		{
			await _productService.ApproveNewProduct(id);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Duyệt sản phẩm thành công",
				data: null
			));
		}


		[HttpPost("designs")]
		public async Task<IActionResult> CreateDesign(DesignCreateDto dto)
		{
			await _productService.CreateDesign(dto);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Tạo bản thiết kế thành công",
				data: null
			));
		}

		[HttpPut("designs/{id}")]
		public async Task<IActionResult> UpdateDesign(string id, DesignUpdateDto dto)
		{
			await _productService.UpdateDesign(id, dto);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Cập nhật bản thiết kế thành công",
				data: null
			));
		}


		[HttpPost("furnitures")]
		public async Task<IActionResult> CreateFurniture(FurnitureCreateDto dto)
		{
			await _productService.CreateFurniture(dto);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Tạo sản phẩm thành công",
				data: null
			));
		}

		[HttpPut("furnitures/{id}")]
		public async Task<IActionResult> UpdateFurniture(string id, FurnitureUpdateDto dto)
		{
			await _productService.UpdateFurniture(id, dto);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Cập nhật sản phẩm nội thất thành công",
				data: null
			));
		}


		[HttpGet("products/{id}")]
		public async Task<IActionResult> GetProductById(string id)
		{
			var product = await _productService.GetProductById(id);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Lấy sản phẩm thành công",
				data: product
			));
		}

		[HttpPut("design-furnitures/{id}")]
		public async Task<IActionResult> UpdateFurnituresInDesign(string id, List<InDesignFurnitureDto> dtos)
		{
			await _productService.UpdateFurnituresInDesign(id, dtos);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Cập nhật thành công",
				data: null
			));
		}


		[HttpPost("products/review")]
		public async Task<IActionResult> Review(string id, string comment, int star)
		{
			await _productService.Review(id, comment, star);

			return Ok(new BaseResponse<object>(
				statusCode: StatusCodeEnum.OK,
				message: "Review sản phẩm",
				data: null
			));
		}

	}
}
