using UnityEngine;

namespace Script.View_Controller.Character_System.HFSM.Util
{
	/// <summary>
	/// Default timer that calculates the elapsed time based on Time.time.
	/// </summary>
	public class Timer : ITimer
	{
		private float startTime;
		public float Elapsed => Time.time - startTime;

		public Timer()
		{
			startTime = Time.time;
		}

		public void Reset()
		{
			startTime = Time.time;
		}

		public static implicit operator float(Timer timer) => timer.Elapsed;
	}
}
