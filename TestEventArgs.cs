using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDrivenProgram
{

    // Defining a class to hold custom-event info.
    public class TestEventArgs : EventArgs
    {

        public TestEventArgs(string message)
        {
            Message = message;
        }


        public string Message { get; set; }

    }

    // CLass that is used to publish the event
    public class Publisher
    {
        // Declare the event using EventHandler<T>
        public event EventHandler<TestEventArgs> RaiseTestEvent;

        public void DoTestLogin()
        {

        }
        
        protected virtual void OnRaiseTestEvent(TestEventArgs e)
        {
            // Create a copy of the event in case multiple subscribers try to call an event at the same time
            EventHandler<TestEventArgs> raiseEvent = RaiseTestEvent;

            // Check if the event has subscribers
            if (raiseEvent != null)
            {
                e.Message += $" at {DateTime.Now}";

                // Call to raise the event
                raiseEvent(this, e);
            }
        }
    }
}
