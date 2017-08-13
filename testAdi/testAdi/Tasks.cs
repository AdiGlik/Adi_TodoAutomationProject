using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace test
{
    class Tasks
    {
        List<string> Active;
        List<string> Completed;
        public int numOfActiveTasks { get; set; }
        public int numOfCompletedTasks { get; set; }

        public Tasks()
        {
            Active = new List<string>();
            numOfActiveTasks = 0;
            Completed = new List<string>();
            numOfCompletedTasks = 0;
        }
        public void ChangeLists(string name , List<string> AddTo, List<string> RemoveFrom)
        {
            AddTo.Add(name);
            if (RemoveFrom.Count > 0 )
            {
                RemoveFrom.Remove(name);
                
            }
            //update counters
            numOfActiveTasks = Active.Count;
            numOfCompletedTasks = Completed.Count;
        }

        //adding to Active list meaning removing from Completed if exists
        public void AddToActiveList(string name)
        {
            ChangeLists(name, Active, Completed);
        }

        //adding to Completed list meaning Active from completing
        public void AddToCompleteList(string name)
        {
            ChangeLists(name,Completed ,Active);
        }

		public void ClearCompleted()
		{
			if (Completed.Count > 0)
			{
				Completed.Clear();
				numOfCompletedTasks = 0;
			}
		}

		public  bool CompareListToActive(ReadOnlyCollection<IWebElement> todoList)
		{
			if (todoList.Count != numOfActiveTasks)
				return false;
			int i = 0;
			foreach (IWebElement name in todoList)
			{
				if (name.Text != Active[i])
					return false;
				i++;
			}
			return true;
		}


		public bool CompareListToCompleted(ReadOnlyCollection<IWebElement> todoList)
		{
			if (todoList.Count != numOfCompletedTasks)
				return false;
			int i = 0;
			foreach (IWebElement name in todoList)
			{
				if (name.Text != Completed[i])
					return false;
				i++;
			}
			return true;
		}


		

    }
}
