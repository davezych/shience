#Shience
A C# library for carefully refactoring critical paths. It's a .NET port(ish) of Github's Scientist library. (https://github.com/github/scientist)

##How do I do science?
Let's pretend you're changing the way you're handling permissions. Unit tests help, but it's useful to compare behaviors under load, in real conditions. Shience helps with that.

```csharp
var science = Shience.New<bool>("widget-permissions", (e) => { /*Publish results*/ });

var userCanRead = science.Test(
        control: () => return UserPermissions.CheckUser(currentUser), 
        candidate: () => return User.Can(currentUser, Permission.Read))
    .Execute();

if(userCanRead)
{
    //do things!
}
```

Shience will run the control (the old way) and the candidate (the new way) in random order. It will return the control result to you for use, but will also compare the control result with the candidate result to determine whether the behaviors are the same. It will publish the comparison result using the publisher specified.

##Publishing
When instantiating a new experiment you need to provide a publisher so you have a way to record the results. The simplist way is to provide a lambda:

```csharp
Shience.New<bool>("widget-permissions", (experimentResults) => {
    using(var sw = new StreamWriter(@"/my/file/path/results.txt"))
    {
        sw.Write(experimentResults.TestName);
        sw.Write(experimentResults.Matched);
        //etc
    }
});
```

For more complex publishing, it's best to create a class with a `void<TResult>(ExperimentResult<TResult>)` method. For instance, if you're using Entity Framework and want to publish to your database:

```csharp
public class DatabasePublisher
{
    MyEfContext _context;
    public DatabasePublisher(MyEfContext context)
    {
        _context = context;
    }

    public void Publish<TResult>(ExperimentResult<TResult> e)
    {
        _context.PublishingResults.Add(new PublishingResult {
            TestName = e.TestName,
            Matched = e.Matched,
            //etc
        });

        _context.SaveChanges();
    }
}

var science = Shience.New<bool>("widget-permissions", new DatabasePublisher(myEfContext).Publish);
```

##Context
Test results sometimes aren't useful without context. You can add objects that you might feel are useful when viewing comparison results. The context objects will be published with the rest of the data.

```csharp
var userCanRead = science.Test(
        control: () => return UserPermissions.CheckUser(currentUser), 
        candidate: () => return User.Can(currentUser, Permission.Read))
    .WithContext(contexts: new[] {currentUser, "Within DisplayWidget method", DateTime.UtcNow })
    .Execute();
```

##Conditional runs
Sometimes you don't want to science. If that's the case, you can specify a predicate indicating whether or not to skip the test using the `Where` method. A value of `true` indicates the test will run, a value of `false` indicates the test should be *skipped*.

```csharp
var userCanRead = science.Test(
        control: () => return UserPermissions.CheckUser(currentUser),
        candidate: () => return User.Can(currentUser, Permission.Read))
    .Where(() => !user.IsAdmin) //Only run if user is not an admin
    .Execute();
```

###Ramping up experiments
The `Where` method can be used to specify a percentage of time an experiment should run:

```csharp
var userCanRead = science.Test(
        control: () => return UserPermissions.CheckUser(currentUser),
        candidate: () => return User.Can(currentUser, Permission.Read))
    .Where(() => new Random().Next() % 10 == 0) //Run 10% of all requests
    .Execute();
```

This allows you to start small, ensure performance is okay and fix any immediate mismatches and then ramp up when you're ready to science all the things.

###Chaining
You can also chain `Where` calls if you have multiple conditions:

```csharp
var userCanRead = science.Test(
        control: () => return UserPermissions.CheckUser(currentUser),
        candidate: () => return User.Can(currentUser, Permission.Read))
    .Where(() => new Random().Next() % 2 == 0)
    .Where(() => !currentUser.IsAdmin)
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
var result = science.Test(
                        control: () => new TestHelper(1),
                        candidate: () => TestHelper(2))
                    .Execute();
```

###Pass in a custom `Func<>`
You can also pass in a comparing `Func<>` to the `WithComparer` method.

```csharp
var userCanRead = science.Test(
                              control: () => return UserPermissions.CheckUser(currentUser), 
                              candidate: () => return User.Can(currentUser, Permission.Read))
                         .WithComparer((controlResult, candidateResult) => controlResult == candidateResult);
```

##Async

Tests can be run in parallel using the `ExecuteAsync` method. When run in parallel the order in which they start is no longer randomized. To run tests in parallel, `await` the `ExecuteAsync` method:

```csharp
var result = await science.Test(
                                control: () => { Thread.Sleep(5000); return true; },
                                candidate: () => { Thread.Sleep(5000); return true; })
                          .ExecuteAsync();
```

##Designing an experiment

Due to the fact that both the control *and* candidate have the possibility of running, Shience **should not** be used to test **write** operations. If Shience is set up to run a write operation, it's entirely possible that the write could happen twice (which is probably not wanted). 

It's best to only do science on read operations. 

