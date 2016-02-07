namespace DbLocalizationProvider.Tests
{
    public class SubKeyModel
    {
        public SubKeyModel()
        {
            EvenMoreComplex = new DeeperSubKeyModel();
        }

        public int AnotherProperty { get; set; }
        public DeeperSubKeyModel EvenMoreComplex { get; set; }
    }
}