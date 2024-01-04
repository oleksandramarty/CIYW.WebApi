using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Category.Requests;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Validators.Categories;
using MediatR;

namespace CIYW.Mediator.Mediator.Category.Handlers;

public class CreateCategoryCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCategoryCommand, Guid>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Category.Category> categoryRepository;

    public CreateCategoryCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Category.Category> categoryRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.categoryRepository = categoryRepository;
    }

    public async Task<Guid> Handle(CreateOrUpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateCategoryCommand, Guid>(command, () => new CreateOrUpdateCategoryCommandValidator(true));
        Domain.Models.Category.Category category = this.mapper.Map<CreateOrUpdateCategoryCommand, Domain.Models.Category.Category>(command, opts => opts.Items["IsUpdate"] = false);
        category.Id = Guid.NewGuid();
        category.IsActive = true;
        await this.categoryRepository.AddAsync(category, cancellationToken);

        return category.Id;
    }
}