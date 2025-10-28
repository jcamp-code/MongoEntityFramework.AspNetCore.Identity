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
    public class FindById : TestBase, IAsyncLifetime
    {

        public FindById() : base("MongoUserOnlyStore-FindById") { }

        public async Task InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var store2 = new MongoUserOnlyStore<MongoTestUserInt, DbContext, int>(context);

            await store.CreateAsync(MongoTestUser.First);
            await store.CreateAsync(MongoTestUser.Second);
            await store.CreateAsync(MongoTestUser.Third);
            await store2.CreateAsync(MongoTestUserInt.First);
            await store2.CreateAsync(MongoTestUserInt.Second);
            await store2.CreateAsync(MongoTestUserInt.Third);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task FindsCorrectUserWithValidStringId()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var result = await store.FindByIdAsync(TestIds.UserId2);

            result.Should().NotBeNull();
            result.UserName.Should().Be("User Name2");
        }

        [Fact]
        public async Task ReturnsNullWithInvalidStringId()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var result = await store.FindByIdAsync("none");

            result.Should().BeNull();
        }

        [Fact]
        public async Task FindsCorrectUserWithValidIntId()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUserInt, DbContext, int>(context);

            var result = await store.FindByIdAsync("2000");

            result.Should().NotBeNull();
            result.UserName.Should().Be("User Name2");
        }

        [Fact]
        public async Task ReturnsNullWithInvalidIntId()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUserInt, DbContext, int>(context);

            var result = await store.FindByIdAsync("1234");

            result.Should().BeNull();
        }

        [Fact]
        public async Task ThrowsExceptionWithNull()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.FindByIdAsync(null);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
