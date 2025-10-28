﻿using MongoDB.Bson;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses
{
    public class MongoTestUser : MongoIdentityUser
    {
        public string CustomData { get; set; }

        public static MongoTestUser First => new MongoTestUser
        {
            Id = TestIds.UserId1,
            Email = "test1@testing.com",
            UserName = "User Name1",
            CustomData = "Some Info 1",
            NormalizedEmail = "TEST1@TESTING.COM",
            NormalizedUserName = "USER NAME1"
        };
        public static MongoTestUser Second => new MongoTestUser
        {
            Id = TestIds.UserId2,
            Email = "test2@testing.com",
            UserName = "User Name2",
            CustomData = "Some Info 2",
            NormalizedEmail = "TEST2@TESTING.COM",
            NormalizedUserName = "USER NAME2"
        };
        public static MongoTestUser Third => new MongoTestUser
        {
            Id = TestIds.UserId3,
            Email = "test3@testing.com",
            UserName = "User Name3",
            CustomData = "Some Info 3",
            NormalizedEmail = "TEST3@TESTING.COM",
            NormalizedUserName = "USER NAME3"
        };
    }
}
