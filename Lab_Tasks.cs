// Apply SOLID design principles on the following code samples for better design

    // 1.
    // a. Based on specifications, we need to create an interface and a TeamLead class to implement it.
    // b. Later another role like Manager, who assigns tasks to TeamLead and will not work on the tasks, is introduced into the system,
    // Apply needed refactoring to for better design and mention the current design smells
    public Interface ILead
    {  
        void CreateSubTask();  
        void AssginTask();  
        void WorkOnTask();
    }
    public class TeamLead : ILead
    {
        public void AssignTask()
        {
            // create a task
            Task t = new Task() {Title = "Merge and Deploy", Description = "Task to merge and deploy sharing feature to develop"};

            //Code to assign a task. 
            t.AssignTo(new Developer() { Name="Developer1" });
        }
        public void CreateSubTask()
        {
            //Code to create a sub task  
        }
        public void WorkOnTask()
        {
            //Code to implement perform assigned task.  
        }
    }

/********************************Solution 1 ************************************/
// single responsibility : the team lead class have many responsibilities
// the task is tightly coupled in the AssignTask() and also the Developer => sol. delegate the creation 
// DIP (Dependency Inversion Principle): depend on abstractions (ITask, IDeveloper)
// Interface segregation: instead of creating another class for manager and repeating half the function of lead => sol. create another general interface that ILead and IManage can implement

public interface IRole
{
    void AssignTask();
}
public interface ILead:IRole
{  
    void CreateSubTask();   
    void WorkOnTask();
}
public interface IDeveloper
{
    public string Name { get; set; }
}
public interface ITask
{
    string Title { get; set; }
    string Description { get; set; }
    void AssignTo(IDeveloper developer);
}

public class TeamLead : ILead, IDeveloper
{
    string Name { get; set; }
    ITask task { get;set;}
    IDeveloper developer { get; set; }
    public TeamLead( ITask _task, IDeveloper _developer)
    {
        task = _task;
        developer = _developer;
    }
    public void AssignTask()
    {
        // create a task
        task.Title = "Merge and Deploy";
        task.Description = "Task to merge and deploy sharing feature to develop";

        //Code to assign a task. 
        developer.Name = "Developer1";
        task.AssignTo(developer);
    }
    public void CreateSubTask()
    {
        //Code to create a sub task  
    }
    public void WorkOnTask()
    {
        //Code to implement perform assigned task.  
    }
}

public class Manager : IRole
{
    ITask task { get; set; }
    ILead teamLead { get; set; }
    public Manager(ITask _task, ILead _teamLead)
    {
        task = _task;
        teamLead = _teamLead;   
    }
    public void AssignTask()
    {
        // create a task
        task.Title = "Merge and Deploy";
        task.Description = "Task to merge and deploy sharing feature to develop";

        //Code to assign a task. 
        teamLead.Name = "teamLead1";
        task.AssignTo(teamLead);
    }
}

/**************************************************************************/

// 2. Client need to build an application to manage data using group of SQL files.
// a. we need to develop load text and save text functionalities for group of SQL files in the application directory.
// b. we need a manager class that manages the load and saves the text of group of SQL files along with the SqlFile Class.
public class SqlFile
    {
        public string FilePath { get; set; }
        public string FileText { get; set; }
        public string LoadText()
        {
            /* Code to read text from sql file */
        }
        public string SaveText()
        {
            /* Code to save text into sql file */
        }
    }
    public class SqlFileManager
    {
        public List<SqlFile> lstSqlFiles { get; set}

        public string GetTextFromFiles()
        {
            StringBuilder objStrBuilder = new StringBuilder();
            foreach (var objFile in lstSqlFiles)
            {
                objStrBuilder.Append(objFile.LoadText());
            }
            return objStrBuilder.ToString();
        }
        public void SaveTextIntoFiles()
        {
            foreach (var objFile in lstSqlFiles)
            {
                objFile.SaveText();
            }
        }
    }


/********************************Solution 2 ************************************/
//single responsibility : SqlFile class has more than 1 res. it loads and saves ==> sol. implements 2 interfaces
//DIP : the SqlFileManager depends on SqlFile not on abstraction => make it depends on the Iload and ISave not the sqlFile itself
public interface ILoad
{
    string LoadText();
}
public interface ISave
{
    string SaveText();
}
public class LoadTextSqlFile : ILoad
{
    public string LoadText(SqlFile file)
    {
        /* Code to read text from sql file */
    }
}
public class SaveTextSqlFile: ISave
{
    public string SaveText(SqlFile file)
    {
        /* Code to save text into sql file */
    }
}
public class SqlFile
{
    public string FilePath { get; set; }
    public string FileText { get; set; }
}
public class SqlFileManager
{
    public List<SqlFile> lstSqlFiles { get; set}
    ILoad load { get; set; }
    ISave save { get; set; }

