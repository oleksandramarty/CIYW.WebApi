using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Category.Requests;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Validators.Categories;
using MediatR;

namespace CIYW.Mediator.Mediator.Category.Handlers;

public class UpdateCategoryCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCategoryCommand, Guid>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Category.Category> categoryRepository;

    public UpdateCategoryCommandHandler(
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
        this.ValidateRequest<CreateOrUpdateCategoryCommand, Guid>(command, () => new CreateOrUpdateCategoryCommandValidator(false));
        Domain.Models.Category.Category category = await this.categoryRepository.GetByIdAsync(command.Id.Value, cancellationToken);

        this.ValidateExist<Domain.Models.Category.Category, Guid?>(category, command.Id);
        
        Domain.Models.Category.Category updatedCategory = this.mapper.Map<CreateOrUpdateCategoryCommand, Domain.Models.Category.Category>(command, category, opts => opts.Items["IsUpdate"] = true);

        await this.categoryRepository.UpdateAsync(updatedCategory, cancellationToken);

        return category.Id;
    }
}