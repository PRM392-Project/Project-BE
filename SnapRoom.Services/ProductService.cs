using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Common.Utils;
using SnapRoom.Contract.Repositories.Dtos.ProductDtos;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;
using static SnapRoom.Common.Base.BaseException;

namespace SnapRoom.Services
{
	public class ProductService : IProductService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IAuthService _authService;

		public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authenticationService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_authService = authenticationService;
		}

		public async Task<BasePaginatedList<object>> GetDesigns(int pageNumber, int pageSize)
		{

			List<Product> query = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Design != null).ToListAsync();

			var responseItems = query.Select(x => new
			{
				x.Id,
				x.Name,
				x.Description,
				x.Rating,
				x.Price,
				PrimaryImage = new { x.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
				Categories = x.ProductCategories?.Select(pc => new
				{
					Id = pc.CategoryId,
					Name = _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId)
						.Select(c => c.Name).FirstOrDefault()
				}).ToList(),
			}).ToList();

			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}

		public async Task<BasePaginatedList<object>> GetDesignsForDesigner(int pageNumber, int pageSize)
		{
			string designerId = _authService.GetCurrentAccountId();

			return await GetDesignsByDesignerId(designerId, pageNumber, pageSize);
		}

		public async Task<BasePaginatedList<object>> GetDesignsByDesignerId(string designerId, int pageNumber, int pageSize)
		{
			Account? designer = await _unitOfWork.GetRepository<Account>().Entities.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer && a.DeletedBy == null).FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			List<Product> query = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Design != null && p.DesignerId == designerId).ToListAsync();

			var responseItems = query.Select(x => new
			{
				x.Id,
				x.Name,
				x.Description,
				x.Rating,
				x.Price,
				PrimaryImage = new { x.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
				Categories = x.ProductCategories?.Select(pc => new
				{
					Id = pc.CategoryId,
					Name = _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId)
						.Select(c => c.Name).FirstOrDefault()
				}).ToList(),
			}).ToList();

			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}


		public async Task<BasePaginatedList<object>> GetFurnitures(int pageNumber, int pageSize)
		{

			List<Product> query = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Furniture != null).ToListAsync();

			var responseItems = query.Select(x => new
			{
				x.Id,
				x.Name,
				x.Description,
				x.Rating,
				x.Price,
				PrimaryImage = new { x.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
				Categories = x.ProductCategories?.Select(pc => new
				{
					Id = pc.CategoryId,
					Name = _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId)
						.Select(c => c.Name).FirstOrDefault()
				}).ToList(),

			}).ToList();

			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}

		public async Task<BasePaginatedList<object>> GetFurnituresForDesigner(int pageNumber, int pageSize)
		{
			string designerId = _authService.GetCurrentAccountId();

			return await GetFurnituresByDesignerId(designerId, pageNumber, pageSize);
		}

		public async Task<BasePaginatedList<object>> GetFurnituresByDesignerId(string designerId, int pageNumber, int pageSize)
		{

			Account? designer = await _unitOfWork.GetRepository<Account>().Entities.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer && a.DeletedBy == null).FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			List<Product> query = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Furniture != null && p.DesignerId == designerId).ToListAsync();

			var responseItems = query.Select(x => new
			{
				x.Id,
				x.Name,
				x.Description,
				x.Rating,
				x.Price,
				PrimaryImage = new { x.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
				Categories = x.ProductCategories?.Select(pc => new
				{
					Id = pc.CategoryId,
					Name = _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId)
						.Select(c => c.Name).FirstOrDefault()
				}).ToList(),

			}).ToList();

			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}


		public async Task<BasePaginatedList<object>> GetNewProducts(int pageNumber, int pageSize)
		{
			List<Product> query = await _unitOfWork.GetRepository<Product>().Entities
	.			Where(p => !p.Approved).ToListAsync();

			var responseItems = query.Select(x => new
			{
				x.Id,
				x.Name,
				x.Description,
				x.Rating,
				x.Price,
				PrimaryImage = new { x.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
				Categories = x.ProductCategories?.Select(pc => new
				{
					Id = pc.CategoryId,
					Name = _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId)
						.Select(c => c.Name).FirstOrDefault()
				}).ToList(),

			}).ToList();

			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}

		public async Task ApproveNewProduct(string id)
		{
			Product? product = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Id == id && !p.Approved && p.DeletedBy == null).FirstOrDefaultAsync();

			if (product == null)
			{
				throw new ErrorException(404, "", "Mã sản phẩm không hợp lệ");
			}

			product.Approved = true;
			await _unitOfWork.SaveAsync();
		}


		public async Task CreateDesign(DesignCreateDto dto)
		{
			string designerId = _authService.GetCurrentAccountId();

			Account? designer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer)
				.FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			Category? style = await _unitOfWork.GetRepository<Category>().Entities
				.Where(c => c.Id == dto.StyleId && c.Style && c.DeletedBy == null)
				.FirstOrDefaultAsync();

			if (style == null)
			{
				throw new ErrorException(404, "", "Danh mục phong cách không hợp lệ");
			}

			Product design = new()
			{
				Name = dto.Name,
				Description = dto.Description,
				DesignerId = designerId,
				Price = dto.Price,
				Active = dto.Active,
				Rating = 0.0,
				ProductCategories = new List<ProductCategory>()
			};

			// Attach the design entity
			design.Design = new Design
			{
				Id = design.Id
			};

			design.ProductCategories.Add(new ProductCategory
			{
				ProductId = design.Id,
				CategoryId = dto.StyleId
			});

			// Prepare categories
			foreach (var categoryId in dto.CategoryIds.Distinct()) // prevent duplicates just in case
			{
				var categoryExists = await _unitOfWork.GetRepository<Category>().Entities
					.AnyAsync(c => c.Id == categoryId && !c.Style && c.DeletedBy == null);

				if (!categoryExists)
				{
					throw new ErrorException(404, "", "Danh mục không hợp lệ");
				}

				design.ProductCategories.Add(new ProductCategory
				{
					ProductId = design.Id,
					CategoryId = categoryId
				});
			}

			try
			{
				// Handle primary image
				string primaryImageUrl = await CoreHelper.UploadImage(dto.PrimaryImage);
				Image primaryImage = new Image
				{
					ProductId = design.Id,
					ImageSource = primaryImageUrl,
					IsPrimary = true
				};
				await _unitOfWork.GetRepository<Image>().InsertAsync(primaryImage);

				if (dto.Images != null && dto.Images.Any())
				{
					foreach (var imageFile in dto.Images)
					{
						string imageUrl = await CoreHelper.UploadImage(imageFile);
						Image image = new Image
						{
							ProductId = design.Id,
							ImageSource = imageUrl,
							IsPrimary = false
						};
						await _unitOfWork.GetRepository<Image>().InsertAsync(image);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ErrorException(500, "", "Lỗi khi tạo thiết kế: " + ex.Message);
			}


			await _unitOfWork.GetRepository<Product>().InsertAsync(design);
			await _unitOfWork.SaveAsync();
		}

		public async Task UpdateDesign(string id, DesignUpdateDto dto)
		{
			string designerId = _authService.GetCurrentAccountId();

			Account? designer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer)
				.FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			Product? design = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Id == id && p.Design != null && p.DeletedBy == null && p.DesignerId == designerId).FirstOrDefaultAsync();

			if (design == null)
			{
				throw new ErrorException(404, "", "Thiết kế không hợp lệ");
			}

			design.Name = dto.Name ?? design.Name;
			design.Price = dto.Price ?? design.Price;
			design.Description = dto.Description ?? design.Description;
			design.Active = dto.Active ?? design.Active;

			if (dto.StyleId != null)
			{
				Category? style = await _unitOfWork.GetRepository<Category>().Entities
					.Where(c => c.Id == dto.StyleId && c.Style && c.DeletedBy == null)
					.FirstOrDefaultAsync();

				if (style == null)
				{
					throw new ErrorException(404, "", "Danh mục phong cách không hợp lệ");
				}
				bool check = true;
				foreach(var productCategory in design.ProductCategories!)
				{
					if (productCategory.CategoryId == dto.StyleId)
					{
						check = false;
						break;
					}
				}
				if (check) 
				{
					ProductCategory? existingStyle = design.ProductCategories?.FirstOrDefault(pc => pc.ProductId == design.Id && pc.Category.Style);

					if (existingStyle != null)
						await _unitOfWork.GetRepository<ProductCategory>().DeleteAsync(existingStyle);

					ProductCategory newStyle = new ProductCategory
					{
						ProductId = design.Id,
						CategoryId = dto.StyleId
					};
					await _unitOfWork.GetRepository<ProductCategory>().InsertAsync(newStyle);
				}
			}

			if (dto.CategoryIds != null && dto.CategoryIds.Any())
			{
				// Remove old categories that are not in the new list
				var existingCategories = design.ProductCategories?.Where(pc => !pc.Category.Style).ToList() ?? new List<ProductCategory>();
				foreach (var category in existingCategories)
				{
					if (!dto.CategoryIds.Contains(category.CategoryId))
					{
						await _unitOfWork.GetRepository<ProductCategory>().DeleteAsync(category);
					}
				}
				// Add new categories
				foreach (var categoryId in dto.CategoryIds.Distinct()) // prevent duplicates just in case
				{
					var categoryExists = await _unitOfWork.GetRepository<Category>().Entities
						.AnyAsync(c => c.Id == categoryId && !c.Style && c.DeletedBy == null);
					if (!categoryExists)
					{
						throw new ErrorException(404, "", "Danh mục không hợp lệ");
					}
					if (!existingCategories.Any(pc => pc.CategoryId == categoryId))
					{
						ProductCategory newCategory = new ProductCategory
						{
							ProductId = design.Id,
							CategoryId = categoryId
						};
						await _unitOfWork.GetRepository<ProductCategory>().InsertAsync(newCategory);
					}
				}
			}

			try
			{
				// Handle primary image
				if (dto.PrimaryImage != null)
				{
					string primaryImageUrl = await CoreHelper.UploadImage(dto.PrimaryImage);

					Image? primaryImage = design.Images?.FirstOrDefault(img => img.IsPrimary);

					if (primaryImage != null)
					{
						// DELETE the old image
						await CoreHelper.DeleteImage(primaryImage.ImageSource);
						await _unitOfWork.GetRepository<Image>().DeleteAsync(primaryImage);
					}

					// ADD the new primary image
					var newPrimaryImage = new Image
					{
						ProductId = design.Id,
						ImageSource = primaryImageUrl,
						IsPrimary = true
					};
					await _unitOfWork.GetRepository<Image>().InsertAsync(newPrimaryImage);
				}
				if (dto.Images != null && dto.Images.Any())
				{
					// STEP 1: Delete existing non-primary images from DB and Blob
					var existingImages = design.Images?.Where(img => !img.IsPrimary).ToList();
					if (existingImages != null && existingImages.Any())
					{
						foreach (var oldImage in existingImages)
						{
							await CoreHelper.DeleteImage(oldImage.ImageSource); // Remove from Blob
							await _unitOfWork.GetRepository<Image>().DeleteAsync(oldImage); // Remove from DB
						}
					}

					// STEP 2: Upload and insert new images
					foreach (var imageFile in dto.Images)
					{
						string imageUrl = await CoreHelper.UploadImage(imageFile);
						var newImage = new Image
						{
							ProductId = design.Id,
							ImageSource = imageUrl,
							IsPrimary = false
						};
						await _unitOfWork.GetRepository<Image>().InsertAsync(newImage);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ErrorException(500, "", "Lỗi khi cập nhật thiết kế: " + ex.Message);
			}
			await _unitOfWork.SaveAsync();
		}

		public async Task UpdateFurniture(string id, FurnitureUpdateDto dto)
		{
			string designerId = _authService.GetCurrentAccountId();

			Account? designer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer)
				.FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			Product? furniture = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Id == id && p.Furniture != null && p.DeletedBy == null && p.DesignerId == designerId).FirstOrDefaultAsync();

			if (furniture == null)
			{
				throw new ErrorException(404, "", "Sản phẩm nội thất không hợp lệ");
			}

			furniture.Name = dto.Name ?? furniture.Name;
			furniture.Price = dto.Price ?? furniture.Price;
			furniture.Description = dto.Description ?? furniture.Description;
			furniture.Active = dto.Active ?? furniture.Active;

			if (dto.StyleId != null)
			{
				Category? style = await _unitOfWork.GetRepository<Category>().Entities
					.Where(c => c.Id == dto.StyleId && c.Style && c.DeletedBy == null)
					.FirstOrDefaultAsync();

				if (style == null)
				{
					throw new ErrorException(404, "", "Danh mục phong cách không hợp lệ");
				}
				bool check = true;
				foreach (var productCategory in furniture.ProductCategories!)
				{
					if (productCategory.CategoryId == dto.StyleId)
					{
						check = false;
						break;
					}
				}
				if (check)
				{
					ProductCategory? existingStyle = furniture.ProductCategories?.FirstOrDefault(pc => pc.ProductId == furniture.Id && pc.Category.Style);

					if (existingStyle != null)
						await _unitOfWork.GetRepository<ProductCategory>().DeleteAsync(existingStyle);

					ProductCategory newStyle = new ProductCategory
					{
						ProductId = furniture.Id,
						CategoryId = dto.StyleId
					};
					await _unitOfWork.GetRepository<ProductCategory>().InsertAsync(newStyle);
				}
			}

			if (dto.CategoryIds != null && dto.CategoryIds.Any())
			{
				// Remove old categories that are not in the new list
				var existingCategories = furniture.ProductCategories?.Where(pc => !pc.Category.Style).ToList() ?? new List<ProductCategory>();
				foreach (var category in existingCategories)
				{
					if (!dto.CategoryIds.Contains(category.CategoryId))
					{
						await _unitOfWork.GetRepository<ProductCategory>().DeleteAsync(category);
					}
				}
				// Add new categories
				foreach (var categoryId in dto.CategoryIds.Distinct()) // prevent duplicates just in case
				{
					var categoryExists = await _unitOfWork.GetRepository<Category>().Entities
						.AnyAsync(c => c.Id == categoryId && !c.Style && c.DeletedBy == null);
					if (!categoryExists)
					{
						throw new ErrorException(404, "", "Danh mục không hợp lệ");
					}
					if (!existingCategories.Any(pc => pc.CategoryId == categoryId))
					{
						ProductCategory newCategory = new ProductCategory
						{
							ProductId = furniture.Id,
							CategoryId = categoryId
						};
						await _unitOfWork.GetRepository<ProductCategory>().InsertAsync(newCategory);
					}
				}
			}

			try
			{
				// Handle primary image
				if (dto.PrimaryImage != null)
				{
					string primaryImageUrl = await CoreHelper.UploadImage(dto.PrimaryImage);

					Image? primaryImage = furniture.Images?.FirstOrDefault(img => img.IsPrimary);

					if (primaryImage != null)
					{
						// DELETE the old image
						await CoreHelper.DeleteImage(primaryImage.ImageSource);
						await _unitOfWork.GetRepository<Image>().DeleteAsync(primaryImage);
					}

					// ADD the new primary image
					var newPrimaryImage = new Image
					{
						ProductId = furniture.Id,
						ImageSource = primaryImageUrl,
						IsPrimary = true
					};
					await _unitOfWork.GetRepository<Image>().InsertAsync(newPrimaryImage);
				}
				if (dto.Images != null && dto.Images.Any())
				{
					// STEP 1: Delete existing non-primary images from DB and Blob
					var existingImages = furniture.Images?.Where(img => !img.IsPrimary).ToList();
					if (existingImages != null && existingImages.Any())
					{
						foreach (var oldImage in existingImages)
						{
							await CoreHelper.DeleteImage(oldImage.ImageSource); // Remove from Blob
							await _unitOfWork.GetRepository<Image>().DeleteAsync(oldImage); // Remove from DB
						}
					}

					// STEP 2: Upload and insert new images
					foreach (var imageFile in dto.Images)
					{
						string imageUrl = await CoreHelper.UploadImage(imageFile);
						var newImage = new Image
						{
							ProductId = furniture.Id,
							ImageSource = imageUrl,
							IsPrimary = false
						};
						await _unitOfWork.GetRepository<Image>().InsertAsync(newImage);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ErrorException(500, "", "Lỗi khi cập nhật nội thất: " + ex.Message);
			}
			await _unitOfWork.SaveAsync();

		}

		public async Task CreateFurniture(FurnitureCreateDto dto)
		{
			string designerId = _authService.GetCurrentAccountId();

			Account? designer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == designerId && a.Role == RoleEnum.Designer)
				.FirstOrDefaultAsync();

			if (designer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			Category? style = await _unitOfWork.GetRepository<Category>().Entities
				.Where(c => c.Id == dto.StyleId && c.Style && c.DeletedBy == null)
				.FirstOrDefaultAsync();

			if (style == null)
			{
				throw new ErrorException(404, "", "Danh mục phong cách không hợp lệ");
			}

			Product furniture = new()
			{
				Name = dto.Name,
				Description = dto.Description,
				ParentDesignId = dto.ParentDesignId,
				DesignerId = designerId,
				Price = dto.Price,
				Active = dto.Active,
				Rating = 0.0,
				ProductCategories = new List<ProductCategory>()
			};

			// Attach the design entity
			furniture.Furniture = new Furniture
			{
				Id = furniture.Id
			};

			furniture.ProductCategories.Add(new ProductCategory
			{
				ProductId = furniture.Id,
				CategoryId = dto.StyleId
			});

			// Prepare categories
			foreach (var categoryId in dto.CategoryIds.Distinct()) // prevent duplicates just in case
			{
				var categoryExists = await _unitOfWork.GetRepository<Category>().Entities
					.AnyAsync(c => c.Id == categoryId && !c.Style && c.DeletedBy == null);

				if (!categoryExists)
				{
					throw new ErrorException(404, "", "Danh mục không hợp lệ");
				}

				furniture.ProductCategories.Add(new ProductCategory
				{
					ProductId = furniture.Id,
					CategoryId = categoryId
				});
			}

			try
			{
				// Handle primary image
				string primaryImageUrl = await CoreHelper.UploadImage(dto.PrimaryImage);
				Image primaryImage = new Image
				{
					ProductId = furniture.Id,
					ImageSource = primaryImageUrl,
					IsPrimary = true
				};
				await _unitOfWork.GetRepository<Image>().InsertAsync(primaryImage);

				if (dto.Images != null && dto.Images.Any())
				{
					foreach (var imageFile in dto.Images)
					{
						string imageUrl = await CoreHelper.UploadImage(imageFile);
						Image image = new Image
						{
							ProductId = furniture.Id,
							ImageSource = imageUrl,
							IsPrimary = false
						};
						await _unitOfWork.GetRepository<Image>().InsertAsync(image);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ErrorException(500, "", "Lỗi khi tạo thiết kế: " + ex.Message);
			}


			await _unitOfWork.GetRepository<Product>().InsertAsync(furniture);
			await _unitOfWork.SaveAsync();
		}


		public async Task<object> GetProductById(string id)
		{

			Product? product = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Id == id && p.DeletedBy == null).FirstOrDefaultAsync();

			if (product == null)
			{
				throw new ErrorException(404, "", "Mã sản phẩm không hợp lệ");
			}

			if (product.Design != null)
			{
				return await GetDesign(product);
			}
			else if (product.Furniture != null)
			{
				return await GetFurniture(product);
			}

			throw new ErrorException(404, "", "Sản phẩm chưa được phân loại");
		}

		private async Task<object> GetFurniture(Product product)
		{
			await _unitOfWork.SaveAsync();

			var responseItem = new
			{
				product.Id,
				Type = "Furniture",
				product.Name,
				product.Description,
				product.Rating,
				product.Price,
				product.Active,
				product.Approved,
				Designer = new { product.Designer?.Id, product.Designer?.Name, product.Designer?.AvatarSource },
				ParentDesign = new { product.ParentDesign?.Id, product.ParentDesign?.Name },
				PrimaryImage = new { product.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
				Images = product.Images?
					.Where(img => !img.IsPrimary)
					.Select(img => new { img.ImageSource})
					.ToList(),
				Style = product.ProductCategories?
					.Select(pc => _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId)
						.Select(c => new { c.Id, c.Name, c.Style })
						.FirstOrDefault())
					.Where(c => c != null)
					.OrderByDescending(c => c?.Style)
					.Select(c => new { c?.Id, c?.Name })
					.FirstOrDefault(),
				Categories = product.ProductCategories?
					.Select(pc => _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId && !c.Style)
						.Select(c => new
						{
							c.Id,
							c.Name
						})
						.FirstOrDefault())
					.Where(c => c != null)
					.ToList(),
				Reviews = product.ProductReviews?
					.Select(pr => new
					{
						pr.Comment,
						pr.Star,
						Customer = new { pr.Customer?.Id, pr.Customer?.Name },
						Date = pr.Time.ToString("dd/MM/yyyy")
					})
					.OrderByDescending(pr => pr.Date)
					.ToList()
			};


			return responseItem;
		}

		private async Task<object> GetDesign(Product product)
		{
			await _unitOfWork.SaveAsync();

			var responseItem = new
			{
				product.Id,
				Type = "Design",
				product.Name,
				product.Description,
				product.Rating,
				product.Price,
				product.Active,
				product.Approved,
				Designer = new { product.Designer?.Id, product.Designer?.Name, product.Designer?.AvatarSource },
				Furnitures = product.Furnitures?
					.Select(f => new
					{
						f.Id,
						f.Name,
						PrimaryImage = new { f.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
						f.InDesignQuantity
					})
					.ToList(),
				PrimaryImage = new { product.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageSource },
				Images = product.Images?
					.Where(img => !img.IsPrimary)
					.Select(img => new { img.ImageSource })
					.ToList(),
				Style = product.ProductCategories?
					.Select(pc => _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId)
						.Select(c => new { c.Id, c.Name, c.Style })
						.FirstOrDefault())
					.Where(c => c != null)
					.OrderByDescending(c => c?.Style)
					.Select(c => new { c?.Id, c?.Name })
					.FirstOrDefault(),
				Categories = product.ProductCategories?
					.Select(pc => _unitOfWork.GetRepository<Category>().Entities
						.Where(c => c.Id == pc.CategoryId && !c.Style)
						.Select(c => new
						{
							c.Id,
							c.Name
						})
						.FirstOrDefault())
					.Where(c => c != null)
					.ToList(),
				Reviews = product.ProductReviews?
					.Select(pr => new
					{
						pr.Comment,
						pr.Star,
						Customer = new { pr.Customer?.Id, pr.Customer?.Name },
						Date = pr.Time.ToString("dd/MM/yyyy")
					})
					.OrderByDescending(pr => pr.Date)
					.ToList()
			};


			return responseItem;
		}


		public async Task UpdateFurnituresInDesign(string id, List<InDesignFurnitureDto> dtos)
		{
			Product? design = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Id == id && p.Design != null && p.DeletedBy == null).FirstOrDefaultAsync();

			if (design == null)
			{
				throw new ErrorException(404, "", "Thiết kế không hợp lệ");
			}

			foreach (var dto in dtos)
			{
				Product? furniture = await _unitOfWork.GetRepository<Product>().Entities
					.Where(p => p.Id == dto.Id && p.Furniture != null && p.ParentDesignId == id && p.DeletedBy == null).FirstOrDefaultAsync();
				if (furniture == null)
				{
					throw new ErrorException(404, "", "Nội thất không hợp lệ");
				}
				if (dto.Quantity < 0)
				{
					throw new ErrorException(400, "", "Số lượng trong thiết kế không thể âm");
				}
				furniture.InDesignQuantity = dto.Quantity;
			}

			await _unitOfWork.SaveAsync();
		}

		public async Task Review(string id, string comment, int star)
		{
			string customerId = _authService.GetCurrentAccountId();

			Account? customer = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Id == customerId && a.Role == RoleEnum.Customer)
				.FirstOrDefaultAsync();

			if (customer == null)
			{
				throw new ErrorException(404, "", "Tài khoản không hợp lệ");
			}

			Product? product = await _unitOfWork.GetRepository<Product>().Entities
				.Where(p => p.Id == id && p.DeletedBy == null).FirstOrDefaultAsync();

			if (product == null)
			{
				throw new ErrorException(404, "", "Sản phẩm không hợp lệ");
			}
			
			if (product.ProductReviews?.Any(pr => pr.CustomerId == customerId) == true)
			{
				throw new ErrorException(400, "", "Bạn đã đánh giá sản phẩm này rồi");
			}

			ProductReview review = new ProductReview
			{
				ProductId = product.Id,
				CustomerId = customerId,
				Comment = comment,
				Star = star,
				Time = CoreHelper.SystemTimeNow
			};
			await _unitOfWork.GetRepository<ProductReview>().InsertAsync(review);
			await _unitOfWork.SaveAsync();

			double rating = 0;

			foreach(var productReviews in product.ProductReviews!)
			{
				rating+= productReviews.Star;
			}
			rating /= product.ProductReviews!.Count;
			product.Rating = rating;

			await _unitOfWork.SaveAsync();
		}
	}
}