# aiv-tween
Aiv.Tween portable .Net module. Tested with AIV libraries and Unity3D

This is a reflection-based Tween implementation + a bunch of Easing functions taken from the https://github.com/tweenjs/tween.js project

Its main usage is teaching reflection, delegates and events

Installation
------------

Unity3D:

Just download the unity package and import in your project

Others:

place the DLL in your project or add the Tween.cs file (found into src/). The Easing.cs file contains the easing functions
and it is totally optional

Unit tests:

they use NUnit (a complete solution is available in the repository)

Usage
-----

Each animation sequence should be mapped to a `Aiv.Tween.Tween` Object:

```cs

using Aiv.Tween;

...

Tween idleAnimation = new Tween();

```

On the Tween class you can call (and chain like in common tween implementaions) various methods describing the animation steps

To(object, parameters, time)
----------------------------

```cs
Tween idleAnimation = new Tween();
idleAnimation.To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f);
```

this will update the transform.position (like in Unity3D) field with the value Vector3(0, 17, 0) in 3.5 seconds

You can add multiple iterations to the same keyframe:


```cs
Tween idleAnimation = new Tween();
idleAnimation.To(transform, new { position = new Vector3(0, 17, 0) }, GetComponent<Renderer>().material, new {color = Color.blue}, 3.5f);
```

Note: the last argument is always the animation time/length

Start()
-------

Once the animation is configured you need to start it:

```cs
Tween idleAnimation = new Tween();
idleAnimation.To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f);
idleAnimation.Start();
```

Update(float t)
---------------

A started animation requires to be updated with time values (look at the chain usage !)

```cs
using Aiv.Tween;
using UnityEngine;

public class TweenBehaviour : MonoBehaviour {

  private Tween idleAnimation;

  void Start() {
    idleAnimation = new Tween().To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f).
                      To(transform, new { eulerAngles = new Vector3(0, 90, 0), position = new Vector3(2, 4, 6) }, 1.5f).
                      To(transform, new { eulerAngles = new Vector3(0, -90, 0) }, 1.5f).
                      Start();
  }
  
  void Update() {
    idleAnimation.Update(Time.time);
  }
}
```

DeltaUpdate(float dt)
---------------------

If you prefer to use the Time.deltaTime approach (instead of absolute timestamps) you can use the DeltaUpdate() method:

```cs
using Aiv.Tween;
using UnityEngine;

public class TweenBehaviour : MonoBehaviour {

  private Tween idleAnimation;

  void Start() {
    idleAnimation = new Tween().To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f).
                      To(transform, new { eulerAngles = new Vector3(0, 90, 0) }, 1.5f).
                      To(transform, new { eulerAngles = new Vector3(0, -90, 0) }, 1.5f).
                      Start();
  }
  
  void Update() {
    idleAnimation.DeltaUpdate(Time.deltaTime);
  }
}
```

Note for Unity: use transform.eulerAngles for rotations, as Quaternions cannot be easily interpolated

Delay(float t)
--------------

You can chain a simple sleep in you animation:

```cs
using Aiv.Tween;
using UnityEngine;

public class TweenBehaviour : MonoBehaviour {

  private Tween idleAnimation;

  void Start() {
    idleAnimation = new Tween().To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f).
                      Delay(5).
                      To(transform, new { eulerAngles = new Vector3(0, 90, 0) }, 1.5f).
                      Delay(10).
                      To(transform, new { eulerAngles = new Vector3(0, -90, 0) }, 1.5f).
                      Start();
  }
  
  void Update() {
    idleAnimation.DeltaUpdate(Time.deltaTime);
  }
}
```

this will wait 5 seconds before starting rotating the object, and 10 seconds before rotating back


Repeat(int times)
-----------------

By default each animation is played one time.

You can increase the number of played iterations using the Repeat() method:


```cs
using Aiv.Tween;
using UnityEngine;

public class TweenBehaviour : MonoBehaviour {

  private Tween idleAnimation;

  void Start() {
    idleAnimation = new Tween().To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f).
                      Delay(5).
                      To(transform, new { eulerAngles = new Vector3(0, 90, 0) }, 1.5f).
                      Delay(10).
                      To(transform, new { eulerAngles = new Vector3(0, -90, 0) }, 1.5f).
                      Repeat(10).
                      Start();
  }
  
  void Update() {
    idleAnimation.DeltaUpdate(Time.deltaTime);
  }
}
```

this will repeat the animation 10 times before ending

Note: passing -1 to Repeat() will infinitely loop the animation

Loop()
------

Like Repeat(-1)

Call(KeyFrameHandler handler)
-----------------------------

This method allows to insert custom function calls in the animation chain:

```cs
using Aiv.Tween;
using UnityEngine;

public class TweenBehaviour : MonoBehaviour {

  private Tween idleAnimation;

  void Start() {
    idleAnimation = new Tween().To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f).
                      Call(SayHello).
                      To(transform, new { eulerAngles = new Vector3(0, 90, 0) }, 1.5f).
                      Call(SayHello).
                      To(transform, new { eulerAngles = new Vector3(0, -90, 0) }, 1.5f).
                      Call(SayHello).
                      Start();
  }
  
  void Update() {
    idleAnimation.DeltaUpdate(Time.deltaTime);
  }
  
  private void SayHello(Tween t) {
    Debug.Log("Hello");
  }
}
```

will print "Hello" after each animation keyframe

you can use lambdas too

```cs
using Aiv.Tween;
using UnityEngine;

public class TweenBehaviour : MonoBehaviour {

  private Tween idleAnimation;

  void Start() {
    idleAnimation = new Tween().To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f).
                      Call(t => Debug.Log("Hello")).
                      To(transform, new { eulerAngles = new Vector3(0, 90, 0) }, 1.5f).
                      Call(t => Debug.Log("Hello")).
                      To(transform, new { eulerAngles = new Vector3(0, -90, 0) }, 1.5f).
                      Call(t => Debug.Log("Hello")).
                      Start();
  }
  
  void Update() {
    idleAnimation.DeltaUpdate(Time.deltaTime);
  }
  
}
```

Stop()
------

You can stop an animation whenever you want with the Stop() method.
The Stop() method is automatically called on Animation end.

To restart a stopped animation you need to call Start() again


Pause()/Resume()
----------------

```cs
	// Update is called once per frame
	void Update() {
		rotating.DeltaUpdate(Time.deltaTime);

		if (Input.GetMouseButtonDown(0)) {
			rotating.Pause();
		}

		if (Input.GetMouseButtonDown(1)) {
			rotating.Resume();
		}
	}
```

'rotating' here is a Tween animation

Events
------

Currently the following events are defined:

```cs
public delegate void StartHandler(Tween sender);
public delegate void StopHandler(Tween sender);
public delegate void UpdateHandler(Tween sender);
		
public event StartHandler OnStart;
public event StopHandler OnStop;
public event UpdateHandler OnUpdate;
```

Recursion Mode
--------------

By default a "fast keyframe" (a keyframe without duration, like Call, or Nop) is called soon after the current frame (to reduce flickering on debugging), but only if the current frame has a duration. This is a security measure to avoid unexpected loops. If you are absolutely sure your animation will not generate a code-loop you can enable "infinite" recursion mode using the 'recursionMode' enum field.
If you do not want the recursion mode, you can completely turn it off setting 'recursionMode' to Off.

Currently the following recursion modes are supported:

* Tween.RecursionMode.OneTime (default, call only one frame without duration after one with a duration)
* Tween.RecursionMode.Infinite (continuosly call keyframes without duration)
* Tween.RecursionMode.Off (disable recursion completely)
