﻿using System;
using System.Data.Entity;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.Reflection;
using EntityFramework.DynamicFilters;

namespace StudioX.EntityFramework.Uow
{
    public class EfDynamicFiltersUnitOfWorkFilterExecuter : IEfUnitOfWorkFilterExecuter
    {
        public void ApplyDisableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            foreach (var activeDbContext in unitOfWork.As<EfUnitOfWork>().GetAllActiveDbContexts())
            {
                activeDbContext.DisableFilter(filterName);
            }
        }

        public void ApplyEnableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            foreach (var activeDbContext in unitOfWork.As<EfUnitOfWork>().GetAllActiveDbContexts())
            {
                activeDbContext.EnableFilter(filterName);
            }
        }

        public void ApplyFilterParameterValue(IUnitOfWork unitOfWork, string filterName, string parameterName, object value)
        {
            foreach (var activeDbContext in unitOfWork.As<EfUnitOfWork>().GetAllActiveDbContexts())
            {
                if (TypeHelper.IsFunc<object>(value))
                {
                    activeDbContext.SetFilterScopedParameterValue(filterName, parameterName, (Func<object>)value);
                }
                else
                {
                    activeDbContext.SetFilterScopedParameterValue(filterName, parameterName, value);
                }
            }
        }

        public void ApplyCurrentFilters(IUnitOfWork unitOfWork, DbContext dbContext)
        {
            foreach (var filter in unitOfWork.Filters)
            {
                if (filter.IsEnabled)
                {
                    dbContext.EnableFilter(filter.FilterName);
                }
                else
                {
                    dbContext.DisableFilter(filter.FilterName);
                }

                foreach (var filterParameter in filter.FilterParameters)
                {
                    if (TypeHelper.IsFunc<object>(filterParameter.Value))
                    {
                        dbContext.SetFilterScopedParameterValue(filter.FilterName, filterParameter.Key, (Func<object>)filterParameter.Value);
                    }
                    else
                    {
                        dbContext.SetFilterScopedParameterValue(filter.FilterName, filterParameter.Key, filterParameter.Value);
                    }
                }
            }
        }
    }
}
