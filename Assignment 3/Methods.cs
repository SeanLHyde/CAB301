using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3
{
    public class Methods
    {
        private Dictionary<string, (int, List<string>)> tasks;

        public void Options()
        {
            Console.WriteLine("");
            Console.WriteLine("What would you like to do? Your options are: ");
            Console.WriteLine("1. Input data from text file");
            Console.WriteLine("2. Add a new task to the project");
            Console.WriteLine("3. Remove a task from the project");
            Console.WriteLine("4. Change the time a task needs to be completed");
            Console.WriteLine("5. Save the updated information into a text file");
            Console.WriteLine("6. Find a sequence of tasks that does not violate any task dependency");
            Console.WriteLine("7. Find the earliest possible commencement time for each of the tasks in the project");
            Console.WriteLine("8. End");
            Console.WriteLine("Please type a number between 1-8 to select the action you'd like to do");
        }

        public void InputDataFromFile()
        {
            //Get the path for the file from the user
            Console.WriteLine("Please enter the path to the file you'd like to read from");
            string path = Console.ReadLine();

            //Check if the user provided a path
            if (path == null) { throw new Exception("Text file name is required"); }

            //Create a new Dictionary to save the tasks to if there isn't one already
            if (tasks == null)
            {
                tasks = new Dictionary<string, (int, List<string>)>();
            }

            //Read each line of the file
            //Write each line to the console for the user to see
            //Add each task to the Dictionary
            foreach (var line in File.ReadLines(path))
            {
                Console.WriteLine(line);
                string[] el = line.Split(new string[] { ", " }, StringSplitOptions.None);

                // Validation 
                int.TryParse(el[1], out var res);

                //Get task details
                string task = el[0];
                int time = Convert.ToInt32(res);
                List<string> dependencies = new List<string>();

                for (int i = 2; i < el.Length; i++)
                {
                    dependencies.Add(el[i]);
                }

                tasks.Add(task, (time, dependencies));
            }
            //Let the user know the action was successful
            Console.WriteLine("Successfully inputed data from: " + path);
        }

        public void AddNewTask()
        {
            //Asks the user to provide a task, & saves the task to input
            Console.WriteLine("Please enter the name, time and any dependencies seperated by a , and a space. Example T100, 100, T99");
            var input = Console.ReadLine();

            //Check that there is an input
            if (input == null) { throw new Exception("An input is required"); }

            string[] el = input.Split(new string[] { ", " }, StringSplitOptions.None);

            //Get task details
            string task = el[0];
            int time = Convert.ToInt32(el[1]);
            List<string> dependencies = new List<string>();

            for (int i = 2; i < el.Length; i++)
            {
                dependencies.Add(el[i]);
            }

            //If the Dictionary task is null, then create a new Dictionary & before adding the task
            //Otherwise add the task to the existing Dictionary
            if (tasks == null)
            {
                tasks = new Dictionary<string, (int, List<string>)>();
            }
            tasks.Add(task, (time, dependencies));

            //Let the user know that the task was added successfully
            Console.WriteLine("Successfully added new task");
        }

        public void RemoveTask()
        {
            //Asks the user to provide the name of the task they wish to remove
            Console.WriteLine("Please enter the name only of the task you wish to remove: ");
            string taskToRemove = Console.ReadLine();

            if (taskToRemove == null) { throw new Exception("Task name is required"); }
            //Removes the task from the Dictionary
            tasks.Remove(taskToRemove);

            //Goes through all tasks & checks their dependencies for the removed task, & if it exists as a dependency on another tasks it removes it as a dependency
            foreach (var kpv in tasks)
            {
                kpv.Value.Item2.Remove(taskToRemove);
            }

            //Let the user know that the action was successful
            Console.WriteLine("Successfully removed " + taskToRemove + " and removed it as a dependency");
        }


        public void ChangeTimeofTask()
        {

            //Get the name of the task and save it into a variable
            Console.WriteLine("Please enter the name of the task: ");
            var name = Console.ReadLine();

            //Get the time of the task and save it into a variable
            Console.WriteLine("Please enter the new time of the task: ");
            var time = int.Parse(Console.ReadLine());

            //Set the new time for the task
            if (tasks.TryGetValue(name, out var _))
            {
                tasks[name] = (time, tasks[name].Item2);
            }

            //Confirm its completed
            Console.WriteLine("Successfullt changed time of task " + name + " to " + time);
        }

        public void SaveToTextFile()
        {
            //Ask the user where they want the output saved.
            Console.WriteLine("Please enter the location and name of the file you'd like to save to: ");
            var path = Console.ReadLine();
            //var dep = tasks;
            //Output all tasks into the file
            
            File.WriteAllLines(path, tasks.Select(kvp => string.Format("{0}, {1}, {2}", kvp.Key, kvp.Value.Item1, string.Join(", ", kvp.Value.Item2.ToArray()))));
  
            //Confirms that it was saved successfully
            Console.WriteLine("Successfully saved to " + path);
        }

        //Method to return the Sequence of Tasks to the user
        public void FindSequenceOfTasks()
        {

            Console.WriteLine("Please enter a path for the file to be saved to. The file will be saved under the name Sequence.txt: ");
            var path = Console.ReadLine() + "\\Sequence.txt";
            Console.WriteLine($"File has been saved at {path}");

            if (tasks == null) Console.WriteLine("There are no tasks in the collection, please import some from a file or manually add before using this.");

            var result = CompleteFindSequenceOfTasks();

            if (result.Count == 0)
            {
                Console.WriteLine("You cannot complete all the tasks provided.");
                return;
            }

            Console.WriteLine($"You can complete the task in the following order: {string.Join(",", result)}");
            File.WriteAllLines(path, result.Select(res => string.Join(",", res)));

        }

        //Method to Generate the result of the Depth first Search
        public List<string> CompleteFindSequenceOfTasks()
        {
            HashSet<string> visited = new HashSet<string>();
            HashSet<string> cycle = new HashSet<string>();
            List<string> result = new List<string>();

            foreach (var kpv in tasks)
            {
                if (!DFSSequence(kpv.Key, visited, cycle, result))
                {
                    return new List<string>();
                }
            }

            return result;

        }

        //Method to do the Depth First Search of tasks
        public bool DFSSequence(string task, HashSet<string> visited, HashSet<string> cycle, List<string> result)
        {
            if (cycle.Contains(task))
            {
                return false;
            }
            if (visited.Contains(task))
            {
                return true;
            }

            cycle.Add(task);
            var dependencies = tasks[task].Item2;

            foreach (var de in dependencies)
            {
                if (!DFSSequence(de, visited, cycle, result))
                {
                    return false;
                }
            }

            cycle.Remove(task);
            visited.Add(task);
            result.Add(task);

            return true;

        }

        public void FindEarliestCommencementTime()
        {
            if (tasks == null) Console.WriteLine("There are no tasks in the collection, please import some from a file or manually add before using this.");

            Console.WriteLine("Please enter a path for the file to be saved to. The file will be saved under the name Sequence.txt: ");
            var path = Console.ReadLine() + "\\EarliestTimes.txt";
            Console.WriteLine($"File has been saved at {path}");

            var canComplete = CompleteFindSequenceOfTasks().Count > 0;
            if (!canComplete)
            {
                Console.WriteLine("You cannot complete all the tasks provided.");
                return;
            }

            var result = new List<(string, int)>();
            foreach (var kpv in tasks)
            {
                result.Add((kpv.Key, FindShortestTimeToStartTask(kpv.Key)));
            }

            File.WriteAllLines(path, result.Select(res => string.Format("{0}, {1}", res.Item1, res.Item2)));
        }
      

        public int FindShortestTimeToStartTask(string task)
        {
            var taskTree = tasks.ToDictionary(p => p.Key, p => (p.Value.Item1, p.Value.Item2.ToList()));
            var time = 0;

            while (taskTree[task].Item2.Count != 0)
            {
                time += FindMin(taskTree);
            }

            return time;
        }

        public int FindMin(Dictionary<string, (int, List<string>)> taskTree)
        {
            var leafNodes = new List<string>();
            var maxTime = 0;
            foreach (var kpv in taskTree)
            {
                if (kpv.Value.Item2.Count == 0)
                {
                    leafNodes.Add(kpv.Key);
                    maxTime = Math.Max(kpv.Value.Item1, maxTime);
                }
            }

            foreach (var leaf in leafNodes)
            {
                RemoveTasksFromTree(taskTree, leaf);
            }

            return maxTime;
        }

        public void RemoveTasksFromTree(Dictionary<string, (int, List<string>)> taskTree, string taskToRemove)
        {
            taskTree.Remove(taskToRemove);

            //Goes through all tasks & checks their dependencies for the removed task, & if it exists as a dependency on another tasks it removes it as a dependency
            foreach (var kpv in taskTree)
            {
                kpv.Value.Item2.Remove(taskToRemove);
            }
        }
    }

}

