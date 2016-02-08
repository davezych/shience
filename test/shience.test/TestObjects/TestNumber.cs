namespace Shience.Test.TestObjects
{
    internal sealed class TestNumber
    {
        public TestNumber(int number)
        {
            Number = number;
        }

        private int Number { get; }

        public override bool Equals(object obj)
        {
            var otherTestHelper = obj as TestNumber;

            return otherTestHelper?.Number == Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
    }
}