using AutoMapper;
using CIYW.Domain.Models.Categories;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Categories.Requests;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Validators.Categories;
using CIYW.Models.Responses.Categories;
using MediatR;

namespace CIYW.Mediator.Mediator.Categories.Handlers;

public class UpdateCategoryCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Category>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Category> categoryRepository;

    public UpdateCategoryCommandHandler(
        IMapper mapper,
        IGenericRepository<Category> categoryRepository, 
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.categoryRepository = categoryRepository;
    }

    public async Task<MappedHelperResponse<CategoryResponse, Category>> Handle(CreateOrUpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Category>>(command, () => new CreateOrUpdateCategoryCommandValidator(false));
        Category category = await this.categoryRepository.GetByIdAsync(command.Id.Value, cancellationToken);

        this.ValidateExist<Category, Guid?>(category, command.Id);
        
        Category updatedCategory = this.mapper.Map<CreateOrUpdateCategoryCommand, Category>(command, category, opts => opts.Items["IsUpdate"] = true);

        await this.categoryRepository.UpdateAsync(updatedCategory, cancellationToken);

        return this.GetMappedHelper<CategoryResponse, Category>(updatedCategory);
    }
}