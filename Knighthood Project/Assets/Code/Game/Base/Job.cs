// Steve Yeager
// 8.18.2013
// prime31studios

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

/// <summary>
/// Wrapper for the coroutine class.
/// </summary>
public class Job
{
  public event Action<bool> JobCompleteEvent;
  private IEnumerator coroutine;
  private bool killed;
  private float runtime;
  public bool paused { get; private set; }
  public bool running { get; private set; }
  private Queue<Job> childJobs;


  /// <summary>
  /// 
  /// </summary>
  /// <param name="coroutine"></param>
  /// <param name="start"></param>
  public Job(IEnumerator coroutine, bool start = true)
  {
    this.coroutine = coroutine;
    if (start)
    {
      Start();
    }
  } // end Job


  /// <summary>
  /// 
  /// </summary>
  /// <param name="coroutine"></param>
  /// <param name="runtime"></param>
  /// <param name="start"></param>
  public Job(IEnumerator coroutine, float runtime, bool start = true)
  {
    this.coroutine = coroutine;
    this.runtime = runtime;
    if (start)
    {
      Start();
    }
  } // end Job


  /// <summary>
  /// Pause the job.
  /// </summary>
  public void Pause()
  {
    paused = true;
  } // end Pause


  /// <summary>
  /// Unpause the job.
  /// </summary>
  public void UnPause()
  {
    paused = false;
  } // end UnPause


  /// <summary>
  /// Toggle pause.
  /// </summary>
  public void TogglePause()
  {
    paused = !paused;
  } // end TogglePause


  /// <summary>
  /// Run the coroutine.
  /// </summary>
  public void Start()
  {
    running = true;
    JobManager.Instance.StartCoroutine(Work());

    if (runtime > 0f)
    {
      End(runtime);
    }
  } // end Start


  /// <summary>
  /// Run the coroutine.
  /// </summary>
  /// <returns>Return the coroutine as it runs.</returns>
  public IEnumerator StartAsCoroutine()
  {
    running = true;
    if (runtime > 0f)
    {
      End(runtime);
    }
    yield return JobManager.Instance.StartCoroutine(Work());
  } // end StartAsCoroutine


  /// <summary>
  /// End Job by killing it.
  /// </summary>
  public void Kill()
  {
    killed = true;
    running = false;
    paused = false;
  } // end Kill


  /// <summary>
  /// End Job by killing it.
  /// </summary>
  /// <param name="delay">Time in seconds to delay before killing.</param>
  public void Kill(float delay)
  {
    delay *= 1000;
    new Timer(obj =>
      {
        lock (this)
        {
          Kill();
        }
      }, null, (int)delay, Timeout.Infinite);
  } // end Kill


  /// <summary>
  /// End Job without killing it.
  /// </summary>
  public void End()
  {
    killed = false;
    running = false;
    paused = false;
  } // end End


  /// <summary>
  /// End Job without killing it.
  /// </summary>
  /// <param name="delay">Delay in seconds before ending.</param>
  private void End(float delay)
  {
    delay *= 1000;
    new Timer(obj =>
    {
      lock (this)
      {
        End();
      }
    }, null, (int)delay, Timeout.Infinite);
  } // end End


  /// <summary>
  /// 
  /// </summary>
  /// <param name="child"></param>
  /// <returns></returns>
  public Job CreateChildJob(IEnumerator child, float runtime = 0f)
  {
    Job job = new Job(child, runtime, false);
    AddChildJob(job);
    return job;
  } // end CreateChildJob


  /// <summary>
  /// 
  /// </summary>
  /// <param name="child"></param>
  public void AddChildJob(Job child)
  {
    if (childJobs == null)
    {
      childJobs = new Queue<Job>();
    }
    childJobs.Enqueue(child);
  } // end AddChildJob


  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  private IEnumerator Work()
  {
    // return null in case of starting paused
    yield return null;

    while (running)
    {
      if (paused)
      {
        yield return null;
      }
      else
      {
        if (coroutine.MoveNext())
        {
          yield return coroutine.Current;
        }
        else
        {
          // run any child jobs
          if (childJobs != null)
          {
            yield return JobManager.Instance.StartCoroutine(RunChildJobs());
          }

          running = false;
        }
      }
    }

    // fire complete event
    if (JobCompleteEvent != null)
    {
      JobCompleteEvent(killed);
    }
  } // end Work


  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  private IEnumerator RunChildJobs()
  {
    if (childJobs != null && childJobs.Count > 0)
    {
      do
      {
        Job childJob = childJobs.Dequeue();
        yield return JobManager.Instance.StartCoroutine(childJob.StartAsCoroutine());
      } while (childJobs.Count > 0);
    }
  } // end RunChildJobs

} // end Job class