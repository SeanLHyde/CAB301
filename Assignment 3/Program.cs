using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace Assignment3;
class Assignment1
{
    static public void Main(String[] args)
    {
        Methods methods = new Methods();
        var input = 0;

        //Keeps the program running so that the user can choose multiple options in  
        do
        {
            methods.Options();
            try
            {
                input = int.Parse(Console.ReadLine());
            }

            catch { }

            switch (input)
            {
                case 1:
                    methods.InputDataFromFile();
                    break;
                case 2:
                    methods.AddNewTask();
                    break;
                case 3:
                    methods.RemoveTask();
                    break;
                case 4:
                    methods.ChangeTimeofTask();
                    break;
                case 5:
                    methods.SaveToTextFile();
                    break;
                case 6:
                    methods.FindSequenceOfTasks();
                    break;
                case 7:
                    methods.FindEarliestCommencementTime();
                    break;
                default:
                    methods.Options();
                    break;
            }
        } while (input != 8);

        Console.WriteLine("Thank you, closing application");


    }

}





