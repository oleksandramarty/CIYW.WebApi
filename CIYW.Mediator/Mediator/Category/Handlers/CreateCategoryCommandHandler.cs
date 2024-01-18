using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Category.Requests;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Validators.Categories;
using CIYW.Models.Responses.Category;
using MediatR;

namespace CIYW.Mediator.Mediator.Category.Handlers;

public class CreateCategoryCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Domain.Models.Category.Category>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Category.Category> categoryRepository;

    public CreateCategoryCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Category.Category> categoryRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.categoryRepository = categoryRepository;
    }

    public async Task<MappedHelperResponse<CategoryResponse, Domain.Models.Category.Category>> Handle(CreateOrUpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Domain.Models.Category.Category>>(command, () => new CreateOrUpdateCategoryCommandValidator(true));
        Domain.Models.Category.Category category = this.mapper.Map<CreateOrUpdateCategoryCommand, Domain.Models.Category.Category>(command, opts => opts.Items["IsUpdate"] = false);
        category.Id = Guid.NewGuid();
        category.IsActive = true;
        await this.categoryRepository.AddAsync(category, cancellationToken);

        return this.GetMappedHelper<CategoryResponse, Domain.Models.Category.Category>(category);
    }
}