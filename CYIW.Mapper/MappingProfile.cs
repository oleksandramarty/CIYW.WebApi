using AutoMapper;
using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Category;
using CIYW.Models.Responses.Currency;
using CIYW.Models.Responses.Dictionary;
using CIYW.Models.Responses.Invoice;
using CIYW.Models.Responses.Note;
using CIYW.Models.Responses.Tariff;
using CIYW.Models.Responses.Users;

namespace CYIW.Mapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        this.CreateMap<User, CurrentUserResponse>();

        this.CreateMap<UpdateInvoiceCommand, Invoice>();

        this.CreateMap<UserBalance, CurrentUserResponse>()
            .ForMember(dest => dest.BalanceAmount, opt => opt.MapFrom(src => src.Amount))
            .ForAllMembers(opt => opt.Ignore());

        this.CreateMap<Role, CurrentUserResponse>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Name))
            .ForAllMembers(opt => opt.Ignore());

        this.CreateMap<CreateUserCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsTemporaryPassword, opt => opt.MapFrom(src => true));
        
        CreateMap<CreateOrUpdateNoteCommand, Note>()
            .ConstructUsing((src, ctx) =>
            {
                if (ctx.Items["IsUpdate"] is bool isUpdate && isUpdate)
                {
                    return new Note();
                }
                
                return new Note
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    Name = src.Name,
                    Body = src.Body
                };
            });
        
        this.CreateMap<CreateInvoiceCommand, Invoice>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => DateTime.UtcNow));

        this.CreateMap<Tariff, TariffResponse>();
        this.CreateMap<Currency, CurrencyResponse>();
        this.CreateMap<Note, NoteResponse>();
        this.CreateMap<Category, CategoryResponse>();
        this.CreateMap<BaseFilterQuery, UserInvoicesQuery>();
         
        this.CreateMap<Invoice, BalanceInvoiceResponse>()
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency));

        this.CreateMap<Category, DictionaryItemResponse>()
            .ForMember(dest => dest.Hint, opt => opt.MapFrom(src => src.Description));
        this.CreateMap<Currency, DictionaryItemResponse>()
            .ForMember(dest => dest.Hint, opt => opt.MapFrom(src => src.Symbol));
        this.CreateMap<Tariff, DictionaryItemResponse>()
            .ForMember(dest => dest.Hint, opt => opt.MapFrom(src => src.Description));
        this.CreateMap<Role, DictionaryItemResponse>()
            .ForMember(dest => dest.Hint, opt => opt.MapFrom(src => src.NormalizedName));
    }
}