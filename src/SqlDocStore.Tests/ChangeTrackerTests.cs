﻿namespace SqlDocStore.Tests
{
    using System;
    using System.Linq;
    using Documents;
    using Ploeh.AutoFixture;
    using Shouldly;
    using Xunit;

    public class Item
    {
        private int Id { get; set; }
    }

    public class ChangeTrackerTests
    {
        private ChangeTracker GetChangeTracker()
        {
            return new ChangeTracker();
        }

        [Fact]
        public void insert_change_should_be_tracked()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            var id = person.Id.GetHashCode();
            changeTracker.Insert(person);
            changeTracker.Inserts.First().ShouldBeSameAs(person);
        }
        
        [Fact]
        public void insert_change_should_be_in_change_list()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Insert(person);
            changeTracker.Changes.First().Document.ShouldBeSameAs(person);
        }

        [Fact]
        public void update_untracked_change_should_fail()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            Should.Throw(() => { changeTracker.Update(person); }, typeof(InvalidOperationException));
        }

        [Fact]
        public void update_change_should_be_tracked()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Track(person);
            changeTracker.Update(person);
            changeTracker.Updates.First().ShouldBeSameAs(person);
        }

        [Fact]
        public void update_change_should_be_in_change_list()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Track(person);
            person.FullName = "new name";
            changeTracker.Update(person);
            changeTracker.Changes.First().Document.ShouldBeSameAs(person);
        }

        [Fact]
        public void delete_change_should_tracked()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Insert(person);
            changeTracker.Delete(person);
            changeTracker.Deletions.First().ShouldBeSameAs(person);
        }

        [Fact]
        public void delete_untracked_change_should_fail()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            Should.Throw(() => { changeTracker.Delete(person); }, typeof(InvalidOperationException));
        }

        [Fact]
        public void delete_unknown_id_should_fail()
        {
            var changeTracker = GetChangeTracker();
            Should.Throw(() => { changeTracker.DeleteById(Guid.NewGuid()); }, typeof(InvalidOperationException));
        }

        [Fact]
        public void saved_documents_should_not_be_seen_as_changes()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Insert(person);
            var person2 = fixture.Create<Person>();
            changeTracker.Insert(person2);
            changeTracker.MarkChangesSaved();
            changeTracker.Changes.ShouldBeEmpty();
        }

        [Fact]
        public void should_expose_inserts_for_type()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Insert(person);
            var simple = fixture.Create<SimpleDoc>();
            changeTracker.Insert(simple);
            changeTracker.InsertsFor<Person>().Count().ShouldBe(1);
        }

        [Fact]
        public void should_expose_updates_for_type()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Track(person);
            var simple = fixture.Create<SimpleDoc>();
            changeTracker.Track(simple);
            person.FullName = "test";
            simple.Description = "also test";
            changeTracker.Update(person);
            changeTracker.Update(simple);
            changeTracker.UpdatesFor<SimpleDoc>().Count().ShouldBe(1);
        }

        [Fact]
        public void should_expose_deletions_for_type()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Track(person);
            var person2 = fixture.Create<Person>();
            changeTracker.Track(person2);
            var simple = fixture.Create<SimpleDoc>();
            changeTracker.Track(simple);
            var simple2 = fixture.Create<SimpleDoc>();
            changeTracker.Track(simple2);
            changeTracker.Delete(person);
            changeTracker.Delete(simple);
            changeTracker.DeletionsFor<SimpleDoc>().Count().ShouldBe(1);
        }

        [Fact]
        public void should_clear_changes()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var person = fixture.Create<Person>();
            changeTracker.Track(person);
            person.FullName = "test";
            changeTracker.Update(person);
            changeTracker.Delete(person);
            var simple = fixture.Create<SimpleDoc>();
            changeTracker.Track(simple);
            simple.Description = "test";
            changeTracker.Update(simple);
            var simple2 = fixture.Create<SimpleDoc>();
            changeTracker.Insert(simple2);
            changeTracker.ClearChanges();
            changeTracker.Changes.ShouldBeEmpty();
        }

        [Fact]
        public void should_not_track_invalid_doc()
        {
            var changeTracker = GetChangeTracker();
            var fixture = new Fixture();
            var invalidDoc = fixture.Create<InvalidDoc>();
            Should.Throw(() => { changeTracker.Track(invalidDoc); }, typeof(InvalidOperationException));
        }
    }
}