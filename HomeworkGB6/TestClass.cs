namespace HomeworkGB6
{
    internal class TestClass
    {
        //атрибут можно закомментировать в качестве эксперимента, всё работает
        [CustomName("Integer")]
        public int I { get; set; }

        [CustomName("String")]
        public string? S { get; set; }

        [CustomName("Decimal")]
        public decimal D { get; set; }

        [CustomName("CharArray")]
        public char[]? C { get; set; }
        public TestClass() { }
        public TestClass(int i)
        {
            I = i;
        }
        public TestClass(int i, string s, decimal d, char[] c) : this(i)
        {
            S = s;
            D = d;
            C = c;
        }

    }
}
