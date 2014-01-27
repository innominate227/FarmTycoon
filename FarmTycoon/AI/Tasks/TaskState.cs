using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// Possible States a task can be in
    /// </summary>
    public enum TaskState
    {
        Planning,   //The task is being planned it may or may not ever be started
        //Possible Next States: Waiting, or unreferenced

        Waiting,    //The task is waiting to start, either because its start date has not yet arrived, or the resources needed to preform the task are not available
        //Possible Next States: Started, or Aborted

        Started,    //The task has been started.
        //Possible Next States: Finished, or Aborted

        Finished,   //The task was completed
        //Possible Next States: unreferenced

        Aborted,    //The task was aborted
        //Possible Next States: unreferenced
    }
}
