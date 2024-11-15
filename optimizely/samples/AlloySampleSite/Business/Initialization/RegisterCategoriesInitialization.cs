using AlloySampleSite.Business.Rendering;
using AlloySampleSite.Models.Categories;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using Microsoft.Extensions.DependencyInjection;

namespace AlloySampleSite.Business.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    public class RegisterCategoriesInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var repo = context.Locate.Advanced.GetService<CategoryRepository>();

            if (repo != null)
            {
                var root = repo.GetRoot();
                var category = new Class1(root);

                if (repo.Get(category.Name) == null)
                {
                    repo.Save(category);
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}
