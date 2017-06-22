﻿using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.EntityFramework;
using StudioX.EntityFramework.Repositories;
using StudioX.EntityFrameworkCore.Configuration;
using StudioX.EntityFrameworkCore.Repositories;
using StudioX.EntityFrameworkCore.Uow;
using StudioX.Modules;
using StudioX.Orm;
using StudioX.Reflection;
using StudioX.Reflection.Extensions;

namespace StudioX.EntityFrameworkCore
{
    /// <summary>
    ///     This module is used to implement "Data Access Layer" in EntityFramework.
    /// </summary>
    [DependsOn(typeof(StudioXEntityFrameworkCommonModule))]
    public class StudioXEntityFrameworkCoreModule : StudioXModule
    {
        private readonly ITypeFinder typeFinder;

        public StudioXEntityFrameworkCoreModule(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            IocManager.Register<IStudioXEfCoreConfiguration, StudioXEfCoreConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXEntityFrameworkCoreModule).GetAssembly());

            IocManager.IocContainer.Register(
                Component.For(typeof(IDbContextProvider<>))
                    .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                    .LifestyleTransient()
            );

            RegisterGenericRepositoriesAndMatchDbContexes();
        }

        private void RegisterGenericRepositoriesAndMatchDbContexes()
        {
            var dbContextTypes =
                typeFinder.Find(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    return typeInfo.IsPublic &&
                           !typeInfo.IsAbstract &&
                           typeInfo.IsClass &&
                           typeof(StudioXDbContext).IsAssignableFrom(type);
                });

            if (dbContextTypes.IsNullOrEmpty())
            {
                Logger.Warn("No class found derived from StudioXDbContext.");
                return;
            }

            using (var scope = IocManager.CreateScope())
            {
                foreach (var dbContextType in dbContextTypes)
                {
                    Logger.Debug("Registering DbContext: " + dbContextType.AssemblyQualifiedName);
                    scope.Resolve<IEfGenericRepositoryRegistrar>()
                        .RegisterForDbContext(dbContextType, IocManager, EfCoreAutoRepositoryTypes.Default);

                    IocManager.IocContainer.Register(
                        Component.For<ISecondaryOrmRegistrar>()
                            .Named(Guid.NewGuid().ToString("N"))
                            .Instance(new EfCoreBasedSecondaryOrmRegistrar(dbContextType,
                                scope.Resolve<IDbContextEntityFinder>()))
                            .LifestyleTransient()
                    );
                }

                scope.Resolve<IDbContextTypeMatcher>().Populate(dbContextTypes);
            }
        }
    }
}