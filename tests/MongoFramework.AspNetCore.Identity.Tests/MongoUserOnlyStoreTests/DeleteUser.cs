using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.MongoUserOnlyStoreTests
{
    public class DeleteUser : TestBase
    {

        public DeleteUser() : base("MongoUserOnlyStore-DeleteUser") { }

        [Fact]
        public async Task DeletesDataWithValidUser()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);
            await store.CreateAsync(TestUser.First);

            context.TestUsers.Any().Should().BeTrue();

            context = new TestContext(GetConnection());
            store = new MongoUserOnlyStore<TestUser>(context);
            var user = await context.TestUsers.FirstOrDefaultAsync();

            await store.DeleteAsync(user);

            context.TestUsers.Any().Should().BeFalse();
        }

        [Fact]
        public async Task ReturnsSuccessWithValidUser()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);
            await store.CreateAsync(TestUser.First);

            context.TestUsers.Any().Should().BeTrue();

            context = new TestContext(GetConnection());
            store = new MongoUserOnlyStore<TestUser>(context);
            var user = await context.TestUsers.FirstOrDefaultAsync();

            var result = await store.DeleteAsync(user);

            result.Should().Be(IdentityResult.Success);
        }

        [Fact]
        public Task ThrowsExceptionWithNull()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            var act = () =>
            {
                var _ = store.DeleteAsync(null);
            };
            act.Should().Throw<ArgumentNullException>();
        }

    }
}
