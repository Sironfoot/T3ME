using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcLibrary.Bootstrapper;
using AutoMapper;
using Twitterizer;
using T3ME.Domain.Models;
using MvcLibrary.Ioc;
using Castle.MicroKernel.Registration;
using T3ME.Domain.Repositories;
using T3ME.Business.Mapping;

namespace T3ME.Business.StartUp
{
    [BootstrapperPriority(Priority = 4)]
    public class ConfigureAutoMapper : IBootstrapperTask
    {
        public void Execute()
        {
            IocHelper.Container().Register(Component.For<TwitterUserToTweeterTypeConverter>());

            Mapper.Initialize(map => map.ConstructServicesUsing(type => IocHelper.Container().Resolve(type)));

            Mapper.CreateMap<TwitterUser, Tweeter>()
                .ConvertUsing<TwitterUserToTweeterTypeConverter>();

            Mapper.AssertConfigurationIsValid();
        }
    }
}