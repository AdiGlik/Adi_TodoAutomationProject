using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class TestCases
    {

        IWebDriver driver;
        WebDriverWait wait;
        string[] tasksToEnter;
        Tasks tasksObj;
        StreamWriter resultsFile;
        public bool isPassed { get; set; } //give option to read it from the main program ay any point

        public TestCases(IWebDriver _driver, WebDriverWait _wait, string[] _tasksToEnter, StreamWriter _resultsFile)
        {
            driver = _driver;
            wait = _wait;
            tasksToEnter = _tasksToEnter;
            resultsFile = _resultsFile;
            tasksObj = new Tasks();
            isPassed = true;
        }

        public void Test_InsertItems()
        {

            wait.Until(d => d.FindElement(By.Id("new-todo")));
            foreach (string nameToEnter in tasksToEnter)
            {
                AddItem(nameToEnter);
                AssertTestCase(tasksObj.numOfActiveTasks, GetNumOfItems(), "num of tasks as written below updated");
            }
            //after done entering all names, verify that indeed they are inside: 1. by number, 2 . by name

            IWebElement button = driver.FindElement(By.Id("todo-list"));
            ReadOnlyCollection<IWebElement> tasksList = button.FindElements(By.TagName("li"));
            if (tasksList.Count > 0)
            {
                if (AssertTestCase(tasksList.Count, tasksToEnter.Length, "number of items in list"))
                {

                    for (int i = 0; i < tasksList.Count; i++)
                    {
                        AssertTestCase(tasksToEnter[i], tasksList[i].Text, "name of task");
                    }
                }

            }

        }

     
        public void Test_ClearCompleted()
        {
            AddItem("four");
            IWebElement button = driver.FindElement(By.Id("clear-completed"));
            button.Click();
            tasksObj.ClearCompleted();
            if (tasksObj.numOfActiveTasks > 0)
                AssertTestCase(tasksObj.numOfActiveTasks, GetNumOfItems(), "Active number didn't change");
            //verify completed list is empty
            Test_ButtonsLists(State.Complete);

        }



        public void Test_ButtonsLists(State state)
        {
            string listName = state == State.Complete ? "completed" : "active";

            IWebElement ListObj = driver.FindElement(By.CssSelector("a[href*='#!" + listName + "']"));
            if (ListObj.Displayed)
            {

                ListObj.Click();
                IWebElement todoList = driver.FindElement(By.Id("todo-list"));
                ReadOnlyCollection<IWebElement> tasksList = todoList.FindElements(By.TagName("li"));
                if (tasksList.Count > 0)
                {
                    //check names
                    string CheckListSucceedd;
                    if (state == State.Complete)
                        CheckListSucceedd = tasksObj.CompareListToCompleted(tasksList) ? "passed" : "failed";
                    else
                        CheckListSucceedd = tasksObj.CompareListToActive(tasksList) ? "passed" : "failed";
                    resultsFile.WriteLine("link to " + listName + " list show the correct tasks" + ":  test " + CheckListSucceedd);

                }
                else //verify that indeed there shouldn't be any tasks
                {
                    if (state == State.Complete)
                        AssertTestCase(tasksObj.numOfCompletedTasks, 0, "indeed no tasks in list completed");
                    else
                        AssertTestCase(tasksObj.numOfActiveTasks, 0, "indeed no tasks in list Active");
                }
            }

        }

        public void Test_ToggleAllItems(State toggleState)
        {
            //move to "all" view in order to toggle all items
            driver.FindElement(By.CssSelector("a[href*='#!']")).Click();

            ReadOnlyCollection<IWebElement> toggleList = driver.FindElements(By.ClassName("toggle"));

            for (int i = 0; i < toggleList.Count; i++)
            {
                ToggleItem(toggleList[i], tasksToEnter[i], toggleState);

                //check that num of items display was reduced\increased
                AssertTestCase(tasksObj.numOfActiveTasks, GetNumOfItems(), "num of " + toggleState.ToString() + " tasks was updated after toggeling");

            }
            //check both lists are updated
            Test_ButtonsLists(State.Active);
            Test_ButtonsLists(State.Complete);


        }

        public int GetNumOfItems()
        {
            IWebElement todoCount = driver.FindElement(By.Id("todo-count"));
            string[] splitString = todoCount.Text.Split();
            int numOfItems = Convert.ToInt32(splitString[0]);
            return numOfItems;
        }

        /* this function does AddItem
       * and updates the simulator tasks list
       */
        public void AddItem(string name)
        {
            driver.FindElement(By.Id("new-todo")).SendKeys(name + "\n");
            tasksObj.AddToActiveList(name);

        }

        /* this function does ToggleItem
       * and updates the simulator tasks list
       */
        public void ToggleItem(IWebElement element, string name, State state)
        {
            element.Click();
            if (state == State.Complete)
                tasksObj.AddToCompleteList(name);
            else
                tasksObj.AddToActiveList(name);
        }

        public bool AssertTestCase(Object expected, Object actual, string info)
        {
            if (!expected.Equals(actual))
            {
                string message = "testFailed, expected " + expected.ToString() + "  " + info + ", recieved " + actual.ToString();
                resultsFile.WriteLine(message);
                isPassed = false;
                return false;
            }
            resultsFile.WriteLine(info + ":  test passed");
            return true;
        }

       
    }
}
