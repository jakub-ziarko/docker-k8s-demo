using System;
using System.Threading;

namespace EggPlantApi
{
    public class Retry
    {
        public Action<string> Log { get; set; }

        public void Execute(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Log.Invoke(nameof(action));
                Thread.Sleep(2000);
                Execute(action);
            }
        }
    }
}
