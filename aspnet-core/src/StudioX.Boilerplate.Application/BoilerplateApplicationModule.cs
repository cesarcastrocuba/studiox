﻿using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Configuration;
using StudioX.Boilerplate.Roles.Dto;
using StudioX.Boilerplate.Users.Dto;
using StudioX.Domain.Repositories;
using StudioX.Modules;
using StudioX.Reflection.Extensions;

namespace StudioX.Boilerplate
{
    [DependsOn(
        typeof(BoilerplateCoreModule),
        typeof(StudioXAutoMapperModule))]
    public class BoilerplateApplicationModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<BoilerplateAuthorizationProvider>();

            Configuration.Settings.Providers.Add<BoilerplateSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BoilerplateApplicationModule).GetAssembly());

            // TODO: Is there somewhere else to store these, with the dto classes
            Configuration.Modules.StudioXAutoMapper().Configurators.Add(options =>
            {
                // Role and permission
                options.CreateMap<Permission, string>().ConvertUsing(r => r.Name);
                options.CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

                options.CreateMap<CreateRoleInput, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
                options.CreateMap<UpdateRoleInput, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());

                var repository = IocManager.Resolve<IRepository<Role, int>>();

                // User and role
                options.CreateMap<UserRole, string>().ConvertUsing(r =>
                {
                    //TODO: Fix, this seems hacky
                    var role = repository.FirstOrDefault(r.RoleId);
                    return role.DisplayName;
                });

                IocManager.Release(repository);

                options.CreateMap<UpdateUserInput, User>();
                options.CreateMap<UpdateUserInput, User>().ForMember(x => x.Roles, opt => opt.Ignore());

                options.CreateMap<CreateUserInput, User>();
                options.CreateMap<CreateUserInput, User>().ForMember(x => x.Roles, opt => opt.Ignore());
            });
        }
    }
}