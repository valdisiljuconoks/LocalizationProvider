using DbLocalizationProvider.Internal;
using Xunit;

namespace DbLocalizationProvider.Tests.ComparerTests
{
    public class TranslationComparerTests
    {
        [Fact]
        public void TwoDifferentTranslations_InvariantCulture_NoChangesDetected()
        {
            var sut = new TranslationComparer(true);

            var result = sut.Equals(new LocalizationResourceTranslation
                                    {
                                        Language = "",
                                        Value = "Value 1"
                                    },
                                    new LocalizationResourceTranslation
                                    {
                                        Language = "",
                                        Value = "Value 2"
                                    });

            Assert.True(result);
        }

        [Fact]
        public void TwoDifferentTranslations_InvariantCulture_ComparerShouldNotIgnore_ChangesDetected()
        {
            var sut = new TranslationComparer(false);

            var result = sut.Equals(new LocalizationResourceTranslation
                                    {
                                        Language = "",
                                        Value = "Value 1"
                                    },
                                    new LocalizationResourceTranslation
                                    {
                                        Language = "",
                                        Value = "Value 2"
                                    });

            Assert.False(result);
        }
    }
}
