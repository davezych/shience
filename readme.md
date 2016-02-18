#Shience
A C# library for carefully refactoring critical paths. It's a .NET(ish) port of [Github's Scientist library](https://github.com/github/scientist).

##How do I do science?
Let's pretend you're changing the way you're handling permissions. Unit tests help, but it's useful to compare behaviors under load, in real conditions. Shience helps with that.

```csharp
//Create an experiment
var userCanRead = Science.New<bool>("widget-permissions")
    .Test(control: () => UserPermissions.CheckUser(currentUser), 
          candidate: () => User.Can(currentUser, Permission.Read))
    .PublishTo((e) => { Console.WriteLine(e.Matched); })
    .Execute();

if(userCanRead)
{
    //Do things!
}
```

Shience will run the control (the old way) and the candidate (the new way) in random order. It will return the control result to you for use, but will also compare the control result with the candidate result to determine whether the behaviors are the same. It will publish the comparison result using the publisher specified.

##Context
Test results sometimes aren't useful without context. You can add objects that you might feel are useful when viewing comparison results. The context objects will be published with the rest of the data.

```csharp
var userCanRead = Science.New<bool>("context")
    .Test(control: () => return UserPermissions.CheckUser(currentUser), 
          candidate: () => return User.Can(currentUser, Permission.Read))
    .WithContext(new { 
        Class = nameof(MyClass),
        Method = nameof(MyMethod),
        User = currentUser,  
        Timestamp = DateTime.UtcNow 
    })
    .PublishTo(result => DoSomething(result.Context))
    .Execute();
```

##Conditional runs
Sometimes you don't want to science. If that's the case, you can specify a predicate indicating whether or not to skip the test using the `Where` method. A value of `true` indicates the test will run, a value of `false` indicates the test should be *skipped*.

```csharp
var userCanRead = Science.New<bool>("conditional")
    .Test(control: () => return UserPermissions.CheckUser(currentUser),
          candidate: () => return User.Can(currentUser, Permission.Read))
    .Where(() => !currentUser.IsAdmin) //Only run if user is not an admin
    .Execute();
```

###Ramping up experiments
The `Where` method can be used to specify a percentage of time an experiment should run:

```csharp
var userCanRead = Science.New<bool>("conditional")
    .Test(control: () => return UserPermissions.CheckUser(currentUser),
          candidate: () => return User.Can(currentUser, Permission.Read))
    .Where(() => new Random().Next() % 10 == 0) //Run 10% of all requests
    .Execute();
```

This allows you to start small, ensure performance is okay and fix any immediate mismatches and then ramp up when you're ready to science all the things.

###Chaining
You can also chain `Where` calls if you have multiple conditions:

```csharp
var userCanRead = Science.New<bool>("conditional")
    .Test(control: () => return UserPermissions.CheckUser(currentUser),
          candidate: () => return User.Can(currentUser, Permission.Read))
    .Where(() => new Random().Next() % 2 == 0) //Run for 50% of requests
    .Where(() => !currentUser.IsAdmin) // Only if the user is not an admin
    .Where(() => DateTime.UtcNow.Hour >= 8 && DateTime.UtcNow.Hour < 16) //Don't run at peak hours
    .Execute();
```

##Comparing
Objects can be hard to compare. You can specify how to compare them in 2 ways.

###Override `Equals`
Shience, by default, compares results using `.Equals`. You can override `Equals` and `GetHashCode` on your object and compare that way.

```csharp
private class TestHelper
{
    public TestHelper(int number)
    {
        Number = number;
    }

    public int Number { get; }

    public override bool Equals(object obj)
    {
        var otherTestHelper = obj as TestHelper;
        if (otherTestHelper == null)
        {
            return false;
        }

        return otherTestHelper.Number == this.Number;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() ^ Number;
    }
}
```

then

```csharp
var result = Science.New<bool>("compare")
    .Test(control: () => new TestHelper(1),
          candidate: () => TestHelper(2))
    .Execute();
```

###Pass in a custom `Func<>`
You can also pass in a comparing `Func<>` to the `WithComparer` method.

```csharp
var userCanRead = Science.New<bool>("compare")
    .Test(control: () => return UserPermissions.CheckUser(currentUser), 
          candidate: () => return User.Can(currentUser, Permission.Read))
    .WithComparer((controlResult, candidateResult) => controlResult == candidateResult);
```

##Publishing
Experiments aren't helpful if you don't write down the results. To record results, call the `PublishTo` method and give it an action. For simple publishing, you can specify it inline:

```csharp
var userCanRead = Science.New<bool>("widget-permissions")
    .Test(control: () => return UserPermissions.CheckUser(currentUser),
          candidate: () => return User.Can(currentUser, Permission.Read))
    .PublishTo((e) => { Console.WriteLine($"{e.TestName} result: {e.Matched}") })
    .Execute();
```

For more advanced publishing (to write to a log, database, send to a service, or whatever) write a generic method that takes an `ExperimentResult<TResult>`:

```csharp
public IPublisher
{
    void Publish<TResult>(ExperimentResult<TResult> result);
}

public class MyPublisher : IPublisher
{
    public void Publish<TResult>(ExperimentResult<TResult> result)
    {
        //Write results somewhere
    }
}
```

In most circumstances it's best to use dependency injection to inject the publisher:

```csharp
public class MyTestProxy
{
    private IPublisher Publisher { get; }

    public(IPublisher publisher)
    {
        Publisher = publisher;
    }

    // ...
}
```

And once written, set the action when initializing an experiment:

```csharp
var userCanRead = Science.New<bool>("widget-permissions")
    .Test(control: () => UserPermissions.CheckUser(currentUser),
          candidate: () => User.Can(currentUser, Permission.Read))
    .PublishTo(Publisher.Publish)
    .Execute();
```

An `ExperimentResult` gives you lots of useful information, such as:

- The name of the test
- The result of the control and candidate
- Whether the results matched
- Whether the control or candidate test ran first (if not async)
- Any context objects you set
- Start time in UTC of the test

##Async

Tests can be run in parallel using the `ExecuteAsync` method. When run in parallel the order in which they start is no longer randomized. To run tests in parallel, `await` the `ExecuteAsync` method:

```csharp
var result = await Science.New<bool>("async")
    .Test(control: () => { Thread.Sleep(5000); return true; },
          candidate: () => { Thread.Sleep(5000); return true; })
    .ExecuteAsync();
```

##Designing an experiment

Due to the fact that both the control *and* candidate have the possibility of running, Shience **should not** be used to test **write** operations. If Shience is set up to run a write operation, it's entirely possible that the write could happen twice (which is probably not wanted). 

It's best to only do science on read operations. 
