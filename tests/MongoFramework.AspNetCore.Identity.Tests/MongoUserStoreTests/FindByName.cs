using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.MongoUserStoreTests
{
    public class FindByName : TestBase, IAsyncLifetime
    {

        public FindByName() : base("MongoUserStore-FindByName") { }

        public async ValueTask InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            await store.CreateAsync(MongoTestUser.First);
            await store.CreateAsync(MongoTestUser.Second);
            await store.CreateAsync(MongoTestUser.Third);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task FindsCorrectUserWithValidUserName()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var result = await store.FindByNameAsync("USER NAME2", TestContext.Current.CancellationToken);

            result.Should().NotBeNull();
            result.UserName.Should().Be("User Name2");
        }


        [Fact]
        public async Task FindsTrackedEntityWithValidUserName()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);
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
            var store = new MongoUserStore<MongoTestUser>(context);

            var result = await store.FindByNameAsync("none", TestContext.Current.CancellationToken);

            result.Should().BeNull();
        }

    }
}
