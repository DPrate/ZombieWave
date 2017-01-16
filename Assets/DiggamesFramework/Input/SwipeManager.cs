using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UniSwipe
{
	
	/// <summary>
	/// Swipe manager's sole purpose is to control Swipe detection on update as stands now.
	/// Perhaps this can evolve into a gesture manager?
	/// </summary>
	public class SwipeManager : MonoBehaviour 
	{	
		/// <summary>
		/// The distance required to validate a swipe.
		/// This is used to initialize the Swipe.Threshold.
		/// </summary>
		[SerializeField]
		private float _swipeThreshold;  
		
		/// <summary>
		/// The starting position of a touch or mouse event (possible swipe)
		/// </summary>
		private Vector2 startPosition;
		
		/// <summary>
		/// The end position of a touch or mouse event
		/// </summary>
		private Vector2 endPosition;

		void Awake()
		{  
			Swipe.Threshold = this._swipeThreshold;
		}
		
		// checks and calls to swipe / gestures should be in update.
		void Update()
		{      
			this.DetectSwipe();
		}
		
		/// <summary>
		/// Determines the distance between mouse/touch down and up positions,
		/// and tries to find a swipe in it.
		/// </summary>
		public void DetectSwipe()
		{
			
			if (Input.touches.Length > 0 )
			{
				Touch touch = Input.GetTouch( 0 );          
				if ( touch.phase == TouchPhase.Began )
				{
					this.startPosition = touch.position;
				}
				
				else if ( touch.phase == TouchPhase.Ended )
				{              
					this.endPosition = touch.position;
					
					// The swipe class handles the leg work of detection,                
					Swipe.Detect ( this.startPosition, this.endPosition );      
				}  
			}
			
			#if UNITY_EDITOR || UNITY_STANDALONE
			else
			{
				if (Input.GetMouseButtonDown (0) )
				{
					this.startPosition = Input.mousePosition;
				}
				
				else if ( Input.GetMouseButtonUp (0) )
				{              
					this.endPosition = Input.mousePosition;                      
					Swipe.Detect ( this.startPosition, this.endPosition );
				}  
			}
			#endif
			
		}
		
		void OnApplicationQuit()
		{
			Swipe.ClearEvent ();
		}
		
	}// end SwipeManager class
	
	/// <summary>
	/// The cardinal swipe directions. The order of these values is important,
	/// as their values are used as indices by the Swipe class.
	/// </summary>
	public enum SwipeDirection
	{
		None = 0, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,  
	}

	/// <summary>
	/// The swipe class. Each actual Swipe object has only a few fields and constructors. The rest is static utility.
	/// The Swipe class can publish SwipeEvent to whom it may concern, and because it is an object instead of just
	/// a calculation, Swipes can be referenced, changed, queued, and compared if necessary. They can be created directly
	/// via vector2s or even by a SwipeDirection (complete with simulated start and end vectors),
	/// or can be created upon detection of a valid start and end vector (a distance that exceeds the threshold).
	/// They can implicitly operate as bool, Vector2, and SwipeDirection.
	/// </summary>
	public class Swipe 
	{
		/// <summary>
		/// An empty swipe instance, generated the first time it is required through the Empty property.
		/// this is to avoid using null where possible. It operates as "false", SwipeDirection.None, and Vector2.zero.
		/// This is also what is returned when Swipe.Detect(v1, v2) does not detect a valid swipe.
		/// </summary>
		private static Swipe _empty;
		public static Swipe Empty
		{
			get
			{
				if (_empty == null) _empty = new Swipe( SwipeDirection.None, publishEvent: false );
				return _empty;
			}
		}
		
		/// <summary>
		/// The 8 cardinal vectors ( and Vector2.zero for no direction). The order of these are important,
		/// as their indices correspond to the SwipeDirection enum values.
		/// This is used in iteration to determine the swipe direction from a Vector2 value.
		/// </summary>
		private static Vector2[] cardinalVectors = new Vector2[] 
		{
			new Vector2 ( 0 , 0 ),     // none.
			new Vector2 ( 0 , 1 ),    // up
			new Vector2 ( 0 ,-1 ),    // down
			new Vector2 (-1 , 0 ),    // left
			new Vector2 ( 1 , 0 ),  // right
			new Vector2 (-1 , 1 ),    // up left
			new Vector2 ( 1 , 1 ),  // up right
			new Vector2 (-1 ,-1 ),  // down left
			new Vector2 ( 1 ,-1 )   // down right
		};
		
		/// <summary>
		/// The distance required to validate a swipe.
		/// </summary>
		public static float Threshold;
		
		/// <summary>
		/// The swipe event, published optionally when a new swipe is generated or detected (published by default).
		/// </summary>
		public static EventHandler<SwipeArgs> SwipeEvent;
		
		// everything above and below this region is part of the static utility of Swipe.
		#region Instance fields and methods
		
		/// <summary>
		/// The starting position of a swipe
		/// </summary>
		public Vector2 Start{get; private set;}
		
		/// <summary>
		/// The final position of a swipe
		/// </summary>
		public Vector2 End{ get; private set;}
		
		/// <summary>
		/// The vector of a swipe, equal to End - Start
		/// </summary>
		public Vector2 Vector {get; private set;}
		
		/// <summary>
		/// The cardinal direction of the swipe. This also sets the 4 general directional bools.
		/// </summary>
		public SwipeDirection Direction {get; private set;}

		// this region contains bools for the 4 cardinal directions,
		// in case the diagonals should be ignored.
		// they are set once, the first time they are required.
		#region quad-directional values;
		private bool? _isUpward;
		public bool IsUpward
		{
			get
			{
				if (!_isUpward.HasValue)
				{
					switch (Direction)
					{
					case SwipeDirection.Up:
					case SwipeDirection.UpLeft:
					case SwipeDirection.UpRight:
						_isUpward = true;
						break;
					default: _isUpward = false;
						break;
					}
				}
				return _isUpward.Value;
			}
		}
		
		private bool? _isLeftward;
		public bool IsLeftward
		{
			get
			{
				if (!_isLeftward.HasValue)
				{
					switch (Direction)
					{
					case SwipeDirection.Left:
					case SwipeDirection.UpLeft:
					case SwipeDirection.DownLeft:
						_isLeftward = true;
						break;
					default: _isLeftward = false;
						break;
					}
				}
				return _isLeftward.Value;
			}
		}
		
		private bool? _isDownward;
		public bool IsDownward
		{
			get
			{
				if (!_isDownward.HasValue)
				{
					switch (Direction)
					{
					case SwipeDirection.Down:
					case SwipeDirection.DownLeft:
					case SwipeDirection.DownRight:
						_isDownward = true;
						break;
					default: _isDownward = false;
						break;
					}
				}
				return _isDownward.Value;
			}
		}
		private bool? _isRightward;
		public bool IsRightward
		{
			get
			{
				if (!_isRightward.HasValue)
				{
					switch (Direction)
					{
					case SwipeDirection.Right:
					case SwipeDirection.UpRight:
					case SwipeDirection.DownRight:
						_isRightward = true;
						break;
					default: _isRightward = false;
						break;
					}
				}
				return _isRightward.Value;
			}
		}
		#endregion
		
		/// <summary>
		/// Gets a value indicating whether this instance is the empty swipe.
		/// </summary>  
		public bool IsEmpty
		{  
			get
			{
				return Swipe.Empty == this;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Swipe"/> class for a given direction.
		/// This automatically generates vectors with Vector2.zero as a start,
		/// with a magnitude just barely larger than the minimum for a valid swipe.
		/// </summary>
		/// <param name="direction">The direction of the swipe.</param>
		/// <param name="publishEvent">If set to <c>true</c> publish the swipe event to subscribers..</param>
		public Swipe(SwipeDirection direction, bool publishEvent = true)
		{
			this.Direction = direction;
			this.Vector = Swipe.GetVector ( direction) * Threshold * 1.1f;
			this.Start = Vector2.zero;
			this.End = this.Vector;
			
			if (publishEvent)
			{
				Swipe.PublishSwipeEvent(this);
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Swipe"/> class using start and end vectors.
		/// Direction and swipe vector are generated from the start / end points.
		/// </summary>
		/// <param name="start">The start of the swipe</param>
		/// <param name="end">The end position of the swipe</param>
		/// <param name="publishEvent">If set to <c>true</c> publish the swipe event.</param>
		public Swipe(Vector2 start, Vector2 end, bool publishEvent = true)
		{
			this.Start = start;
			this.End = end;
			this.Vector = end - start;
			this.Direction = GetDirection (this.Vector);
			
			if (publishEvent)
			{
				Swipe.PublishSwipeEvent(this);
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Swipe"/> class using a single vector
		/// assumed to be end - start.  It generates start and end points for start as Vector2.zero,
		/// and calculates a direction value.
		/// </summary>
		/// <param name="swipeVector">The directional vector of the swipe.</param>
		/// <param name="publishEvent">If set to <c>true</c> publish the swipe event.</param>
		public Swipe(Vector2 swipeVector, bool publishEvent = true)
		{
			this.Start = Vector2.zero;
			this.End = swipeVector;
			this.Vector = swipeVector;
			this.Direction = Swipe.GetDirection(this.Vector);
			
			if (publishEvent)
			{
				Swipe.PublishSwipeEvent(this);
			}
		}
		#endregion
		
		
		/// <summary>
		/// Checks the vector between start and end to see if it exceeds the threshold.
		/// it returns the swipe if detected, or the empty swipe if the vector is too small.
		/// this acts as a conditional constructor.
		/// </summary>
		public static Swipe Detect(Vector2 start, Vector2 end, bool publishEvent = true)
		{
			Vector2 vector = end - start;
			if (vector.sqrMagnitude >= (Swipe.Threshold * Swipe.Threshold))
			{
				Swipe newSwipe = new Swipe(start, end, publishEvent);          
				return newSwipe;
			}
			return Swipe.Empty;
		}
		
		/// <summary>
		/// Checks the directional vector to see if it exceeds the threshold.
		/// it returns the swipe if detected, or the empty swipe if the vector is too small.
		/// this acts as a conditional constructor.
		/// </summary>
		public static Swipe Detect(Vector2 vector, bool publishEvent = true)
		{
			if (vector.sqrMagnitude >= (Swipe.Threshold * Swipe.Threshold))
			{
				Swipe newSwipe = new Swipe(vector, publishEvent);          
				return newSwipe;
			}      
			return Swipe.Empty;
		}
		
		#region Value conversion
		/// <summary>
		/// Using a direction, gets the cardinal vector associated.
		/// this uses the direction as an index in the previously assigned vector array.
		/// </summary>
		public static Vector2 GetVector(SwipeDirection direction)
		{
			return Swipe.cardinalVectors[(int) direction];
			
		}
		
		/// <summary>
		/// Using a start and end vector, iterate through the cardinal vectors,
		/// and find the associated direction.
		/// </summary>
		public static SwipeDirection GetDirection(Vector2 start, Vector2 end)
		{
			return GetDirection(end - start);
		}
		
		/// <summary>
		/// Using a directional vector, iterate through the cardinal vectors,
		/// and find the associated direction.
		/// </summary>
		public static SwipeDirection GetDirection(Vector2 vector)
		{
			// we start at index 1 because index 0 is None
			Vector2 normal = vector.normalized;
			for (int i = 1; i < Swipe.cardinalVectors.Length; i++)
			{
				Vector2 cardinal = Swipe.cardinalVectors[i];
				if (Vector2.Dot ( normal, cardinal) > 0.906f)
				{
					// convert the appropriate index into a swipe direction enum.
					return (SwipeDirection) i;  
				}
			}
			
			// if none of the indices fit, return none.      
			return SwipeDirection.None;      
		}
		#endregion
		
		#region event
		/// <summary>
		/// Publishs the swipe event if not null.
		/// called when constructing a swipe, if the constructor's publishEvent is true.
		/// </summary>
		private static void PublishSwipeEvent(Swipe detectedSwipe)
		{
			if (Swipe.SwipeEvent != null)          
			{
				SwipeEvent (Swipe.Empty, new SwipeArgs( detectedSwipe));
			}
		}
		
		/// <summary>
		/// Clears the event. While it's the responsibility of other scripts to unsubscribe,
		/// this utility allows a measure of safety.
		/// </summary>
		public static void ClearEvent()
		{
			SwipeEvent = null;
		}
		#endregion
		
		#region casting
		/// <summary>
		/// implicitly casts a swipe to true if it exists and is not empty, otherwise false.
		///</summary>
		public static implicit operator bool(Swipe swipe)
		{
			return (swipe == null) || swipe.IsEmpty;
		}
		
		/// <summary>
		/// implicitly casts a swipe to its directional Vector2.
		/// </summary>
		public static implicit operator Vector2(Swipe swipe)
		{
			return swipe.Vector;
		}
		
		/// <param name="swipe">Swipe.</param>
		public static implicit operator SwipeDirection(Swipe swipe)
		{
			return swipe.Direction;
		}
		#endregion
		
		public override string ToString()
		{
			return string.Format (
				"[Swipe: IsEmpty={8} Start={0}, End={1}, Vector={2}, Direction={3}, IsUpward={4}, IsLeftward={5}, IsDownward={6}, IsRightward={7}, ]",
				Start, End, Vector, Direction, IsUpward, IsLeftward, IsDownward, IsRightward, IsEmpty);
		}
	}// end Swipe class
	
	/// <summary>
	/// The event arguments sent when a Swipe publishes the Swipe.SwipeEvent,
	/// it includes the newly constructed / detected swipe as a reference.
	/// </summary>
	public class SwipeArgs: EventArgs
	{  
		public Swipe Swipe;
		
		public SwipeArgs(Swipe swipe)
		{
			this.Swipe = swipe;
		}
	}
	
} // end uniswipe namespace\  
