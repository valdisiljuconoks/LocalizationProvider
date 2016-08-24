using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{

    public class RefactoringTests
    {
        [Fact]
        public void GetRefactoredModel()
        {
            var refactoredModelTypes = TypeDiscoveryHelper.GetTypesChildOf<RefactoredModelAttribute>();

            Assert.NotEmpty(refactoredModelTypes);

            var refactoredModelType = refactoredModelTypes.First();

            var refactoredModel = Activator.CreateInstance(refactoredModelType) as RefactoredModelAttribute;

            refactoredModel.DefineChanges();

            var steps = refactoredModel.Steps;

            Assert.NotEmpty(steps);

            //var simplePropertyRefactoring = steps.First(s => s.NewResourceKey == "SimpleProperty");

            //Assert.NotNull(simplePropertyRefactoring);



            // TODO: property with attributes
            // TODO: nested properties (complex)
        }
    }

    public class RefactoredViewModelMigrations : RefactoredModelAttribute
    {
        public override void DefineChanges()
        {
            //For<RefactoredViewModel>().Property("SimpleProperty").Was("OldProperty");
            For<RefactoredViewModel>().Property(t => t.SimpleProperty).Was("OldProperty");
        }
    }

    [LocalizedModel]
    public class RefactoredViewModel
    {
        public RefactoredViewModel()
        {
            SubProperty = new SubViewModel();
        }

        public string SimpleProperty { get; set; }

        [Required]
        [StringLength(5)]
        public string PropertyWithAttributes { get; set; }

        public SubViewModel SubProperty { get; set; }
    }
}
