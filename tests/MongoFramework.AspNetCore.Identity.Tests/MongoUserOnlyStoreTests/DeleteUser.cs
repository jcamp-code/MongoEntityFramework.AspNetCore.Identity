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
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            await store.CreateAsync(MongoTestUser.First, TestContext.Current.CancellationToken);

            context.TestUsers.Any().Should().BeTrue();

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await context.TestUsers.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            await store.DeleteAsync(user, TestContext.Current.CancellationToken);

            context.TestUsers.Any().Should().BeFalse();
        }

        [Fact]
        public async Task ReturnsSuccessWithValidUser()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            await store.CreateAsync(MongoTestUser.First, TestContext.Current.CancellationToken);

            context.TestUsers.Any().Should().BeTrue();

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await context.TestUsers.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            var result = await store.DeleteAsync(user, TestContext.Current.CancellationToken);

            result.Should().Be(IdentityResult.Success);
        }

        [Fact]
        public async Task ThrowsExceptionWithNull()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.DeleteAsync(null);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
