# aiv-tween
Aiv.Tween portable .Net module. Tested with AIV libraries and Unity3D

This is a reflection-based Tween implementation + a bunch of Easing functions taken from the https://github.com/tweenjs/tween.js project

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
****************************

```cs
Tween idleAnimation = new Tween();
idleAnimation.To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f);
```

this will update the transform.position (like in Unity3D) field with the value Vector3(0, 17, 0) in 3.5 seconds

Start()
*******

Once the animation is configured you need to start it:

```cs
Tween idleAnimation = new Tween();
idleAnimation.To(transform, new { position = new Vector3(0, 17, 0) }, 3.5f);
idleAnimation.Start();
```

Update(float t)
***************

A started animation requires to be updated with time values (look at the chain usage !)

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
    idleAnimation.Update(Time.time);
  }
}
```

DeltaUpdate(float dt)
*********************

If you prefer to use the Time.deltaTime approach (instead of absolue timestamps) you can use the DeltaUpdate() method:

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
