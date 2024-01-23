using AutoMapper;
using CIYW.Domain.Models.Categories;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Categories.Requests;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Validators.Categories;
using CIYW.Models.Responses.Categories;
using MediatR;

namespace CIYW.Mediator.Mediator.Categories.Handlers;

public class CreateCategoryCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Category>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Category> categoryRepository;

    public CreateCategoryCommandHandler(
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
        this.ValidateRequest<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Category>>(command, () => new CreateOrUpdateCategoryCommandValidator(true));
        Category category = this.mapper.Map<CreateOrUpdateCategoryCommand, Category>(command, opts => opts.Items["IsUpdate"] = false);
        category.Id = Guid.NewGuid();
        category.IsActive = true;
        await this.categoryRepository.AddAsync(category, cancellationToken);

        return this.GetMappedHelper<CategoryResponse, Category>(category);
    }
}