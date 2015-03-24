#Shience
A .NET port(ish) of Github's Scientist library. (https://github.com/github/scientist)

##How do I do science?
Let's pretend you're doing the same example as in Scientist's example.

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
                             
Shience will run the control (the old way) and the candidate (the new way) in random order. It will compare the returned results of the methods and publish the comparison result using the publisher specified. 