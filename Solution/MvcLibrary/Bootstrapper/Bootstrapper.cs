using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MvcLibrary.Ioc;
using Castle.Core;
using Castle.MicroKernel.Registration;
using System.Web.Compilation;
using System.Web;
using System.IO;

namespace MvcLibrary.Bootstrapper
{
    public class Bootstrapper
    {
        static Bootstrapper()
        {
            Type bootStrapperType = typeof(IBootstrapperTask);

            List<Type> tasks = new List<Type>();

            foreach (Assembly assembly in AssemblyLocator.GetBinFolderAssemblies())
            {
                var types = from t in assembly.GetTypes()
                            where bootStrapperType.IsAssignableFrom(t)
                                && t.IsInterface == false
                                && t.IsAbstract == false
                            select t;

                tasks.AddRange(types);
            }

            foreach (Type task in tasks)
            {
                if (!IocHelper.Container().Kernel.HasComponent(task.FullName))
                {
                    IocHelper.Container().AddComponentLifeStyle(task.FullName, task, LifestyleType.Transient);
                }
            }
        }


        public static void Run()
        {
            Run(null);
        }

        public static void Run(IList<Assembly> assemblies)
        {
            Type priorityType = typeof(BootstrapperPriorityAttribute);

            IList<IBootstrapperTask> tasks = IocHelper.Container().ResolveAll<IBootstrapperTask>()
                .OrderBy(t =>
                {
                    Type taskType = t.GetType();

                    BootstrapperPriorityAttribute priority = taskType
                        .GetCustomAttributes(priorityType, false)
                        .SingleOrDefault() as BootstrapperPriorityAttribute;

                    if (priority != null)
                    {
                        return priority.Priority;
                    }

                    return Int32.MaxValue;
                })
                .ToList();

            if (assemblies != null)
            {
                tasks = tasks
                    .Where(t =>
                    {
                        Assembly typesAssembly = t.GetType().Assembly;
                        return assemblies.Any(a => a == typesAssembly);
                    })
                    .ToList();
            }

            foreach (IBootstrapperTask task in tasks)
            {
                task.Execute();
            }
        }
    }
}