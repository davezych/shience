#Shience
A C# library for carefully refactoring critical paths. It's a .NET port(ish) of Github's Scientist library. (https://github.com/github/scientist)

##How do I do science?
Let's pretend you're changing the way you're handling permissions. Unit tests help, but it's useful to compare behaviors under load, in real conditions. Shience helps with that.

```csharp
//Set a publisher
Shience.SetPublisher(new FilePublisher(@"C:\file\path\to\results.txt"));
    
var science = Shience.New<bool>("widget-permissions");
    
var userCanRead = science.Test(
    control: (() => return UserPermissions.CheckUser(currentUser); ), 
    candidate: (() => return User.Can(currentUser, Permission.Read); )
);

if(userCanRead)
{
    //do things!
}
```
                             
Shience will run the control (the old way) and the candidate (the new way) in random order. It will return the control result to you for use, but will also compare the control result with the candidate result to determine whether the behaviors are the same. It will publish the comparison result using the publisher specified.

##Context
Test results sometimes aren't useful without context. You can add objects that you might feel are useful when viewing comparison results. The context objects will be published with the rest of the data.

```csharp
var userCanRead = science.Test(
    control: (() => return UserPermissions.CheckUser(currentUser); ), 
    candidate: (() => return User.Can(currentUser, Permission.Read); ),
    contexts: new[] {currentUser, "Within DisplayWidget method", DateTime.UtcNow }
);
```
                                
##Comparing
Objects can be hard to compare. You can specify how to compare them in 2 ways.

###Override `Equals`
Shience, by default, compares results using `.Equals`. You can override `Equals` and `GetHashCode` on your object and compare that way.

```csharp
private class TestHelper
{
    public int Number { get; set; }

    public override bool Equals(object obj)
    {
        var otherTestHelper = obj as TestHelper;
        if (otherTestHelper == null)
        {
            return false;
        }

        if (otherTestHelper.Number == this.Number)
        {
            return true;
        }

        return false;
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
                        control: (() => { return new TestHelper {Number = 1}; }),
                        candidate: (() => { return new TestHelper {Number = 2}; })
                    );
```

###Pass in a custom `Func<>`
You can also pass in a comparing `Func<>` to the `Test` method.

```csharp
var userCanRead = science.Test(
                              control: (() => return UserPermissions.CheckUser(currentUser); ), 
                              candidate: (() => return User.Can(currentUser, Permission.Read); ),
                              comparer: (controlResult, candidateResult) => { return controlResult == candidateResult; }
                         );
```

##Writing your own Publisher
To write your own custom publisher (to write to a database, or send to a service or whatever) implement `IPublisher`:

```csharp
public class MyPublisher : IPublisher
{
    public void Publish<TResult>(ExperimentResult<TResult> result)
    {
        //Write results somewhere
    }
}
```

And once written, set the publisher in Shience setup:

```csharp
Shience.Shience.SetPublisher(new MyPublisher());
```

##Async

Tests can be run in parallel using the `TestAsync` method. When run in parallel the order in which they start is no longer randomized. To run tests in parallel, `await` the `TestAsync` method:

```csharp
var result = await science.TestAsync(
                                  control: (() => { Thread.Sleep(5000); return true; }),
                                  candidate: (() => { Thread.Sleep(5000); return true; }),
                              );
```

##Designing an experiment

Due to the fact that both the control *and* candidate have the possibility of running, Shience **should not** be used to test **write** operations. If Shience is set up to run a write operation, it's entirely possible that the write could happen twice (which is probably not wanted). 

It's best to only do science on read operations. 
