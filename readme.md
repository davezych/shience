#Shience
A .NET port(ish) of Github's Scientist library. (https://github.com/github/scientist)

##How do I do science?
Let's pretend you're doing the same example as in Scientist's example and are changing the way you're handling permissions. Unit tests help, but it's useful to compare behaviors under load, in real conditions. Shience helps with that.

    //Set a publisher
    Shience.SetPublisher(typeof(FilePublisher<>), @"C:\file\path\to\results.txt");
    
    var science = Shience.New<bool>("widget-permissions");
    
    var userCanRead = science.Test(
                                    control: (() => return UserPermissions.CheckUser(currentUser); ), 
                                    candidate: (() => return User.Can(currentUser, Permission.Read); )
                             )
                             
    if(userCanRead)
    {
        //do things!
    }
                             
Shience will run the control (the old way) and the candidate (the new way) in random order. It will return the control result to you for use, but will also compare the control result with the candidate result to determine whether the behaviors are the same. It will publish the comparison result using the publisher specified.

##Context
Test results sometimes aren't useful without context. You can add objects that you might feel are useful when viewing comparison results. The context objects can be published with the specified Publisher.

    var userCanRead = science.Test(
                                    control: (() => return UserPermissions.CheckUser(currentUser); ), 
                                    candidate: (() => return User.Can(currentUser, Permission.Read); ),
                                    contexts: new[] {currentUser, "Within DisplayWidget method", DateTime.UtcNow }
                                );
                                
##Comparing
Objects can be hard to compare. You can specify how to compare them in 2 ways.

###Override `Equals`
Shience, by default, compares results using `.Equals`. You can override `Equals` and `GetHashCode` on your object and compare that way.

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

###Pass in a custom `Func<>`
You can also pass in a comparing `Func<>` to the `Test` method.

    var userCanRead = science.Test(
                                    control: (() => return UserPermissions.CheckUser(currentUser); ), 
                                    candidate: (() => return User.Can(currentUser, Permission.Read); ),
                                    comparer: (controlResult, candidateResult) => { return controlResult == candidateResult; }
                             )