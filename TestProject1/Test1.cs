using System;

namespace TestProject1
{
    public class BaseExceptionForTest : Exception, ISerializableClass, IEquatable<BaseExceptionForTest>
    {
        public BaseExceptionForTest(String m) : base(m) { }

        public bool Equals(BaseExceptionForTest? other) 
      
            =>
            (other !=null)&& 
            (this.GetType() == other.GetType())&&
            (other.Message==Message);

        public string Serialise() => this.ToString();
    }
    public interface ISerializableClass
    {
        String Serialise();
    }

    public abstract class Test<TInput, TReturn, TExcepion> : ISerializableClass
        where TInput : ISerializableClass
        where TReturn : ISerializableClass, IEquatable<TReturn>
        where TExcepion : BaseExceptionForTest, ISerializableClass, IEquatable<TExcepion>

    {
        public TInput TestInput { get; set; }
        public TReturn? ExpectedReturn { get; set; }
        public TExcepion? expectedExcepion { get; set; }
        protected abstract TReturn Sut(TInput input);
        public string Serialise() =>
                 $@"
        {nameof(TestInput)}:{TestInput.Serialise()}
        {nameof(ExpectedReturn)}:{ExpectedReturn.Serialise()}
        {nameof(expectedExcepion)}:{expectedExcepion.Serialise()}";
        public void TestCase()
        {

            //Exception? thrownExcepion; ;
            try
            {
                var ret = Sut(TestInput);
                Assert.AreEqual(ExpectedReturn, ret);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (TExcepion actualException)
            {
                Assert.AreEqual(expectedExcepion, actualException);
            }
            catch (Exception ) {
                throw;
            }
            ;
        }
    }
    public class SimpleException : BaseExceptionForTest
    {
        public SimpleException(string message) : base(message) { }
        

    }
    public class Input : ISerializableClass
    {
        public int Value { get; set; }
        public string Serialise() =>
         Value.ToString();

    }
    public class Return : Input, IEquatable<Return>
    {
        public bool Equals(Return? other) => (other != null) && other.Value == Value;

    }
    public class SimpleTest : Test<Input, Return, BaseExceptionForTest>
    {
        protected override Return Sut(Input input) => (input.Value == 0) ? throw new SimpleException("Упс 0! ") : new Return() { Value = input.Value };

    }

    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void PassedTestCase()
        {
            var test = new SimpleTest() {expectedExcepion=null,TestInput=new Input() { Value =10},ExpectedReturn= new Return() { Value=10}  };
            test.TestCase();
        }
        [TestMethod]
        public void FailedTestcase()
        {
            var test = new SimpleTest() { expectedExcepion = null, TestInput = new Input() { Value = 11 }, ExpectedReturn = new Return() { Value = 10 } };
            test.TestCase();

        }
        [TestMethod]
        public void PassedAbnormalTestcase()
        {
            var test = new SimpleTest() { expectedExcepion = new SimpleException("Упс 0! "), TestInput = new Input() { Value = 0 }, };
            test.TestCase();
        }
        [TestMethod]
        public void FailedAbnormalTestcase()
        {
            var test = new SimpleTest() { expectedExcepion = null, TestInput = new Input() { Value = 0 }, };
            test.TestCase();
        }
        [TestMethod]
        public void FailedAbnormalTestcase1()
        {
            var test = new SimpleTest() { expectedExcepion = new BaseExceptionForTest("Упс 0! "), TestInput = new Input() { Value = 0 }, };
            test.TestCase();
        }
    }
}
