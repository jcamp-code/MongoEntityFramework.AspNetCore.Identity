using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.MongoUserOnlyStoreTests
{
    public class FindByName : TestBase, IAsyncLifetime
    {

        public FindByName() : base("MongoUserOnlyStore-FindByName") { }

        public async ValueTask InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            await store.CreateAsync(MongoTestUser.First);
            await store.CreateAsync(MongoTestUser.Second);
            await store.CreateAsync(MongoTestUser.Third);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task FindsCorrectUserWithValidUserName()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var result = await store.FindByNameAsync("USER NAME2", TestContext.Current.CancellationToken);

            result.Should().NotBeNull();
            result.UserName.Should().Be("User Name2");
        }


        [Fact]
        public async Task FindsTrackedEntityWithValidUserName()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var tracked = await store.FindByIdAsync(TestIds.UserId2, TestContext.Current.CancellationToken);
            tracked.CustomData = "updated";

            var result = await store.FindByNameAsync("USER NAME2", TestContext.Current.CancellationToken);

            result.Should().BeSameAs(tracked);
            result.CustomData.Should().Be("updated");
        }


        [Fact]
        public async Task ReturnsNullWithInvalidUserName()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var result = await store.FindByNameAsync("none", TestContext.Current.CancellationToken);

            result.Should().BeNull();
        }

        [Fact]
        public async Task ThrowsExceptionWithNull()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.FindByNameAsync(null);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
