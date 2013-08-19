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
  public bool paused { get; private set; }
  public bool running { get; private set; }
  private Stack<Job> childJobs;


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
  } // end Start


  /// <summary>
  /// Run the coroutine.
  /// </summary>
  /// <returns>Return the coroutine as it runs.</returns>
  public IEnumerator StartAsCoroutine()
  {
    running = true;
    yield return JobManager.Instance.StartCoroutine(Work());
  } // end StartAsCoroutine


  /// <summary>
  /// 
  /// </summary>
  public void Kill()
  {
    killed = true;
    running = false;
    paused = false;
  } // end Kill


  /// <summary>
  /// 
  /// </summary>
  /// <param name="delay"></param>
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
  /// 
  /// </summary>
  /// <param name="child"></param>
  /// <returns></returns>
  public Job CreateChildJob(IEnumerator child)
  {
    Job job = new Job(child, false);
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
      childJobs = new Stack<Job>();
    }
    childJobs.Push(child);
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
        Job childJob = childJobs.Pop();
        yield return JobManager.Instance.StartCoroutine(childJob.StartAsCoroutine());
      } while (childJobs.Count > 0);
    }
  } // end RunChildJobs

} // end Job class