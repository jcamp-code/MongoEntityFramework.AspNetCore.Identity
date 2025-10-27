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
    public class CreateUser : TestBase
    {

        public CreateUser() : base("MongoUserOnlyStore-CreateUser") { }

        [Fact]
        public async Task ReturnsSuccessWithStringId()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            var result = await store.CreateAsync(TestUser.First);

            result.Should().Be(IdentityResult.Success);
        }

        [Fact]
        public async Task CreatesDataWithStringId()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            await store.CreateAsync(TestUser.First);

            context.TestUsers.Any().Should().BeTrue();
            context.TestUsers.Count().Should().Be(1);
            context.TestUsers.FirstOrDefault()?.CustomData.Should().Be("Some Info 1");
        }

        [Fact]
        public async Task DoesNotCreatesDataWithAutoSaveOff()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            store.AutoSaveChanges = false;

            await store.CreateAsync(TestUser.First);

            context.TestUsers.Any().Should().BeFalse();
            context.TestUsers.Count().Should().Be(0);
        }


        [Fact]
        public async Task ReturnsSuccessWithIntId()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUserInt, DbContext, int>(context);

            var result = await store.CreateAsync(TestUserInt.First);

            result.Should().Be(IdentityResult.Success);
        }

        [Fact]
        public async Task CreatesDataWithIntId()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUserInt, DbContext, int>(context);

            await store.CreateAsync(TestUserInt.First);

            context.TestUsersInt.Any().Should().BeTrue();
            context.TestUsersInt.Count().Should().Be(1);
            context.TestUsersInt.FirstOrDefault()?.CustomData.Should().Be("Some Info 1");
        }

        [Fact]
        public async Task ThrowsExceptionWithNull()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            var act = async () =>
            {
                await store.CreateAsync(null);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
