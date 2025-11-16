using HotChocolate.Types;
using JurisSeeker.Domain.Entities;

namespace JurisSeeker.Shared.GraphQL;

public class UserTypes : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Field(t => t.Id)
            .Description("Уникальный идентификатор пользователя")
            .Type<IdType>();
        
        descriptor.Field(u => u.Email)
            .Description("Email адрес пользователя")
            .Type<NonNullType<StringType>>();
        
        descriptor.Ignore(u => u.Password);
    }
}