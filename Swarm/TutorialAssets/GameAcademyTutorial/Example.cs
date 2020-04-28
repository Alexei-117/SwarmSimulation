using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Example : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DoExample();
    }

    private void DoExample()
    {
        // Initialize variables
        // 
        NativeArray<float> resultArray = new NativeArray<float>(1, Allocator.TempJob);

        // Instantiate Job
        Simplejob simplejob = new Simplejob()
        {
            variable = 5f,
            resultArray = resultArray
        };

        AnotherJob secondJob = new AnotherJob();
        secondJob.resultArray = resultArray;

        // Schedule Job
        JobHandle handle = simplejob.Schedule<Simplejob>();

        //!! When you pass the handle, you make a dependency of this job with the first one
        //!! The first job automatically completes, so you don't have to do it
        JobHandle secondHandle = secondJob.Schedule<AnotherJob>(handle);

        // Wait for it to complete.
        //!! Normally you access or complete the job from other threads
        //!! Cannot access native array data without completing the job
        //handle.Complete();
        secondHandle.Complete();

        // Get result
        Debug.Log("Result of the example is: " + resultArray[0]);
        
        // Disposing of the array
        resultArray.Dispose();
    }

    //!! IJobs work on a single parallel thread unless otherwise specified
    private struct Simplejob : IJob
    {
        public float variable;
        public NativeArray<float> resultArray;

        public void Execute()
        {
            resultArray[0] = variable;
        }
    }

    private struct AnotherJob : IJob
    {
        public NativeArray<float> resultArray;

        public void Execute()
        {
            resultArray[0] += 1;
        }
    }
}
