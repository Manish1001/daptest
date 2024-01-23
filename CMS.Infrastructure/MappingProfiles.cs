namespace CMS.Infrastructure
{
    using AutoMapper;
    using Domain.Entities;
    using Shared.Models;

    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            this.CreateMap<AuditLogEntryModel, AuditLog>();
            this.CreateMap<UserTokenModel, UserToken>().ReverseMap();
            this.CreateMap<CafeteriaModel, Cafeteria>();
            this.CreateMap<CafeteriaLocationModel, CafeteriaLocation>();
            this.CreateMap<CustomerModel, Customer>();
            this.CreateMap<CustomerInvoiceModel, CustomerInvoice>();
            this.CreateMap<CustomerInvoiceTransactionModel, CustomerInvoiceTransaction>();
            this.CreateMap<CustomerLocationModel, CustomerLocation>();
            this.CreateMap<CustomerOrderModel, CustomerOrder>();
            this.CreateMap<LookupModel, Lookup>();
            this.CreateMap<LookupTypeModel, LookupType>();
            this.CreateMap<MealModel, Meal>();
            this.CreateMap<MealItemModel, MealItem>();
            this.CreateMap<ProductModel, Product>();
            this.CreateMap<RoleModel, Role>();
            this.CreateMap<UserModel, User>();
            this.CreateMap<UserAccountModel, UserAccount>();
            this.CreateMap<UserCredentialModel, UserCredential>();
            this.CreateMap<UserLocationModel, UserLocation>();
            this.CreateMap<UserPermissionModel, UserPermission>();
        }
    }
}