    public SqlFileManager(ILoad _load, ISave _save)
    {
        load = _load;
        save = _save;
    }

    public string GetTextFromFiles()
    {
        StringBuilder objStrBuilder = new StringBuilder();
        foreach (var objFile in lstSqlFiles)
        {
            var content = load.LoadText(objFile);
            objStrBuilder.Append(content);
        }
        return objStrBuilder.ToString();
    }
    public void SaveTextIntoFiles()
    {
        foreach (var objFile in lstSqlFiles)
        {
            save.SaveText(objFile);
        }
    }
}




/********************************************************************************/

// c. New Requirement:
// After some time our leaders might tell us that we may have a few read-only files in the application folder, 
// so we need to restrict the flow whenever it tries to do a save on them.
public class SqlFile
    {
        public string FilePath { get; set; }
        public string FileText { get; set; }
        public string LoadText()
        {
            /* Code to read text from sql file */
        }
        public void SaveText()
        {
            /* Code to save text into sql file */
        }
    }
    public class ReadOnlySqlFile : SqlFile
    {
        public string LoadText()
        {
            /* Code to read text from sql file */
        }
        public void SaveText()
        {
            /* Throw an exception when app flow tries to do save. */
            throw new IOException("Can't Save");
        }
    }

    // d. To avoid an exception we need to modify "SqlFileManager" by adding one condition to the loop.
    public class SqlFileManager
    {
        public List<SqlFile> lstSqlFiles { get; set; }

        public string GetTextFromFiles()
        {
            StringBuilder objStrBuilder = new StringBuilder();
            foreach (var objFile in lstSqlFiles)
            {
                objStrBuilder.Append(objFile.LoadText());
            }
            return objStrBuilder.ToString();
        }
        public void SaveTextIntoFiles()
        {
            foreach (var objFile in lstSqlFiles)
            {
                //Check whether the current file object is read-only or not.If yes, skip calling it's  
                // SaveText() method to skip the exception.  

                if (!objFile is ReadOnlySqlFile)
                    objFile.SaveText();
            }
        }
    }

//e. Apply needed refactoring to for better design and mention the current design smells




/********************************Solution 3 ************************************/
//single responsibility : SqlFile class has more than 1 res. it loads and saves ==> sol. implements 2 interfaces
//DIP : the SqlFileManager depends on SqlFile not on abstraction => make it depends on the Iload and ISave not the sqlFile itself
//Liskov : it treats ReadOnlySqlFile  as SqlFiles
//interface segregation: it forces ReadOnlySqlFile to have save and then throws exception in it => sol. separate the functions

public interface ILoad
{
    string LoadText(SqlFile file);
}
public interface ISave
{
    string SaveText(SqlFile file);
}
public class LoadTextSqlFile : ILoad
{
    public string LoadText(SqlFile file)
    {
        /* Code to read text from sql file */
    }
}
public class SaveTextSqlFile : ISave
{
    public string SaveText(SqlFile file)
    {
        /* Code to save text into sql file */
    }
}
public class SqlFile
{
    public string FilePath { get; set; }
    public string FileText { get; set; }
}
public class ReadOnlySqlFile : ILoad
{
    public string LoadText(SqlFile file)
    {
        /* Code to read text from sql file */
    }
}
public class SqlFileManager
{
    public List<SqlFile> lstSqlFiles { get; set}
    ILoad load { get; set; }
    ISave save { get; set; }

    public SqlFileManager(ILoad _load, ISave _save)
    {
        load = _load;
        save = _save;
    }
    public string GetTextFromFiles()
    {
        StringBuilder objStrBuilder = new StringBuilder();
        foreach (var objFile in lstSqlFiles)
        {
            var content = load.LoadText(objFile);
            objStrBuilder.Append(content);
        }
        return objStrBuilder.ToString();
    }
    public void SaveTextIntoFiles()
    {
        foreach (var objFile in lstSqlFiles)
        {
            if (objFile is ISave save)  //check if it implements the ISave first
            {
                save.SaveText(objFile);
            }
        }
    }
}