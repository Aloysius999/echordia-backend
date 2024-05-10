using Ech.Common.Crypto;

namespace Ech.Common.Test
{
    public class HashTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestVerify()
        {
            string password1 = "eChordia99&%";
            string hashPassword1 = "$2b$10$vke7HX0FvB7DZFZVhGd4o.OUMWQUcDnh4Q2CHgFCdzLClakdTyiNK";

            bool res1 = Hash.Verify(password1, hashPassword1);
            Assert.IsTrue(res1, "Password verify failed");

            string password2 = "Hello12@£";
            string hashPassword2 = "$2b$10$tgDqF9OAV7kwLZIM9dpc2u3B0PoEOgGyvelFfQ.mLGASlaS/iFYI2";

            bool res2 = Hash.Verify(password2, hashPassword2);
            Assert.IsTrue(res2, "Password verify failed");

            Assert.Pass();
        }
    }
}